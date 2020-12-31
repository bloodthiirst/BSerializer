using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BSerializer.Core.Nodes
{
    public class Metadata
    {
        public string TypeFullName { get; set; }
        public int ReferenceTracker { get; set; }

        private Type type { get; set; }
        public Type Type
        {
            get
            {
                if(type == null)
                {
                    type = Assembly.GetEntryAssembly().GetType(TypeFullName);
                }
                return type;
            }
        }
    }
}
