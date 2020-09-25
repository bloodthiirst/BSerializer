using System;

namespace BSerializer
{
    public interface ISerializer
    {
        Type Type { get; }
        object Deserialize(string s);
        string Serialize(object obj);
        bool TryDeserialize(string s, ref object obj);
        bool TrySerialize(object obj, ref string s);
    }
}
