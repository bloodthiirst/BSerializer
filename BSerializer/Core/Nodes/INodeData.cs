using System.Collections.Generic;

namespace BSerializer.Core.Parser.SerializationNodes
{
    public interface INodeData
    {
        NodeType Type { get; }
        string Data { get; }
        List<INodeData> SubNodes { get; set; }
        int Position { get; set; }
    }
}
