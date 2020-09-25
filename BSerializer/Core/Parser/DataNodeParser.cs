using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Parser
{
    public class DataNodeParser : INodeParser
    {
        private static readonly NodeType serializationNodeType = NodeType.DATA;
        public NodeType NodeType => serializationNodeType;

        public bool IsValid(string data, out IList<INodeData> nodeDatas, int position)
        {
            nodeDatas = new List<INodeData>(1);

            NodeDataBase nodeData = new NodeDataBase(NodeType,data,position );

            nodeDatas.Add(nodeData);

            return true;
        }
    }
}
