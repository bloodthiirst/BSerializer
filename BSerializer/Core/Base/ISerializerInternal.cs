using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Base
{
    internal interface ISerializerInternal : ISerializer
    {
        void SerializeInternal(object obj , SerializationContext context , StringBuilder sb);
        object DeserializeInternal(string data, DeserializationContext context);
    }
}
