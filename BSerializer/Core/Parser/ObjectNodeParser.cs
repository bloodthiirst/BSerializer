using BSerializer.Core.Parser.SerializationNodes;
using System.Collections.Generic;
using System.IO;

namespace BSerializer.Core.Parser
{
    public class ObjectNodeParser : INodeParser
    {
        private const string START_SYMBOL = "{";
        private const string END_SYMBOL = "}";
        private readonly int START_LENGTH = START_SYMBOL.Length;
        private readonly int END_LENGTH = END_SYMBOL.Length;

        private static readonly NodeType serializationNodeType = NodeType.OBJECT;
        public NodeType NodeType => serializationNodeType;
        public string WrappingStart => START_SYMBOL;

        public string WrappingEnd => END_SYMBOL;

        public bool HasWrapping => true;

        public bool NeedsSeparation => true;

        public bool IsValid(string data, out IList<INodeData> nodeDatas, int position, out string patched)
        {
            if(string.IsNullOrEmpty(data))
            {
                patched = data;
                nodeDatas = null;
                return false;
            }
            if(data.Length < (START_LENGTH + END_LENGTH))
            {
                patched = data;
                nodeDatas = null;
                return false;
            }
            if (!data.StartsWith(START_SYMBOL))
            {
                patched = data;
                nodeDatas = null;
                return false;
            }
            if (!data.EndsWith(END_SYMBOL))
            {
                patched = data;
                nodeDatas = null;
                return false;
            }


            var content = data.Substring(START_LENGTH, data.Length - (START_LENGTH + END_LENGTH));

            NodeDataBase obj = new NodeDataBase(NodeType, data, position);

            nodeDatas = new List<INodeData>(1);

            INodeData symbolStart = new NodeDataBase(NodeType.SYMBOL, START_SYMBOL, position);
            INodeData nodeData = new NodeDataBase(NodeType.OBJECT_CONTENT, content, position);
            INodeData symbolEnd = new NodeDataBase(NodeType.SYMBOL, END_SYMBOL, position);


            obj.SubNodes.Add(symbolStart);
            obj.SubNodes.Add(nodeData);
            obj.SubNodes.Add(symbolEnd);

            nodeDatas.Add(obj);
            patched = string.Empty;
            return true;
        }
    }
}
