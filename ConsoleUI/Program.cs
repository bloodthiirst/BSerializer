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
            CustomSerializer customSerializer = new CustomSerializer(typeof(Person));

            BSerializer<Person> serializer = new BSerializer<Person>();
            BSerializer<IPerson> serializerInterface = new BSerializer<IPerson>();
            BSerializer<List<IPerson>> listSerializer = new BSerializer<List<IPerson>>();

            var person = new Person() { Id = 123, FirstName = "Bloodthirst", LastName = "Ketsueki", Address = "Some place", Parent = null };

            /*
            string interfaceSerialized = serializerInterface.Serialize(person);
            IPerson interfaceDiserialized = serializerInterface.Deserialize(interfaceSerialized);

            string serialized = serializer.Serialize(person);
            Person deserialized = serializer.Deserialize(serialized);
            */
            List<IPerson> list = new List<IPerson>() { person, person };

            string serializedList = listSerializer.Serialize(list);

            List<IPerson> deserializedList = listSerializer.Deserialize(serializedList);

        }
    }
}
