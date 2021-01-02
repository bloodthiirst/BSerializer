using BSerializer.Core.Custom;
using System.Collections.Generic;

using NUnit.Framework;

namespace BSerializer.UnitTest.Model
{
    [TestFixture]
    public class BasicSerializationPerformanceTests
    {

        [Test(Description = "Check BSerializer performance : Recursion Serialization")]
        public void RecursionTest()
        {
            Person parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;

            string text = SetupClass.Serializer.Serialize(parent);

            Person obj = SetupClass.Serializer.Deserialize(text);

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

            string text = SetupClass.DictionarySerializer.Serialize(dict);

            Dictionary<int, Person> obj = SetupClass.DictionarySerializer.Deserialize(text);
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

            string text = SetupClass.InterfaceSerializer.Serialize(p);

            IPerson inter = SetupClass.InterfaceSerializer.Deserialize(text);
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

            string text = SetupClass.ListSerializer.Serialize(people);

            List<IPerson> inter = SetupClass.ListSerializer.Deserialize(text);
        }
    }
}
