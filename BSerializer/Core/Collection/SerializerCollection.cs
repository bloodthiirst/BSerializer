using BSerializer.Core.Base;
using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal class SerializerCollection : ISerializerCollection
    {
        public IDictionary<Type, ISerializerInternal> Serializers { get; set; }

        internal  SerializerCollection()
        {
            Serializers = new Dictionary<Type, ISerializerInternal>();
        }
    }
}
