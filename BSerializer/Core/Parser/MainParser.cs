using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Library.Extractors
{
    public class MainParser
    {
        private static List<INodeParser> nodeParsers { get; set; }
        private static IList<INodeParser> NodeParsers
        {
            get
            {
                if (nodeParsers == null)
                {
                    nodeParsers = new List<INodeParser>()
                    {
                        ArrayNodeParser.Instance,
                        ObjectNodeParser.Instance,
                        MetadataNodeParser.Instance,
                        CommentNodeParser.Instance,
                        DataNodeParser.Instance
                    };
                }

                return nodeParsers;
            }
        }
        private static List<IParserNoSeparator> noSeparator { get; set; }
        private static IList<IParserNoSeparator> NoSeparator
        {
            get
            {
                if (noSeparator == null)
                {
                    noSeparator = new List<IParserNoSeparator>()
                    {
                        CommentNodeParser.Instance,
                        MetadataNodeParser.Instance
                    };
                }

                return noSeparator;
            }
        }
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
        }


        public bool ExtractNodeData(string data, out IList<INodeData> nodes)
        {
            NodeDatas.Clear();

            Brackets.Clear();

            stringBuilder.Clear();

            string prop = null;

            for (int i = 0; i < data.Length; i++)
            {
                char currentChar = data[i];
                bool skip = false;

                prop = null;
                
                // check for nodes that dont need separators
                foreach (IParserNoSeparator noSep in NoSeparator)
                {
                    if (Brackets.Count != 0)
                        continue;

                    if(prop == null)
                    {
                        prop = stringBuilder.ToString();

                    }

                    if (noSep.Validate(prop) && Brackets.Count == 0)
                    {
                        ParserPass(i);
                        break;
                    }
                }

                // check for nodes that need separators
                if (currentChar == SerializerConsts.DATA_SEPARATOR && Brackets.Count == 0)
                {
                    if (stringBuilder.Length != 0)
                    {
                        ParserPass(i);
                    }
                    continue;
                }



                // check for nodes with wrappers
                foreach (INodeParser withWrapping in NodeParsers)
                {
                    if (!withWrapping.HasWrapping)
                        continue;


                    if (i + 1 >= withWrapping.WrappingEnd.Length)
                    {
                        string lastChars = data.Substring(i, withWrapping.WrappingEnd.Length);

                        if (lastChars.Equals(withWrapping.WrappingEnd))
                        {
                            if (Brackets.Count == 0)
                            {
                                skip = false;
                                continue;
                            }
                            if (Brackets.Peek().Equals(withWrapping.WrappingStart))
                            {
                                stringBuilder.Append(withWrapping.WrappingEnd);
                                Brackets.Pop();
                                skip = true;
                                break;
                            }
                        }
                    }

                    if (i + 1 >= withWrapping.WrappingStart.Length)
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

                if (currentChar == '\n' || currentChar == '\t' || currentChar == '\r' || currentChar == '\b')
                {
                    continue;
                }

                stringBuilder.Append(currentChar);
            }

            if (stringBuilder.Length == 0)
            {
                nodes = null;
                return false;
            }

            ParserPass(stringBuilder.Length);
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

                if (parser.IsValid(prop, out nodes, position, out patched))
                {
                    stringBuilder = new StringBuilder(patched);

                    for (int n = 0; n < nodes.Count; n++)
                    {
                        NodeDatas.Add(nodes[n]);

                        for (int i1 = 0; i1 < nodes[n].SubNodes.Count; i1++)
                        {
                            INodeData sub = nodes[n].SubNodes[i1];
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

                        for (int i1 = 0; i1 < restOfNodes.Count; i1++)
                        {
                            INodeData n = restOfNodes[i1];
                            NodeDatas.Add(n);
                        }
                    }

                    return;
                }
            }
        }
    }
}
