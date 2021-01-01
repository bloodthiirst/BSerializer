using BSerializer.Core.Custom;
using System;
using System.Collections.Generic;
using Xunit;

namespace BSerializer.UnitTest.Model
{
    public class UnitTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture TestFixture;

        public  UnitTest(TestFixture TestFixture)
        {
            this.TestFixture = TestFixture;
        }

        [Fact]
        public void RecursionTest()
        {
            Person parent = new Person() { age = 32, Address = "Some other place", FirstName = "Parent", LastName = "McParenton", Id = 69 };
            parent.Parent = parent;

            string text = TestFixture.Serializer.Serialize(parent);

            Person obj = TestFixture.Serializer.Deserialize(text);

            Assert.Equal(obj, obj.Parent);
        }

        [Fact]
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

            string text = TestFixture.DictSerializer.Serialize(dict);

            Dictionary<int, Person> obj = TestFixture.DictSerializer.Deserialize(text);

            string textTest = TestFixture.DictSerializer.Serialize(obj);

            Assert.Equal(text, textTest);
        }

        [Fact]
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

            string text = TestFixture.SerializerInterface.Serialize(p);

            IPerson inter = TestFixture.SerializerInterface.Deserialize(text);

            Assert.NotNull(inter);
            Assert.IsType<Person>(inter);
        }
    }
}
