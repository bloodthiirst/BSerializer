using System.Collections.Generic;

namespace BSerializer.Core.Base
{
    internal class DeserializationContext
    {
        private Dictionary<int, object> CachedInstances { get; set; } = new Dictionary<int, object>();

        public bool TryGet(int reference, out object instance)
        {
            return CachedInstances.TryGetValue(reference, out instance);
        }

        public bool Register(int reference, object instance)
        {
            if (CachedInstances.ContainsKey(reference))
            {
                return false;
            }
            CachedInstances.Add(reference, instance);
            return true;
        }
    }
}
