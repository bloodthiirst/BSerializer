namespace BSerializer.BaseTypes
{
    public class IntSerializer : SerializerPrimitiveBase<int>
    {
        private const string EMPTy = "0";

        public override string EmptySymbol => EMPTy;

        public override object EmptyValue => 0;

        public override int Deserialize(string s)
        {
            return int.Parse(s);
        }

        public override string Serialize(int obj)
        {
            return obj.ToString();
        }

        public override bool TryDeserialize(string s, ref int obj)
        {
            return int.TryParse(s, out obj);
        }

        public override bool TrySerialize(int obj, ref string s)
        {
            s = Serialize(obj);
            return true;
        }
    }
}
