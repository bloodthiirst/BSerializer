using BSerializer.BaseTypes;
using BSerializer.Core.Collection;
using BSerializer.Core.Custom;
using BSerializer.Core.Parser.SerializationNodes;
using ConsoleUI.Model;
using Library.Extractors;
using System.Collections.Generic;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializerCollection collection = new SerializerCollection();
            collection.Serializers.Add(typeof(bool), new BooleanSerializer());
            collection.Serializers.Add(typeof(float), new FloatSerializer());
            collection.Serializers.Add(typeof(string), new StringSerializer());
            collection.Serializers.Add(typeof(int), new IntSerializer());
            collection.Serializers.Add(typeof(double), new DoubleSerializer());

            CustomSerializer customSerializer = new CustomSerializer(typeof(Person), collection);

            string data = "{ 1 , Houssem , ASSAL , null }";

            GenericSerializer<Person> serializer = new GenericSerializer<Person>(collection);


            var person = serializer.Deserialize(data);
            var text = serializer.Serialize(person);
        }
    }
}
