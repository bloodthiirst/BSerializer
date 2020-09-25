using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Parser.SerializationNodes
{
    public class NodeDataBase : INodeData
    {
        public NodeType Type { get; }
        public string Data { get;}
        public List<INodeData> SubNodes { get; set; }
        public int Position { get; set; }

        public NodeDataBase(NodeType type, string data , int position)
        {
            Type = type;
            Data = data;
            SubNodes = new List<INodeData>();
            Position = position;
        }

    }
}
