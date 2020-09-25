using BSerializer.Core.Parser.SerializationNodes;
using System.Collections.Generic;

namespace BSerializer.Core.Parser
{
    public class ObjectNodeParser : INodeParser
    {
        private const string START_SYMBOL = "{";
        private const string END_SYMBOL = "}";
        private static readonly NodeType serializationNodeType = NodeType.OBJECT;
        public NodeType NodeType => serializationNodeType;

        public bool IsValid(string data, out IList<INodeData> nodeDatas, int position)
        {
            if(string.IsNullOrEmpty(data))
            {
                nodeDatas = null;
                return false;
            }
            if(data.Length < 2)
            {
                nodeDatas = null;
                return false;
            }
            if (data[0] != '{')
            {
                nodeDatas = null;
                return false;
            }
            if (data[data.Length - 1] != '}')
            {
                nodeDatas = null;
                return false;
            }


            var content = data.Substring(1, data.Length - 2);

            NodeDataBase obj = new NodeDataBase(NodeType, data, position);

            nodeDatas = new List<INodeData>(1);

            INodeData symbolStart = new NodeDataBase(NodeType.SYMBOL, START_SYMBOL, position);
            INodeData nodeData = new NodeDataBase(NodeType, content, position);
            INodeData symbolEnd = new NodeDataBase(NodeType.SYMBOL, END_SYMBOL, position);


            obj.SubNodes.Add(symbolStart);
            obj.SubNodes.Add(nodeData);
            obj.SubNodes.Add(symbolEnd);

            nodeDatas.Add(obj);

            return true;
        }
    }
}
