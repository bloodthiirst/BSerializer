using BSerializer.Core.Base;
using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal interface ISerializerCollection
    {
        IDictionary<Type,ISerializerInternal> Serializers { get; }
    }
}
