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
using System.Runtime.InteropServices;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class CustomSerializer : ISerializerInternal
    {
        private const string NULL = "null";
        public Type CustomType { get; }
        private IList<ISerializerInternal> Serializers { get; }
        private IList<Action<object,object>> PropertieSetter { get; set; }
        private IList<Func<object, object>> PropertieGetter { get; set; }
        private ISerializerInternal asInterface { get; }
        private int PropertiesCount { get; set; }
        public CustomSerializer(Type customType)
        {
            CustomType = customType;

            Serializers = new List<ISerializerInternal>();
            PropertieSetter = new List<Action<object, object>>();
            PropertieGetter = new List<Func<object, object>>();
            asInterface = this;

            if (! SerializerDependencies.SerializerCollection.Serializers.ContainsKey(CustomType))
            {
                SerializerDependencies.SerializerCollection.Serializers.Add(CustomType, this);
            }

            Initialize();
        }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;

        private List<PropertyInfo> GetSerializableProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.GetAccessors().Length == 2)
                .ToList();
        }

        private void Initialize()
        {
            // get properties
            List<PropertyInfo> props = GetSerializableProperties(CustomType);

            PropertiesCount = props.Count;

            for (int i = 0; i < props.Count; i++)
            {
                PropertyInfo prop = props[i];
                Func<object, object> getter = SerializerUtils.GetterToDelegate(prop.GetMethod);
                Action<object, object> setter = SerializerUtils.SetterToDelegate(prop.SetMethod);
                ISerializerInternal serializer = SerializerDependencies.SerializerCollection.Serializers[prop.PropertyType];

                PropertieGetter.Add(getter);
                PropertieSetter.Add(setter);
                Serializers.Add(serializer);
            }
        }

        private bool CanDeserialize(IList<INodeData> nodes , out Type type)
        {
            if (nodes.Count != 1)
            {
                type = null;
                return false;
            }
            
            if(nodes[0].Type != NodeType.OBJECT)
            {
                type = null;
                return false;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            if (validNodes.Count != PropertiesCount)
            {
                type = null;
                return false;
            }

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            Type typeFromString = Assembly.GetEntryAssembly().GetType(typeNode.SubNodes[1].Data);

            CustomSerializer serializer = new CustomSerializer( typeof(Metadata));

            Metadata metadata = (Metadata) serializer.DeserializeFromNodes(new List<INodeData>() { typeNode });

            if (!CustomType.IsAssignableFrom(typeFromString))
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

            return DeserializeFromNodes(list);

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

        string  ISerializerInternal.Serialize(object obj, SerializationSettings settings)
        {
            if (obj == null)
                return EmptySymbol;

            if (obj.Equals(EmptyValue))
                return EmptySymbol;

            StringBuilder sb = new StringBuilder();

            ObjectNodeParser objectNodeParser = new ObjectNodeParser();

            sb.Append(objectNodeParser.WrappingStart);
            sb.Append('\n');
            settings.TabPadding++;
            sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
            sb.Append("<");
            sb.Append(CustomType.FullName);
            sb.Append(">");
            sb.Append('\n');
            for (int i = 0; i < PropertiesCount - 1; i++)
            {
                object val = PropertieGetter[i].Invoke(obj);

                
                string valAsString = Serializers[i].Serialize(val , settings);
                sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
                sb.Append(valAsString);
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                sb.Append('\n');
            }
            sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
            object lastVal = PropertieGetter[PropertiesCount - 1].Invoke(obj);
            string lastValAsString = Serializers[PropertiesCount - 1].Serialize(lastVal , settings);
            sb.Append(lastValAsString);
            sb.Append('\n');
            settings.TabPadding--;
            sb.Append(SerializerUtils.GetTabSpaces(settings.TabPadding));
            sb.Append(objectNodeParser.WrappingEnd);

            return sb.ToString();
        }
    }
}
