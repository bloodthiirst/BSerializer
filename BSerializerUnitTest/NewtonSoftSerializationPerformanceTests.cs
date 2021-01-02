using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;

namespace BSerializer.UnitTest.Model
{
    [TestFixture]
    public class NewtonSoftSerializationPerformanceTests
    {
        [Test(Description = "Check Json preformance : List Serialization")]
        public void ListTestJson()
        {
            Person p = new Person()
            {
                Id = 69,
                age = 32,
                Address = "Some other place",
                FirstName = "Parent",
                LastName = "McParenton"
            };

            List<IPerson> people = new List<IPerson>() { p, p, p, p, p };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            string text = JsonConvert.SerializeObject(people, settings);

            List<IPerson> obj = JsonConvert.DeserializeObject<List<IPerson>>(text, settings);
        }

        [Test(Description = "Check Json preformance : Dictionary Serialization")]
        public void DictionaryTestJson()
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

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            string text = JsonConvert.SerializeObject(dict, settings);

            Dictionary<int, Person> obj = JsonConvert.DeserializeObject<Dictionary<int, Person>>(text, settings);
        }

        [Test(Description = "Check Json preformance : Interface Serialization")]
        public void InterfaceTestJson()
        {
            Person p = new Person()
            {
                Id = 69,
                age = 32,
                Address = "Some other place",
                FirstName = "Parent",
                LastName = "McParenton"
            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            string text = JsonConvert.SerializeObject(p, settings);

            IPerson obj = JsonConvert.DeserializeObject<IPerson>(text, settings);
        }
    }
}
