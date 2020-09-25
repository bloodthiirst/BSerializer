using BSerializer.Core.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class CustomSerializer : ISerializer
    {
        public Type CustomType { get; }
        private ISerializerCollection SerializerCollection { get; }
        private IList<Func<object,string>> PropertieSerializer { get; set; }
        private IList<Func<string, object>> PropertieDeserializer { get; set; }
        private IList<Action<object,object>> PropertieSetter { get; set; }
        private IList<Func<object, object>> PropertieGetter { get; set; }
        public CustomSerializer(Type customType , ISerializerCollection serializerCollection)
        {
            CustomType = customType;
            SerializerCollection = serializerCollection;

            PropertieSerializer = new List<Func<object, string>>();
            PropertieDeserializer = new List<Func<string, object>>();
            PropertieSetter = new List<Action<object, object>>();
            PropertieGetter = new List<Func<object, object>>();

            Initialize();
        }

        public Type Type => CustomType;

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


            foreach (PropertyInfo prop in props)
            {
                Func<object, object> getter = SerializerUtils.GetterToDelegate(prop.GetMethod);
                Action<object, object> setter = SerializerUtils.SetterToDelegate(prop.SetMethod);

                PropertieGetter.Add(getter);
                PropertieSetter.Add(setter);

            }
        }

        public object Deserialize(string s)
        {
            throw new NotImplementedException();
        }

        public string Serialize(object obj)
        {
            throw new NotImplementedException();
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
