using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSerializer.Core.Parser
{
    public class CommentNodeParser : INodeParser, IParserNoSeparator
    {
        private static CommentNodeParser _Instance { get; set; }
        public static CommentNodeParser Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new CommentNodeParser();
                }

                return _Instance;
            }
        }

        private const string START_SYMBOL = "#";
        private const string END_SYMBOL = "#";
        private readonly int START_LENGTH = START_SYMBOL.Length;
        private readonly int END_LENGTH = END_SYMBOL.Length;
        private static readonly NodeType serializationNodeType = NodeType.COMMENT;
        public NodeType NodeType => serializationNodeType;
        public string WrappingStart => START_SYMBOL;

        public string WrappingEnd => END_SYMBOL;

        public bool HasWrapping => true;

        public bool NeedsSeparation => false;
        private List<int> cachedList = new List<int>();
        public bool IsValid(string data, out IList<INodeData> nodeDatas, int position, out string patched)
        {
            if (string.IsNullOrEmpty(data))
            {
                nodeDatas = null;
                patched = data;
                return false;
            }
            if (data.Length < 2)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            cachedList.Clear();

            AllIndexesOf(data, START_SYMBOL , cachedList);

            if (cachedList.Count() == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            AllIndexesOf(data, END_SYMBOL, cachedList);

            if (cachedList.Count() == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            var indexes = GetIndexes(data , cachedList);

            int startIndex = indexes[0] + 1;
            int endIndex = indexes[1];


            var content = data.Substring(startIndex, endIndex - startIndex);

            var fullText = data.Substring(startIndex - START_LENGTH, content.Length + START_LENGTH + END_LENGTH);

            NodeDataBase obj = new NodeDataBase(NodeType, fullText, position);

            nodeDatas = new List<INodeData>(1);

            INodeData symbolStart = new NodeDataBase(NodeType.COMMENT, START_SYMBOL, position);
            INodeData nodeData = new NodeDataBase(NodeType, content, position);
            INodeData symbolEnd = new NodeDataBase(NodeType.COMMENT, END_SYMBOL, position);

            patched = data.Substring(0, startIndex - 1);
            patched += data.Substring(endIndex + 1, data.Length - endIndex - 1);

            obj.SubNodes.Add(symbolStart);
            obj.SubNodes.Add(nodeData);
            obj.SubNodes.Add(symbolEnd);

            nodeDatas.Add(obj);

            return true;
        }

        private void AllIndexesOf(string str, string searchstring, List<int> output)
        {
            output.Clear();
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                output.Add(minIndex);
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }

        private List<int> GetIndexes(string data , List<int> res)
        {
            AllIndexesOf(data, START_SYMBOL, res);
            
            // make sure there's at least 2 different indexes
            var hash = new HashSet<int>();


            foreach (var i in res)
            {
                hash.Add(i);
            }

            AllIndexesOf(data, END_SYMBOL , res);

            foreach (var i in res)
            {
                hash.Add(i);
            }

            return hash.ToList();
        }

        public bool Validate(string data)
        {
            var starts = new List<int>();

            AllIndexesOf(data, START_SYMBOL, starts);

            if (starts.Count() == 0)
            {
                return false;
            }

            var ends = new List<int>();

            AllIndexesOf(data, END_SYMBOL, ends);

            if (ends.Count() == 0)
            {
                return false;
            }
            // make sure there's at least 2 different indexes
            var hash = new HashSet<int>();


            foreach (var i in starts)
            {
                hash.Add(i);
            }

            foreach (var i in ends)
            {
                hash.Add(i);
            }

            if (hash.Count < 2)
            {
                return false;
            }

            // if we have odd separators
            // that means its not well formatted
            if (hash.Count % 2 != 0)
            {
                return false;
            }


            return true;
        }
    }
}
