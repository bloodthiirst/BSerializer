namespace BSerializer.BaseTypes
{
    public class FloatSerializer : SerializerBase<float>
    {
        public override float Deserialize(string s)
        {
            return float.Parse(s);
        }

        public override string Serialize(float obj)
        {
            return obj.ToString();
        }

        public override bool TryDeserialize(string s, ref float obj)
        {
            return float.TryParse(s, out obj);
        }

        public override bool TrySerialize(float obj, ref string s)
        {
            s = Serialize(obj);
            return true;
        }
    }
}
