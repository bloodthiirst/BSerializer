namespace BSerializer.BaseTypes
{
    public class StringSerializer : SerializerPrimitiveBase<string>
    {
        private const string EMPTY = "''";

        public override string EmptySymbol => EMPTY;

        public override object EmptyValue => string.Empty;

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
