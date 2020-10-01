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

        public string WrappingStart => null;

        public string WrappingEnd => null;

        public bool HasWrapping => false;

        public bool NeedsSeparation => true;

        public bool IsValid(string data, out IList<INodeData> nodeDatas, int position,out string patched)
        {
            nodeDatas = new List<INodeData>(1);

            NodeDataBase nodeData = new NodeDataBase(NodeType,data,position );

            nodeDatas.Add(nodeData);
            patched = string.Empty;
            return true;
        }
    }
}
