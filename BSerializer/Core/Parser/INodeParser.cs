using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Parser
{
    public interface INodeParser
    {
        string WrappingStart { get; }
        string WrappingEnd { get; }
        bool HasWrapping { get; }
        bool NeedsSeparation { get; }
        bool IsValid(string data , out IList<INodeData> nodeData , int position , out string patchedData);
    }
}
