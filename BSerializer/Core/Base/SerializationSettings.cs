using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Base
{
    internal class SerializationSettings
    {
        public bool WithPropertiesComments { get; set; } = true;
        public int TabPadding { get; set; }
    }
}
