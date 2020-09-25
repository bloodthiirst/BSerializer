using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    public class SerializerCollection : ISerializerCollection
    {
        public IDictionary<Type, ISerializer> Serializers { get; set; }

        public SerializerCollection()
        {
            Serializers = new Dictionary<Type, ISerializer>();
        }
    }
}
