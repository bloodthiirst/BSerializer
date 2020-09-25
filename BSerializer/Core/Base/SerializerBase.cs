using System;

namespace BSerializer
{
    public abstract class SerializerBase<T> : ISerializer
    {
        private static readonly Type InternalType = typeof(T);
        public Type Type => InternalType;

        public abstract string EmptySymbol { get; }
        public abstract object EmptyValue { get; }

        object ISerializer.Deserialize(string s)
        {
            if(s.Equals(EmptySymbol))
            {
                return EmptyValue;
            }

            return Deserialize(s);
        }

        string ISerializer.Serialize(object obj)
        {
            if (obj.Equals(EmptyValue))
                return EmptySymbol;

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
