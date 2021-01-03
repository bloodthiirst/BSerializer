using BSerializer.Core.Base;
using BSerializer.Core.Collection;
using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using Library.Extractors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class DictionarySerializer : CustomSerializerBase
    {
        private const string NULL = "null";
        public Type CollectionType { get; }
        public Type KeyType { get; }
        public Type ValueType { get; }

        public override INodeParser NodeParser => ArrayNodeParser.Instance;
        public DictionarySerializer(Type customType) : base(customType)
        {
            KeyType = customType.GenericTypeArguments[0];
            ValueType = customType.GenericTypeArguments[1];

            CollectionType = customType.GetGenericTypeDefinition();
        }

        internal override bool ValidateNodes(IList<INodeData> nodes)
        {
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

            IDictionary cast = (IDictionary)obj;

            ICollection keys = cast.Keys;

            int index = 0;

            if (keys.Count != 0)
            {
                foreach (object key in keys)
                {

                    Type keyType = key.GetType();

                    var value = cast[key];

                    Type valueType = value.GetType();

                    if (context.WithPropertiesComments)
                    {
                        sb.Append('\n');
                        sb.Append('\n');
                        SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                        sb.Append($"# [{ index }] #");
                    }

                    ISerializerInternal keySerialiazer = SerializerDependencies.SerializerCollection.GetOrAdd(keyType);
                    ISerializerInternal valueSerialiazer = SerializerDependencies.SerializerCollection.GetOrAdd(valueType);
                    sb.Append('\n');
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);

                    // kv pair
                    sb.Append('{');
                    sb.Append('\n');
                    context.TabPadding++;

                    // key
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    sb.Append("# key #");
                    sb.Append('\n');
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    keySerialiazer.SerializeInternal(key, context , sb);
                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    sb.Append('\n');

                    // value
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    sb.Append("# value #");
                    sb.Append('\n');
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    valueSerialiazer.SerializeInternal(value, context,sb);
                    context.TabPadding--;
                    sb.Append('\n');
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    sb.Append('}');
                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);

                    index++;
                }

                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append('\n');
            context.TabPadding--;
            SerializerUtils.GetTabSpaces(context.TabPadding, sb);
            sb.Append(NodeParser.WrappingEnd);
        }

        internal override object ReadObjectData(IList<INodeData> list, DeserializationContext context, int currentIndex)
        {
            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            object instance = Activator.CreateInstance(CollectionType.MakeGenericType(KeyType, ValueType));

            IDictionary cast = (IDictionary)instance;

            foreach (var kv in list)
            {
                var kvNodes = kv.SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();
                var key = kvNodes[0];
                var value = kvNodes[1];

                var keyElement = SerializerDependencies.SerializerCollection.GetOrAdd(KeyType).DeserializeInternal(key.Data, context);
                var valElement = SerializerDependencies.SerializerCollection.GetOrAdd(ValueType).DeserializeInternal(value.Data, context);

                cast.Add(keyElement, valElement);
            }

            return cast;
        }
    }
}
