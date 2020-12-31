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

        public override INodeParser NodeParser => new ArrayNodeParser();
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

        internal override string WriteSerializationData(object obj, SerializationContext context, StringBuilder sb)
        {
            sb.Append(NodeParser.WrappingStart);
            sb.Append('\n');
            context.TabPadding++;
            sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
            sb.Append("<");
            sb.Append(CustomType.FullName);
            sb.Append(SerializerConsts.DATA_SEPARATOR);
            context.Register(obj, out int newRef);
            sb.Append(newRef);
            sb.Append(">");

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
                        sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                        sb.Append($"# [{ index }] #");
                    }

                    ISerializerInternal keySerialiazer = SerializerDependencies.SerializerCollection.GetOrAdd(keyType);
                    ISerializerInternal valueSerialiazer = SerializerDependencies.SerializerCollection.GetOrAdd(valueType);
                    sb.Append('\n');
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));

                    // kv pair
                    sb.Append('{');
                    sb.Append('\n');
                    context.TabPadding++;

                    // key
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append("# key #");
                    sb.Append('\n');
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    string serializedKey = keySerialiazer.Serialize(key, context);
                    sb.Append(serializedKey);
                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    sb.Append('\n');

                    // value
                    string serializedValue = valueSerialiazer.Serialize(value, context);
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append("# value #");
                    sb.Append('\n');
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append(serializedValue);
                    context.TabPadding--;
                    sb.Append('\n');
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append('}');
                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));

                    index++;
                }

                sb.Remove(sb.Length - 2, 2);
            }
            sb.Append('\n');
            context.TabPadding--;
            sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
            sb.Append(NodeParser.WrappingEnd);


            context.SaveValue(newRef, sb.ToString());

            return sb.ToString();
        }

        internal override object DeserializeFromNodes(IList<INodeData> list, DeserializationContext context, int currentIndex)
        {
            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            object instance = Activator.CreateInstance(CollectionType.MakeGenericType(KeyType, ValueType));

            IDictionary cast = (IDictionary)instance;

            foreach (var kv in list)
            {
                var kvNodes = kv.SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();
                var key = kvNodes[0];
                var value = kvNodes[1];

                var keyElement = SerializerDependencies.SerializerCollection.GetOrAdd(KeyType).Deserialize(key.Data, context);
                var valElement = SerializerDependencies.SerializerCollection.GetOrAdd(ValueType).Deserialize(value.Data, context);

                cast.Add(keyElement, valElement);
            }

            return cast;
        }
    }
}
