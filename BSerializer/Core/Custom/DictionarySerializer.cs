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
    public class DictionarySerializer : ISerializerInternal
    {
        private const string NULL = "null";
        public Type CollectionType { get; }
        public Type KeyType { get; }
        public Type ValueType { get; }

        public Type CustomType { get; }
        private ISerializerInternal  asInterface => this;
        public DictionarySerializer(Type customType )
        {
            CustomType = customType;

            KeyType = customType.GenericTypeArguments[0];
            ValueType = customType.GenericTypeArguments[1];

            CollectionType = customType.GetGenericTypeDefinition();

            if (!SerializerDependencies.SerializerCollection.Serializers.ContainsKey(CustomType))
            {
                SerializerDependencies.SerializerCollection.Serializers.Add(CustomType, this);
            }
        }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;

        private bool CanDeserialize(IList<INodeData> nodes , out Type type)
        {
            if(nodes[0].Type != NodeType.ARRAY)
            {
                type = null;
                return false;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            Type typeFromString = Assembly.GetEntryAssembly().GetType(typeNode.SubNodes[1].Data);

            type = typeFromString;
            return true;

        }

        public object Deserialize(string s)
        {
            if (s.Equals(EmptySymbol))
            {
                return EmptyValue;
            }


            MainParser parser = new MainParser();
            IList<INodeData> list;
            parser.ExtractNodeData(s, out list);

            Type instanceType;

            if(!CanDeserialize(list, out instanceType))
            {
                return EmptyValue;
            }

            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            object instance = Activator.CreateInstance(CollectionType.MakeGenericType(KeyType));

            IList cast = (IList)instance;

            foreach(var el in list)
            {
                var desElement = SerializerDependencies.SerializerCollection.Serializers[KeyType].Deserialize(el.Data);
                cast.Add(desElement);
            }

            return cast;
        }

        public string Serialize(object obj)
        {
            return asInterface.Serialize(obj, new SerializationSettings());
        }

        public bool TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        public bool TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }

        string ISerializerInternal.Serialize(object obj, SerializationSettings settings)
        {
            if (obj == null)
                return EmptySymbol;

            if (obj.Equals(EmptyValue))
                return EmptySymbol;

            StringBuilder sb = new StringBuilder();

            ArrayNodeParser arrayNodeParser = new ArrayNodeParser();


            sb.Append(arrayNodeParser.WrappingStart);
            sb.Append('\n');
            settings.TabPadding++;
            sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
            sb.Append("<");
            sb.Append(CustomType.FullName);
            sb.Append(">");

            IDictionary cast = (IDictionary)obj;

            ICollection keys = cast.Keys;

            int index = 0;

            foreach(object key in keys)
            {
               
                Type keyType = key.GetType();

                var value = cast[key];

                Type valueType = value.GetType();

                if (settings.WithPropertiesComments)
                {
                    sb.Append('\n');
                    sb.Append('\n');
                    sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                    sb.Append($"# [{ index }] #");
                }

                ISerializerInternal keySerialiazer = SerializerDependencies.SerializerCollection.Serializers[keyType];
                ISerializerInternal valueSerialiazer = SerializerDependencies.SerializerCollection.Serializers[valueType];
                sb.Append('\n');
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));

                // kv pair
                sb.Append('{');
                sb.Append('\n');
                settings.TabPadding++;
                
                // key
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                string serializedKey = keySerialiazer.Serialize(key, settings);
                sb.Append(serializedKey);
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                sb.Append('\n');

                // value
                string serializedValue = valueSerialiazer.Serialize(value, settings);
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                sb.Append(serializedValue);
                settings.TabPadding--;
                sb.Append('\n');
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                sb.Append('}');
                sb.Append('\n');
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                index++;
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append('\n');
            settings.TabPadding--;
            sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
            sb.Append(arrayNodeParser.WrappingEnd);

            return sb.ToString();
        }
    }
}
