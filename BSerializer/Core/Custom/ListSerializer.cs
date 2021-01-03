using BSerializer.Core.Base;
using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class ListSerializer : CustomSerializerBase
    {
        private const string NULL = "0";
        public override object EmptyValue => null;
        public override string EmptySymbol => NULL;
        public Type CollectionType { get; }
        public Type ElementsType { get; }
        public override INodeParser NodeParser => ArrayNodeParser.Instance;
        public ListSerializer(Type customType) : base(customType)
        {
            ElementsType = customType.GenericTypeArguments[0];
            CollectionType = customType.GetGenericTypeDefinition();
        }

        internal override bool ValidateNodes(IList<INodeData> nodes)
        {
            if (nodes.Count != 1)
            {
                return false;
            }

            if (nodes[0].Type != NodeType.ARRAY)
            {
                return false;
            }

            return true;
        }


        internal override void WriteObjectData(object obj, SerializationContext context, StringBuilder sb)
        {
            // else then we deserialize the data inside
            sb.Append(NodeParser.WrappingStart);
            sb.Append('\n');
            context.TabPadding++;
            SerializerUtils.GetTabSpaces(context.TabPadding, sb);

            context.Register(obj, out int newRef);

            WriteHeader(sb, newRef);
            sb.Append('\n');

            IEnumerable cast = (IEnumerable)obj;

            int index = 0;

            foreach (object element in cast)
            {
                Type elementType = element.GetType();


                if (context.WithPropertiesComments)
                {
                    sb.Append('\n');
                    sb.Append('\n');
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    sb.Append($"# [{ index }] #");
                }

                ISerializerInternal elementSerialiazer = SerializerDependencies.SerializerCollection.GetOrAdd(elementType);
                sb.Append('\n');
                SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                elementSerialiazer.SerializeInternal(element, context, sb);
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                index++;
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append('\n');
            context.TabPadding--;
            SerializerUtils.GetTabSpaces(context.TabPadding, sb);
            sb.Append(NodeParser.WrappingEnd);
        }

        internal override object ReadObjectData(IList<INodeData> list, DeserializationContext context, int currentIndex)
        {
            // prepare the instance to deserialize into
            object instance = Activator.CreateInstance(CollectionType.MakeGenericType(ElementsType));

            if (currentIndex != -1)
            {
                // note : we do this to count for the case where instance A has itself as one of its properties
                // so we have to preregister the parent to catch it back when we encounter it back as a property
                context.Register(currentIndex, instance);
            }

            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            IList cast = (IList)instance;

            foreach (INodeData el in list)
            {
                object desElement = SerializerDependencies.SerializerCollection
                                                            .GetOrAdd(ElementsType)
                                                            .DeserializeInternal(el.Data, context);
                cast.Add(desElement);
            }

            return cast;
        }

    }
}
