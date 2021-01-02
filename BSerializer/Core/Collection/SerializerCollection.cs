using BSerializer.Core.Base;
using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal class SerializerCollection : ISerializerCollection
    {
        internal IDictionary<Type, ISerializerInternal> Serializers { get; set; }

        internal SerializerCollection()
        {
            Serializers = new Dictionary<Type, ISerializerInternal>();
        }

        public ISerializerInternal GetOrAdd(Type type)
        {
            if (Serializers.TryGetValue(type, out var ser))
            {
                return ser;
            }
            ser = SerializerFactory.GetSerializerForType(type);
            Serializers.Add(type, ser);

            return ser;
        }

        public ISerializerInternal GetOrAdd(Type type, ISerializerInternal toAdd)
        {
            if (Serializers.TryGetValue(type, out var ser) && ser != null)
            {
                return ser;
            }

            Serializers[type] =  toAdd;

            return toAdd;
        }
    }
}
