using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using System.Collections.Generic;
using System.Text;

namespace Library.Extractors
{
    public class MainParser
    {
        private IList<INodeParser> NodeParsers { get; }
        private IList<INodeParser> NodePrepocessor { get; }
        private List<INodeData> NodeDatas { get; set; }
        private List<INodeData> PreprocessedNodeDatas { get; set; }

        private StringBuilder stringBuilder;

        private Stack<char> Brackets;

        public MainParser()
        {
            stringBuilder = new StringBuilder();

            Brackets = new Stack<char>();

            NodeDatas = new List<INodeData>();

            PreprocessedNodeDatas = new List<INodeData>();

            NodePrepocessor = new List<INodeParser>()
            {
                new CommentNodeParser()
            };

            NodeParsers = new List<INodeParser>()
            {
                new ObjectNodeParser(),
                new DataNodeParser()
            };
        }


        public bool ExtractNodeData(string data, out IList<INodeData> nodes)
        {
            NodeDatas.Clear();

            Brackets.Clear();

            stringBuilder.Clear();

            for (int i = 0; i < data.Length; i++)
            {
                char currentChar = data[i];

                if (currentChar == ',' && Brackets.Count == 0)
                {
                    ParserPass(i);
                    continue;
                }

                if(currentChar == '{')
                {
                    Brackets.Push('{');
                    stringBuilder.Append('{');
                    continue;
                }

                if (currentChar == '}')
                {
                    if (Brackets.Peek() == '{')
                    {
                        stringBuilder.Append('}');
                        Brackets.Pop();
                        continue;
                    }
                }

                if(currentChar == '\n' || currentChar == '\t')
                {
                    continue;
                }

                if(currentChar == ' ')
                {
                    continue;
                }

                stringBuilder.Append(currentChar);
            }

            if(stringBuilder.Length == 0)
            {
                nodes = null;
                return false;
            }

            ParserPass(data.Length);
            nodes = new List<INodeData>(NodeDatas);
            return true;
        }

        private void ParserPass(int i)
        {
            var prop = stringBuilder.ToString();

            for (int p = 0; p < NodeParsers.Count; p++)
            {
                INodeParser parser = NodeParsers[p];

                IList<INodeData> nodes = null;

                int position = i - prop.Length;

                if (parser.IsValid(prop, out nodes, position))
                {
                    stringBuilder.Clear();

                    for (int n = 0; n < nodes.Count; n++)
                    {

                        NodeDatas.Add(nodes[n]);

                        foreach (var sub in nodes[n].SubNodes)
                        {

                            if (!NodeUtils.IsTerminal(sub.Type))
                            {
                                IList<INodeData> subnodes;

                                var mainParser = new MainParser();

                                mainParser.ExtractNodeData(sub.Data, out subnodes);
                                sub.SubNodes.AddRange(subnodes);
                            }
                        }
                    }

                    return;
                }
            }
        }
    }
}
