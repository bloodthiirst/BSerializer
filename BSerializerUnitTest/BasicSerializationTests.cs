using BSerializer.Core.Custom;
using System;
using System.Collections.Generic;
using Xunit;

namespace BSerializer.UnitTest.Model
{
    public class BasicSerializationTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture TestFixture;

        public  BasicSerializationTests(TestFixture TestFixture)
        {
            this.TestFixture = TestFixture;
        }

        [Fact(DisplayName = "Check Recursion Serialization")]
        public void RecursionTest()
        {
            Person parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;

            string text = TestFixture.Serializer.Serialize(parent);

            Person obj = TestFixture.Serializer.Deserialize(text);

            Assert.Equal(obj, obj.Parent);
        }

        [Fact(DisplayName = "Check Dictionary Serialization")]
        public void DictionaryTest()
        {
            Person p = new Person() {
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

            string text = TestFixture.DictionarySerializer.Serialize(dict);

            Dictionary<int, Person> obj = TestFixture.DictionarySerializer.Deserialize(text);

            string textTest = TestFixture.DictionarySerializer.Serialize(obj);

            Assert.NotNull(obj);
            Assert.Equal(text, textTest);
            Assert.Equal(obj[420], obj[88]);
        }

        [Fact(DisplayName = "Check Interface Serialization")]
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

            string text = TestFixture.InterfaceSerializer.Serialize(p);

            IPerson inter = TestFixture.InterfaceSerializer.Deserialize(text);

            Assert.NotNull(inter);
            Assert.IsType<Person>(inter);
        }

        [Fact(DisplayName ="Check List Serialization")]
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

            List<IPerson> people = new List<IPerson>() { p };

            string text = TestFixture.ListSerializer.Serialize(people);

            List<IPerson> inter = TestFixture.ListSerializer.Deserialize(text);

            string textBack = TestFixture.ListSerializer.Serialize(inter);

            Assert.Equal(text, textBack);

            Assert.NotNull(inter);
            
            Assert.NotNull(inter[0]);
            
            Assert.IsType<Person>(inter[0]);
        }
    }
}
