using BSerializer.Core.Custom;
using System.Collections.Generic;

using NUnit.Framework;

namespace BSerializer.UnitTest.Model
{
    [TestFixture]
    public class BasicSerializationPerformanceTests
    {
        public BSerializer<Person> Serializer { get; set; }
        public BSerializer<IPerson> InterfaceSerializer { get; set; }
        public BSerializer<List<IPerson>> ListSerializer { get; set; }
        public BSerializer<Dictionary<int, Person>> DictionarySerializer { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Serializer = new BSerializer<Person>();
            InterfaceSerializer = new BSerializer<IPerson>();
            ListSerializer = new BSerializer<List<IPerson>>();
            DictionarySerializer = new BSerializer<Dictionary<int, Person>>();
        }

        [Test(Description = "Check BSerializer performance : Recursion Serialization")]
        public void RecursionTest()
        {
            Person parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;

            string text = Serializer.Serialize(parent);

            Person obj = Serializer.Deserialize(text);

            Assert.AreEqual(obj, obj.Parent);
        }

        [Test(Description = "Check BSerializer performance : Dictionary Serialization")]
        public void DictionaryTest()
        {
            Person p = new Person()
            {
                Id = 69,
                age = 32,
                Address = "Some other place",
                FirstName = "Parent",
                LastName = "McParenton"
            };

            Dictionary<int, Person> dict = new Dictionary<int, Person>()
            {
                { 420 , p },
                { 88 , p }
            };

            string text = DictionarySerializer.Serialize(dict);

            Dictionary<int, Person> obj = DictionarySerializer.Deserialize(text);
        }

        [Test(Description = "Check BSerializer performance : Interface Serialization")]
        public void InterfaceTest()
        {
            Person p = new Person()
            {
                Id = 69,
                age = 32,
                Address = "Some other place",
                FirstName = "Parent",
                LastName = "McParenton"
            };

            string text = InterfaceSerializer.Serialize(p);

            IPerson inter = InterfaceSerializer.Deserialize(text);
        }

        [Test(Description = "Check BSerializer performance : List  Serialization")]
        public void ListTest()
        {
            Person p = new Person()
            {
                Id = 69,
                age = 32,
                Address = "Some other place",
                FirstName = "Parent",
                LastName = "McParenton"
            };

            List<IPerson> people = new List<IPerson>() { p , p , p , p , p };

            string text = ListSerializer.Serialize(people);

            List<IPerson> inter = ListSerializer.Deserialize(text);
        }
    }
}
