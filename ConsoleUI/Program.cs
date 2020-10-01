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

            string data = "{ " +
                "<Person>"+
                "# this is a comment #"+
                "# this is a comment #" +
                "1 , " +
                "Houssem , " +
                "ASSAL , " +
                "null " +
                "}";

            GenericSerializer<Person> serializer = new GenericSerializer<Person>(collection);
            GenericSerializer<IPerson> serializerInterface = new GenericSerializer<IPerson>(collection);

            var person = new Person() { Id = 123, FirstName = "Bloodthirst",LastName ="Ketsueki" , Address = "Some place", Parent = null };

            string interfaceSerialized = serializerInterface.Serialize(person);
            IPerson interfaceDiserialized = serializerInterface.Deserialize(interfaceSerialized);

            string serialized = serializer.Serialize(person);
            Person deserialized = serializer.Deserialize(serialized);

        }
    }
}
