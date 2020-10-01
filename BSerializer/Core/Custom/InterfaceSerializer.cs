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
        private ISerializerCollection SerializerCollection { get; }
        private IList<ISerializer> Serializers { get; }
        private IList<Action<object,object>> PropertieSetter { get; set; }
        private IList<Func<object, object>> PropertieGetter { get; set; }
        private int PropertiesCount { get; set; }
        public InterfaceSerializer(Type customType , ISerializerCollection serializerCollection)
        {
            CustomType = customType;
            SerializerCollection = serializerCollection;
        }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;

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

            if(!CustomType.IsAssignableFrom(typeFromString))
            {
                type = typeFromString;
                return false;
            }

            type = typeFromString;
            return true;

        }

        internal object DeserializeFromNodes(IList<INodeData> list)
        {
            object instance = Activator.CreateInstance(CustomType);

            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            int propIndex = 0;

            for (int i = 0; i < PropertiesCount; i++)
            {
                INodeData node = list[i];

                if (node.Type == Parser.NodeType.COMMENT)
                    continue;
                if (node.Type == Parser.NodeType.SYMBOL)
                    continue;

                object val = Serializers[propIndex].Deserialize(node.Data);

                PropertieSetter[i].Invoke(instance, val);

                propIndex++;
            }

            return instance;
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

            ISerializer concreteSerializer = SerializerCollection.Serializers[instanceType];

            return ((CustomSerializer)concreteSerializer).DeserializeFromNodes(list);

        }

        public string Serialize(object obj)
        {
            var concreteType = obj.GetType();

            return SerializerCollection.Serializers[concreteType].Serialize(obj);
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
