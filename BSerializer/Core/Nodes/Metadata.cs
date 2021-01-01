using BSerializer.Core.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    type = SerializerFactory.AllTypes.FirstOrDefault( t => t.FullName.Equals(TypeFullName));
                }
                return type;
            }
        }
    }
}
