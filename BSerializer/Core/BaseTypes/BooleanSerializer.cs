namespace BSerializer.BaseTypes
{
    public class BooleanSerializer : SerializerPrimitiveBase<bool>
    {
        private const string TRUE = "1";
        private const string FALSE = "0";

        public override string EmptySymbol => FALSE;

        public override object EmptyValue => false;

        public override bool Deserialize(string s)
        {
            return s.Equals(TRUE) ? true : false;
        }

        public override string Serialize(bool obj)
        {
            return obj ? TRUE : FALSE;
        }

        public override bool TryDeserialize(string s, ref bool obj)
        {
            if (!obj.Equals(TRUE) && !obj.Equals(FALSE))
            {
                obj = false;
                return false;
            }

            obj = Deserialize(s);
            return true;
        }

        public override bool TrySerialize(bool obj, ref string s)
        {
            s = obj ? TRUE : FALSE;
            return true;
        }
    }
}
