using BSerializer.Core.Custom;
using System.Collections.Generic;

using NUnit.Framework;

namespace BSerializer.UnitTest.Model
{
    [SetUpFixture]
    public class SetupClass
    {
        public static BSerializer<Person> Serializer { get; set; }
        public static BSerializer<IPerson> InterfaceSerializer { get; set; }
        public static BSerializer<List<IPerson>> ListSerializer { get; set; }
        public static BSerializer<Dictionary<int, Person>> DictionarySerializer { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            Serializer = new BSerializer<Person>();
            InterfaceSerializer = new BSerializer<IPerson>();
            ListSerializer = new BSerializer<List<IPerson>>();
            DictionarySerializer = new BSerializer<Dictionary<int, Person>>();
        }
    }
}
