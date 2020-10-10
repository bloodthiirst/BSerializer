using BSerializer.Core.Base;
using BSerializer.Core.Collection;
using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using Library.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class InterfaceSerializer : ISerializerInternal
    {
        private const string NULL = "null";
        public Type CustomType { get; }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;
        private ISerializerInternal asInterface => this;

        public InterfaceSerializer(Type customType)
        {
            CustomType = customType;

            if (!SerializerDependencies.SerializerCollection.Serializers.ContainsKey(CustomType))
            {
                SerializerDependencies.SerializerCollection.Serializers.Add(CustomType, this);
            }

        }

        private bool CanDeserialize(IList<INodeData> nodes , out Type type , out Metadata metadata, DeserializationContext context)
        {
            if(nodes[0].Type != NodeType.OBJECT)
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

            if (!CustomType.IsAssignableFrom(typeFromString))
            {
                type = typeFromString;
                return false;
            }

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
            var concreteType = obj.GetType();

            return SerializerDependencies.SerializerCollection.Serializers[concreteType].Serialize(obj, context);
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
            if (!CanDeserialize(list, out instanceType, out metadata, context ))
            {
                return EmptyValue;
            }

            ISerializer concreteSerializer = SerializerDependencies.SerializerCollection.Serializers[instanceType];

            return ((CustomSerializer)concreteSerializer).DeserializeFromNodes(list , context);
        }
    }
}
