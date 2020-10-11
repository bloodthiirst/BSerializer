using BSerializer.Core.Base;
using BSerializer.Core.Custom;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal static class SerializerFactory
    {
        internal static ISerializerInternal GetSerializerForType(Type Type)
        {
            ISerializerInternal customSerializer;

            if (typeof(IList).IsAssignableFrom(Type))
            {
                customSerializer = new ListSerializer(Type);
            }

            else if (typeof(IDictionary).IsAssignableFrom(Type))
            {
                customSerializer = new DictionarySerializer(Type);
            }

            else if (Type.IsInterface)
            {
                customSerializer = new InterfaceSerializer(Type);
            }

            else
            {
                customSerializer = new CustomSerializer(Type);
            }

            return customSerializer;
        }
    }
}
