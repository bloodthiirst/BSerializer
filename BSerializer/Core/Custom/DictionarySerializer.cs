﻿using BSerializer.Core.Base;
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

        private bool CanDeserialize(IList<INodeData> nodes , out Type type, out Metadata metadata, DeserializationContext context)
        {
            if(nodes[0].Type != NodeType.ARRAY)
            {
                type = null;
                metadata = null;
                return false;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            CustomSerializer serializer = new CustomSerializer(typeof(Metadata));

            metadata = (Metadata)serializer.DeserializeFromNodes(new List<INodeData>() { typeNode }, context);

            Type typeFromString = Assembly.GetEntryAssembly().GetType(metadata.TypeFullName);

            type = typeFromString;

            return true;

        }

        public object Deserialize(string s)
        {
            return asInterface.Deserialize(s, new DeserializationContext());
        }

        public string Serialize(object obj)
        {
            return asInterface.Serialize(obj, new SerializationContext());
        }

        public bool TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        public bool TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }

        string ISerializerInternal.Serialize(object obj, SerializationContext context)
        {
            if (obj == null)
                return EmptySymbol;

            if (obj.Equals(EmptyValue))
                return EmptySymbol;

            if (context.TryGet(obj, out string ser, out int reference))
            {
                return ser;
            }

            StringBuilder sb = new StringBuilder();

            ArrayNodeParser arrayNodeParser = new ArrayNodeParser();


            sb.Append(arrayNodeParser.WrappingStart);
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

            foreach(object key in keys)
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

                ISerializerInternal keySerialiazer = SerializerDependencies.SerializerCollection.Serializers[keyType];
                ISerializerInternal valueSerialiazer = SerializerDependencies.SerializerCollection.Serializers[valueType];
                sb.Append('\n');
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));

                // kv pair
                sb.Append('{');
                sb.Append('\n');
                context.TabPadding++;
                
                // key
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                string serializedKey = keySerialiazer.Serialize(key, context);
                sb.Append(serializedKey);
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                sb.Append('\n');

                // value
                string serializedValue = valueSerialiazer.Serialize(value, context);
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
            sb.Append('\n');
            context.TabPadding--;
            sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
            sb.Append(arrayNodeParser.WrappingEnd);

            context.SaveValue(newRef, sb.ToString());

            return sb.ToString();
        }

        object ISerializerInternal.Deserialize(string data, DeserializationContext context)
        {
            if (data.Equals(EmptySymbol))
            {
                return EmptyValue;
            }


            MainParser parser = new MainParser();
            IList<INodeData> list;
            parser.ExtractNodeData(data, out list);

            Type instanceType;
            Metadata metadata;
            if (!CanDeserialize(list, out instanceType , out metadata , context ))
            {
                return EmptyValue;
            }

            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            object instance = Activator.CreateInstance(CollectionType.MakeGenericType(KeyType, ValueType));

            IDictionary cast = (IDictionary)instance;

            foreach (var kv in list)
            {
                var key = kv.SubNodes[1].SubNodes[0];
                var value = kv.SubNodes[1].SubNodes[1];

                var keyElement = SerializerDependencies.SerializerCollection.Serializers[KeyType].Deserialize(key.Data , context);
                var valElement = SerializerDependencies.SerializerCollection.Serializers[ValueType].Deserialize(value.Data , context);

                cast.Add(keyElement, valElement);
            }

            return cast;
        }
    }
}
