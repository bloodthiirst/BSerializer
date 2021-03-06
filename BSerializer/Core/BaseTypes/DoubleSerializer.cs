﻿namespace BSerializer.BaseTypes
{
    public class DoubleSerializer : SerializerPrimitiveBase<double>
    {
        private const string EMPTY = "0";

        public override string EmptySymbol => EMPTY;

        public override object EmptyValue => 0;

        public override double Deserialize(string s)
        {
            return double.Parse(s);
        }

        public override string Serialize(double obj)
        {
            return obj.ToString();
        }

        public override bool TryDeserialize(string s, ref double obj)
        {
            return double.TryParse(s, out obj);
        }

        public override bool TrySerialize(double obj, ref string s)
        {
            s = Serialize(obj);
            return true;
        }
    }
}
