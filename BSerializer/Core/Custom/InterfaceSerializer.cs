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
    public class InterfaceSerializer : ISerializer
    {
        private const string NULL = "null";

        public Type CustomType { get; }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;

        public InterfaceSerializer(Type customType)
        {
            CustomType = customType;

            if (!SerializerDependencies.SerializerCollection.Serializers.ContainsKey(CustomType))
            {
                SerializerDependencies.SerializerCollection.Serializers.Add(CustomType, this);
            }

        }

        private bool CanDeserialize(IList<INodeData> nodes , out Type type)
        {
            if(nodes[0].Type != NodeType.OBJECT)
            {
                type = null;
                return false;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            Type typeFromString = Assembly.GetEntryAssembly().GetType(typeNode.SubNodes[1].Data);

            CustomSerializer serializer = new CustomSerializer(typeof(Metadata));

            Metadata metadata = (Metadata)serializer.DeserializeFromNodes(new List<INodeData>() { typeNode });

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

            ISerializer concreteSerializer = SerializerDependencies.SerializerCollection.Serializers[instanceType];

            return ((CustomSerializer)concreteSerializer).DeserializeFromNodes(list);

        }

        public string Serialize(object obj)
        {
            var concreteType = obj.GetType();

            return SerializerDependencies.SerializerCollection.Serializers[concreteType].Serialize(obj);
        }

        public bool TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        public bool TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }
    }
}
