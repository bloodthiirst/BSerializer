using BSerializer.Core.Base;
using BSerializer.Core.Collection;
using System;
using System.Collections;

namespace BSerializer.Core.Custom
{
    public class BSerializer<T> : ISerializer
    {
        public Caching Caching { get; set; }
        private const string NULL = "NULL";
        private static readonly Type GenericType = typeof(T);
        private static string InternalTypeFullName => GenericType.FullName;
        public string TypeFullName => InternalTypeFullName;
        public Type Type => GenericType;

        /// <summary>
        /// string that reprensents the empty value for this type
        /// </summary>
        public string EmptySymbol => NULL;

        /// <summary>
        /// object that reperents the empty value for this type
        /// </summary>
        public object EmptyValue => default(T);

        private ISerializer customSerializer;

        private ISerializer asInterface;

        public BSerializer()
        {
            customSerializer = SerializerFactory.GetSerializerForType(Type);
            asInterface = this;
            
        }

        public T Deserialize(string s)
        {
            return (T) asInterface.Deserialize(s);
        }

        public string Serialize(T obj)
        {
            return asInterface.Serialize(obj);
        }

        object ISerializer.Deserialize(string s)
        {
            return customSerializer.Deserialize(s);
        }

        string ISerializer.Serialize(object obj)
        {
            return customSerializer.Serialize(obj);
        }

        bool ISerializer.TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        bool ISerializer.TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }
    }
}
