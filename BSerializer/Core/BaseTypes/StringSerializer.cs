namespace BSerializer.BaseTypes
{
    public class StringSerializer : SerializerBase<string>
    {
        public override string Deserialize(string s)
        {
            return s;
        }

        public override string Serialize(string obj)
        {
            return obj;
        }

        public override bool TryDeserialize(string s, ref string obj)
        {
            obj = s;
            return true;
        }

        public override bool TrySerialize(string obj, ref string s)
        {
            s = obj;
            return true;
        }
    }
}
