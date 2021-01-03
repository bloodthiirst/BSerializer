using BSerializer.Core.Base;
using System;
using System.Text;

namespace BSerializer
{
    public abstract class SerializerPrimitiveBase<T> : ISerializerInternal
    {
        private static readonly Type InternalType = typeof(T);
        private static readonly string InternalTypeFullName = InternalType.FullName;
        public Type Type => InternalType;
        public string TypeFullName => InternalTypeFullName;
        private ISerializerInternal asInterface => this;
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

        void ISerializerInternal.SerializeInternal(object obj, SerializationContext settings , StringBuilder sb)
        {    
            sb.Append(((ISerializer)this).Serialize(obj));
        }

        object ISerializerInternal.DeserializeInternal(string data, DeserializationContext context)
        {
            return asInterface.Deserialize(data);
        }
    }
}
