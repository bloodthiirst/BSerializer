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
        public string TypeFullName => null;
        public InterfaceSerializer(Type customType)
        {
            CustomType = customType;

            SerializerDependencies.SerializerCollection.GetOrAdd(CustomType, this);

        }

        private bool CanDeserialize(IList<INodeData> nodes, out Type type, out Metadata metadata, DeserializationContext context)
        {
            if (nodes[0].Type != NodeType.OBJECT)
            {
                type = null;
                metadata = null;
                return false;
            }

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            CustomSerializer serializer = new CustomSerializer(typeof(Metadata));

            metadata = (Metadata)serializer.ReadObjectData(new List<INodeData>() { typeNode }, context , -1);

            if (!CustomType.IsAssignableFrom(metadata.Type))
            {
                type = metadata.Type;
                return false;
            }

            type = metadata.Type;
            return true;

        }

        public object Deserialize(string s)
        {
            return asInterface.DeserializeInternal(s, new DeserializationContext());

        }

        public string Serialize(object obj)
        {
            var sb = new StringBuilder();
            asInterface.SerializeInternal(obj, new SerializationContext() , sb);
            return sb.ToString();
        }

        public bool TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        public bool TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }

        void ISerializerInternal.SerializeInternal(object obj, SerializationContext context , StringBuilder sb)
        {
            var concreteType = obj.GetType();

            SerializerDependencies.SerializerCollection.GetOrAdd(concreteType).SerializeInternal(obj, context , sb);
        }

        object ISerializerInternal.DeserializeInternal(string data, DeserializationContext context)
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

            if (!CanDeserialize(list, out instanceType, out metadata, context))
            {
                return EmptyValue;
            }

            if(context.TryGet(metadata.ReferenceTracker , out object cacheFound))
            {
                return cacheFound;
            }

            ISerializer concreteSerializer = SerializerDependencies.SerializerCollection.GetOrAdd(instanceType);

            return ((CustomSerializer)concreteSerializer).ReadObjectData(list, context , metadata.ReferenceTracker);
        }
    }
}
