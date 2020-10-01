using BSerializer.Core.Parser.SerializationNodes;
using Library.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSerializer.Core.Parser
{
    public class MetadataNodeParser : INodeParser , IParserNoSeparator
    {
        private const string START_SYMBOL = "<";
        private const string END_SYMBOL = ">";
        private readonly int START_LENGTH = START_SYMBOL.Length;
        private readonly int END_LENGTH = END_SYMBOL.Length;
        private static readonly NodeType serializationNodeType = NodeType.METADATA;
        public NodeType NodeType => serializationNodeType;
        public string WrappingStart => START_SYMBOL;

        public string WrappingEnd => END_SYMBOL;

        public bool HasWrapping => true;

        public bool NeedsSeparation => false;

        public bool IsValid(string data, out IList<INodeData> nodeDatas , int position, out string patched)
        {
            if(string.IsNullOrEmpty(data))
            {
                nodeDatas = null;
                patched = data;
                return false;
            }
            if(data.Length < 2)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            var commentStarts = AllIndexesOf(data, START_SYMBOL).ToList();
            var commentEnds = AllIndexesOf(data, END_SYMBOL).ToList();

            if(commentStarts.Count() == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            if(commentEnds.Count() == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            var indexes = GetIndexes(data);

            int startIndex = indexes[0] + 1;
            int endIndex = indexes[1];


            var content = data.Substring(startIndex, endIndex - startIndex);

            var fullText = data.Substring(startIndex - START_LENGTH, content.Length + START_LENGTH + END_LENGTH); 

            NodeDataBase obj = new NodeDataBase(NodeType, fullText, position);

            nodeDatas = new List<INodeData>(1);

            INodeData symbolStart = new NodeDataBase(NodeType.SYMBOL, START_SYMBOL, position);
            INodeData nodeData = new NodeDataBase(NodeType, content, position);
            INodeData symbolEnd = new NodeDataBase(NodeType.SYMBOL, END_SYMBOL, position);

            patched = data.Substring(0, startIndex - 1);
            patched += data.Substring(endIndex + 1 , data.Length - endIndex - 1);

            MainParser mainParser = new MainParser();
            IList<INodeData> list;
            mainParser.ExtractNodeData(content, out list);
            nodeData.SubNodes.AddRange(list);

            obj.SubNodes.Add(symbolStart);
            obj.SubNodes.Add(nodeData);
            obj.SubNodes.Add(symbolEnd);

            nodeDatas.Add(obj);

            return true;
        }

        private IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            int minIndex = str.IndexOf(searchstring);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length);
            }
        }

        private List<int> GetIndexes(string data)
        {
            var starts = AllIndexesOf(data, START_SYMBOL).ToList();
            var ends = AllIndexesOf(data, END_SYMBOL).ToList();


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

            return hash.ToList();
        }

        public bool Validate(string data)
        {
            var starts = AllIndexesOf(data, START_SYMBOL).ToList();
            var ends = AllIndexesOf(data, END_SYMBOL).ToList();

            if (starts.Count() == 0)
            {
                return false;
            }

            if (ends.Count() == 0)
            {
                return false;
            }
            // make sure there's at least 2 different indexes
            var hash = new HashSet<int>();


            foreach(var i in starts)
            {
                hash.Add(i);
            }

            foreach (var i in ends)
            {
                hash.Add(i);
            }

            if(hash.Count < 2)
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
