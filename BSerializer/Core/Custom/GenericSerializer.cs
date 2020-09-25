using BSerializer.Core.Collection;
using System;

namespace BSerializer.Core.Custom
{
    public class GenericSerializer<T> : ISerializer
    {
        private const string NULL = "NULL";
        private static readonly Type GenericType = typeof(T);
        public Type Type => GenericType;

        public string EmptySymbol => NULL;

        public object EmptyValue => default(T);

        private CustomSerializer customSerializer;

        private ISerializer asInterface;

        public GenericSerializer(ISerializerCollection serializerCollection)
        {
            customSerializer = new CustomSerializer(Type, serializerCollection);
            asInterface = (ISerializer)this;
        }

        public T Deserialize(string s)
        {
            return (T) asInterface.Deserialize(s);
        }

        public string Serialize<T>(T obj)
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
