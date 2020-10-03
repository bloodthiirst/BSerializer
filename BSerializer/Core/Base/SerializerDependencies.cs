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
                    serializerCollection.Serializers.Add(typeof(bool), new BooleanSerializer());
                    serializerCollection.Serializers.Add(typeof(float), new FloatSerializer());
                    serializerCollection.Serializers.Add(typeof(string), new StringSerializer());
                    serializerCollection.Serializers.Add(typeof(int), new IntSerializer());
                    serializerCollection.Serializers.Add(typeof(double), new DoubleSerializer());
                }

                return serializerCollection;
            }
        }

    }
}
