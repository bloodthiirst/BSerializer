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
                    SerializerCollection ser = new SerializerCollection();
                    ser.Serializers.Add(typeof(bool) , new BooleanSerializer());
                    ser.Serializers.Add(typeof(float) , new FloatSerializer());
                    ser.Serializers.Add(typeof(string) , new StringSerializer());
                    ser.Serializers.Add(typeof(int) ,  new IntSerializer());
                    ser.Serializers.Add(typeof(double),  new DoubleSerializer());

                    serializerCollection = ser;
                }

                return serializerCollection;
            }
        }

    }
}
