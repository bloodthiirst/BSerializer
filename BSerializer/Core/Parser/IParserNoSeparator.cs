using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Parser
{
    public interface IParserNoSeparator
    {
        bool Validate(string data);
    }
}
