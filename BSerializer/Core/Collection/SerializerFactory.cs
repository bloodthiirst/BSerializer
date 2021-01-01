using BSerializer.Core.Base;
using BSerializer.Core.Custom;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BSerializer.Core.Collection
{
    internal static class SerializerFactory
    {
        private static HashSet<Type> allTypes;
        internal static HashSet<Type> AllTypes
        {
            get
            {
                if(allTypes == null)
                {
                    allTypes = new HashSet<Type>();
                }

                return allTypes;
            }
        }

        internal static ISerializerInternal GetSerializerForType(Type Type)
        {
            ISerializerInternal customSerializer;

            AllTypes.Add(Type);

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
