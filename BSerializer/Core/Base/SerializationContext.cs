using System.Collections.Generic;

namespace BSerializer.Core.Base
{
    internal class SerializationContext
    {
        public bool WithPropertiesComments { get; set; } = true;
        public int TabPadding { get; set; }
        private Dictionary<object, int> CachedInstances { get; set; } = new Dictionary<object, int>();

        public bool TryGet(object instance, out int reference)
        {
            int index;
            if (!CachedInstances.TryGetValue(instance, out index))
            {
                reference = -1;
                return false;
            }

            reference = index;
            return true;
        }

        public bool Register(object instance, out int reference)
        {
            if (CachedInstances.ContainsKey(instance))
            {
                reference = -1;
                return false;
            }
            int index = CachedInstances.Count;
            CachedInstances.Add(instance, index);

            reference = index;
            return true;
        }
    }
}
