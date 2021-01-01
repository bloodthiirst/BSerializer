using BSerializer.Core.Custom;
using System;
using System.Collections.Generic;

namespace BSerializer.UnitTest.Model
{
    public class TestFixture : IDisposable
    {
        public BSerializer<Person> Serializer { get; }
        public BSerializer<IPerson> SerializerInterface { get; }
        public BSerializer<List<IPerson>> ListSerialize { get; }
        public BSerializer<Dictionary<int, Person>> DictSerializer { get; }
        public TestFixture()
        {
            Serializer = new BSerializer<Person>();
            SerializerInterface = new BSerializer<IPerson>();
            ListSerialize = new BSerializer<List<IPerson>>();
            DictSerializer = new BSerializer<Dictionary<int, Person>>();
        }
        public void Dispose()
        {

        }
    }
}