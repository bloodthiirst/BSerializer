using BSerializer.BaseTypes;
using BSerializer.Core.Collection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Base
{
    internal static class SerializerDependencies
    {
        private static ISerializerCollection serializerCollection;

        internal static ISerializerCollection SerializerCollection
        {
            get
            {
                if(serializerCollection == null)
                {
                    serializerCollection = new SerializerCollection();
                    SerializerCollection.GetOrAdd(typeof(bool), new BooleanSerializer());
                    SerializerCollection.GetOrAdd(typeof(float), new FloatSerializer());
                    SerializerCollection.GetOrAdd(typeof(string), new StringSerializer());
                    SerializerCollection.GetOrAdd(typeof(int), new IntSerializer());
                    SerializerCollection.GetOrAdd(typeof(double), new DoubleSerializer());
                }

                return serializerCollection;
            }
        }

    }
}
