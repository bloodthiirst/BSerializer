using BSerializer.Core.Base;
using System;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal interface ISerializerCollection
    {
        ISerializerInternal GetOrAdd(Type type);

        ISerializerInternal GetOrAdd(Type type, ISerializerInternal toAdd);
    }
}
