﻿using BSerializer.Core.Base;
using BSerializer.Core.Collection;
using System;
using System.Collections;

namespace BSerializer.Core.Custom
{
    public class BSerializer<T> : ISerializer
    {
        public Caching Caching { get; set; }
        private const string NULL = "NULL";
        private static readonly Type GenericType = typeof(T);
        public Type Type => GenericType;

        public string EmptySymbol => NULL;

        public object EmptyValue => default(T);

        private ISerializer customSerializer;

        private ISerializer asInterface;

        public BSerializer()
        {
            customSerializer = SerializerFactory.GetSerializerForType(Type);
            asInterface = this;
            
        }

        public T Deserialize(string s)
        {
            return (T) asInterface.Deserialize(s);
        }

        public string Serialize(T obj)
        {
            return asInterface.Serialize(obj);
        }

        object ISerializer.Deserialize(string s)
        {
            return customSerializer.Deserialize(s);
        }

        string ISerializer.Serialize(object obj)
        {
            return customSerializer.Serialize(obj);
        }

        bool ISerializer.TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        bool ISerializer.TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }
    }
}
