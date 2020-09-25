using BSerializer.Core.Collection;
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
    public class CustomSerializer : ISerializer
    {
        private const string NULL = "null";

        public Type CustomType { get; }
        private ISerializerCollection SerializerCollection { get; }
        private IList<ISerializer> Serializers { get; }
        private IList<Action<object,object>> PropertieSetter { get; set; }
        private IList<Func<object, object>> PropertieGetter { get; set; }
        private int PropertiesCount { get; set; }
        public CustomSerializer(Type customType , ISerializerCollection serializerCollection)
        {
            CustomType = customType;
            SerializerCollection = serializerCollection;

            Serializers = new List<ISerializer>();
            PropertieSetter = new List<Action<object, object>>();
            PropertieGetter = new List<Func<object, object>>();

            if(!SerializerCollection.Serializers.ContainsKey(CustomType))
            {
                SerializerCollection.Serializers.Add(CustomType, this);
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
                ISerializer serializer = SerializerCollection.Serializers[prop.PropertyType];

                PropertieGetter.Add(getter);
                PropertieSetter.Add(setter);
                Serializers.Add(serializer);
            }
        }


        public object Deserialize(string s)
        {
            if (s.Equals(EmptySymbol))
            {
                return EmptyValue;
            }

            object instance = Activator.CreateInstance(CustomType);

            MainParser parser = new MainParser();
            IList<INodeData> list;
            parser.ExtractNodeData(s, out list);

            list = list[0].SubNodes[1].SubNodes;

            if(list.Count != PropertiesCount)
            {
                return EmptyValue;
            }

            int propIndex = 0;

            for(int i = 0; i < PropertiesCount; i++)
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

        public string Serialize(object obj)
        {
            if (obj == null)
                return EmptySymbol;

            if (obj.Equals(EmptyValue))
                return EmptySymbol;

            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            for(int i = 0; i < PropertiesCount - 1;i++)
            {
                object val = PropertieGetter[i].Invoke(obj);
                string valAsString = Serializers[i].Serialize(val);
                sb.Append(valAsString);
                sb.Append(',');
            }
            object lastVal = PropertieGetter[PropertiesCount - 1].Invoke(obj);
            string lastValAsString = Serializers[PropertiesCount - 1].Serialize(lastVal);
            sb.Append(lastValAsString);
            sb.Append('}');

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
    }
}
