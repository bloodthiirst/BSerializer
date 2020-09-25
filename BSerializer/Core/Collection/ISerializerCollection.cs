using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    public interface ISerializerCollection
    {
        IDictionary<Type,ISerializer> Serializers { get; }
    }
}
