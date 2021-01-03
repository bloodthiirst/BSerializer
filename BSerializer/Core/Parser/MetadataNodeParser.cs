using BSerializer.Core.Parser.SerializationNodes;
using Library.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BSerializer.Core.Parser
{
    public class MetadataNodeParser : INodeParser, IParserNoSeparator
    {
        private static MetadataNodeParser _Instance { get; set; }
        public static MetadataNodeParser Instance
        {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new MetadataNodeParser();
                }

                return _Instance;
            }
        }

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


            int startIndex ;
            int endIndex;

            AllIndexesOf(data, START_SYMBOL, cachedList);
            

            if (cachedList.Count == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            startIndex = cachedList[0];

            AllIndexesOf(data, END_SYMBOL, cachedList);
            if (cachedList.Count == 0)
            {
                nodeDatas = null;
                patched = data;
                return false;
            }

            endIndex = cachedList[0];

            if (startIndex != 0)
            {
                patched = data;
                nodeDatas = new List<INodeData>();
                return false;
            }


            var content = data.Substring(startIndex + 1, endIndex - startIndex - 1);

            var fullText = data.Substring(startIndex, endIndex - startIndex + 1);

            NodeDataBase obj = new NodeDataBase(NodeType, fullText, position);

            nodeDatas = new List<INodeData>(1);

            INodeData symbolStart = new NodeDataBase(NodeType.SYMBOL, START_SYMBOL, position);
            INodeData nodeData = new NodeDataBase(NodeType.METADATA_CONTENT, content, position);
            INodeData symbolEnd = new NodeDataBase(NodeType.SYMBOL, END_SYMBOL, position);

            var firstPart = data.Substring(0, startIndex);
            var secondPart = data.Substring(endIndex + 1, data.Length - 1 - endIndex);
            patched = firstPart + secondPart;

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

        /*
        private List<int> GetIndexes(string data)
        {
            var starts = AllIndexesOf(data, START_SYMBOL);
            var ends = AllIndexesOf(data, END_SYMBOL);


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
        */
        public bool Validate(string data)
        {
            var end = new List<int>();

            AllIndexesOf(data, END_SYMBOL, end);

            if (end.Count() == 0)
            {
                return false;
            }

            var start = new List<int>();

            AllIndexesOf(data, START_SYMBOL, start);

            if (start.Count() == 0)
            {
                return false;
            }

            if (start[0] != 0)
            {
                return false;
            }

            // make sure there's at least 2 different indexes
            var hash = new HashSet<int>();


            foreach (var i in start)
            {
                hash.Add(i);
            }

            foreach (var i in end)
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
