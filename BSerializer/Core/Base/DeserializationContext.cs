using System.Collections.Generic;

namespace BSerializer.Core.Base
{
    public enum Caching
    {
        None, Enabled
    }
    internal class DeserializationContext
    {
        public Caching Caching { get; set; }
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
