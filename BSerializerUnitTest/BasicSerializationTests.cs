using BSerializer.Core.Custom;
using NUnit.Framework;
using System.Collections.Generic;

namespace BSerializer.UnitTest.Model
{
    [TestFixture]
    public class BasicSerializationTests
    {

        [Test(Description = "Check Recursion Serialization")]
        public void RecursionTest()
        {
            Person parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;

            string text = SetupClass.Serializer.Serialize(parent);

            Person obj = SetupClass.Serializer.Deserialize(text);

            Assert.AreSame(obj, obj.Parent);
        }

        [Test(Description = "Check Dictionary Serialization")]
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

            string textTest = SetupClass.DictionarySerializer.Serialize(obj);

            Assert.NotNull(obj);
            Assert.AreEqual(text, textTest);
            Assert.AreSame(obj[420], obj[88]);
        }

        [Test(Description = "Check Interface Serialization")]
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

            Assert.NotNull(inter);
            Assert.IsInstanceOf<Person>(inter);
        }

        [Test(Description = "Check List Serialization")]
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

            string textBack = SetupClass.ListSerializer.Serialize(inter);

            Assert.AreEqual(text, textBack);

            Assert.NotNull(inter);

            Assert.NotNull(inter[0]);

            Assert.IsInstanceOf<Person>(inter[0]);
        }
    }
}
