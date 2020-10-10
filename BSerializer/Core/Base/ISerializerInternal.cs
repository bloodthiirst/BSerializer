using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Base
{
    internal interface ISerializerInternal : ISerializer
    {
        string Serialize(object obj , SerializationContext context);
        object Deserialize(string data, DeserializationContext context);
    }
}
