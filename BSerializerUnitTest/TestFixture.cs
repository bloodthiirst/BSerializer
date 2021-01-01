using BSerializer.Core.Custom;
using System;
using System.Collections.Generic;

namespace BSerializer.UnitTest.Model
{
    public class TestFixture : IDisposable
    {
        public BSerializer<Person> Serializer { get; }
        public BSerializer<IPerson> InterfaceSerializer { get; }
        public BSerializer<List<IPerson>> ListSerializer { get; }
        public BSerializer<Dictionary<int, Person>> DictionarySerializer { get; }
        public TestFixture()
        {
            Serializer = new BSerializer<Person>();
            InterfaceSerializer = new BSerializer<IPerson>();
            ListSerializer = new BSerializer<List<IPerson>>();
            DictionarySerializer = new BSerializer<Dictionary<int, Person>>();
        }
        public void Dispose()
        {

        }
    }
}