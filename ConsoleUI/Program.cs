using BSerializer.Core.Custom;
using ConsoleUI.Model;
using System;
using System.Collections.Generic;
using System.IO;

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
            BSerializer<Dictionary<int, Person>> dictSerializer = new BSerializer<Dictionary<int, Person>>();

            //string test = 



            var parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;
            var person = new Person() { age = 41, Id = 123, FirstName = "Bloodthirst", LastName = "Ketsueki", Address = "Some place", Parent = parent };

            StreamWriter fileWriter = File.CreateText("D:\\DSerializer - Log.txt");



            string serializedPerson = serializer.Serialize(parent);

            var deserialize = serializer.Deserialize(serializedPerson);

            string serializedParent = serializer.Serialize(person); ;

            string interfaceSerialized = serializerInterface.Serialize(person);
            IPerson interfaceDiserialized = serializerInterface.Deserialize(interfaceSerialized);

            string serialized = serializer.Serialize(person);
            Person deserialized = serializer.Deserialize(serialized);


            List<IPerson> list = new List<IPerson>() { person, parent };

            string serializedList = listSerializer.Serialize(list);

            List<IPerson> deserializedList = listSerializer.Deserialize(serializedList);


            Dictionary<int, Person> dict = new Dictionary<int, Person>()
{
    { 420 , parent },
    { 88 , parent }
};

            string serializedDict = dictSerializer.Serialize(dict);
            string serializedEmptyDict = dictSerializer.Serialize(new Dictionary<int, Person>());
            Dictionary<int, Person> deserializedDic = dictSerializer.Deserialize(serializedDict);

            bool sameRefTest = object.ReferenceEquals(deserializedDic[420], deserializedDic[88]);

            fileWriter.Write("/// List ///");

            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(serializedList);

            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);


            fileWriter.Write("/// Object ///");
            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Write(serializedPerson);

            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Write("/// Dictionary ///");
            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Write(serializedDict);

            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Write("/// Empty Dictionary ///");
            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Write(serializedEmptyDict);

            fileWriter.Write(Environment.NewLine);
            fileWriter.Write(Environment.NewLine);

            fileWriter.Dispose();
        }
    }
}
