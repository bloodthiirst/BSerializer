using System;

namespace BSerializer
{
    public abstract class SerializerBase<T> : ISerializer
    {
        private static readonly Type InternalType = typeof(T);
        public Type Type => InternalType;

        object ISerializer.Deserialize(string s)
        {
            return Deserialize(s);
        }

        string ISerializer.Serialize(object obj)
        {
            return Serialize((T)obj);
        }
        bool ISerializer.TrySerialize(object obj, ref string s)
        {
            return TrySerialize((T)obj, ref s);
        }

        bool ISerializer.TryDeserialize(string s, ref object obj)
        {
            T cast = (T)obj;
            return TryDeserialize(s, ref cast);
        }
        public abstract T Deserialize(string s);

        public abstract string Serialize(T obj);

        public abstract bool TryDeserialize(string s, ref T obj);

        public abstract bool TrySerialize(T obj, ref string s);

    }
}
