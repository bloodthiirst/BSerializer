using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Parser
{
    public interface INodeParser
    {
        bool IsValid(string data , out IList<INodeData> nodeData , int position);
    }
}
