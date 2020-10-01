using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Library.Extractors
{
    public class MainParser
    {
        private IList<INodeParser> NodeParsers { get; }
        private IList<INodeParser> NodePrepocessor { get; }
        private IList<IParserNoSeparator> NoSeparator { get; }
        private List<INodeData> NodeDatas { get; set; }
        private List<INodeData> PreprocessedNodeDatas { get; set; }

        private StringBuilder stringBuilder;

        private Stack<string> Brackets;

        public MainParser()
        {
            stringBuilder = new StringBuilder();

            Brackets = new Stack<string>();


            NodeDatas = new List<INodeData>();

            PreprocessedNodeDatas = new List<INodeData>();

            NoSeparator = new List<IParserNoSeparator>()
            {
                new CommentNodeParser(),
                new MetadataNodeParser()
            };

            NodeParsers = new List<INodeParser>()
            {
                new ObjectNodeParser(),
                new MetadataNodeParser(),
                new CommentNodeParser(),
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
                bool skip = false;


                // check for nodes that dont need separators
                foreach (IParserNoSeparator noSep in NoSeparator)
                {
                    string prop = stringBuilder.ToString();

                    if (noSep.Validate(prop) && Brackets.Count == 0)
                    {
                        ParserPass(i);
                        break;
                    }
                }

                // check for nodes that need separators
                if (currentChar == SerializerConsts.DATA_SEPARATOR && Brackets.Count == 0)
                {
                    ParserPass(i);
                    continue;
                }



                // check for nodes with wrappers
                foreach(INodeParser withWrapping in NodeParsers)
                {
                    if (!withWrapping.HasWrapping)
                        continue;

                    if (i+1 >= withWrapping.WrappingStart.Length)
                    {
                        string lastChars = data.Substring(i, withWrapping.WrappingStart.Length);

                        if (lastChars.Equals(withWrapping.WrappingStart))
                        {
                            Brackets.Push(withWrapping.WrappingStart);
                            stringBuilder.Append(withWrapping.WrappingStart);
                            skip = true;
                            break;
                        }
                    }

                    if (i+1 >= withWrapping.WrappingEnd.Length)
                    {
                        string lastChars = data.Substring(i, withWrapping.WrappingEnd.Length);

                        if (lastChars.Equals(withWrapping.WrappingEnd))
                        {
                            if (Brackets.Peek().Equals(withWrapping.WrappingStart))
                            {
                                stringBuilder.Append(withWrapping.WrappingEnd);
                                Brackets.Pop();
                                skip = true;
                                break;
                            }
                        }
                    }

                }


                if (skip)
                    continue;


                /*
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
                */

                if (currentChar == '\n' || currentChar == '\t')
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
                
                string patched;

                if (parser.IsValid(prop, out nodes, position , out patched))
                {
                    stringBuilder = new StringBuilder(patched);

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

                    // rest of the node
                    if (patched.Length != 0)
                    {
                        var patchedParser = new MainParser();

                        IList<INodeData> restOfNodes = null;

                        patchedParser.ExtractNodeData(patched, out restOfNodes);
                        
                        foreach(var n in restOfNodes)
                        {
                            NodeDatas.Add(n);
                        }
                    }

                    return;
                }
            }
        }
    }
}
