using BSerializer.Core.Custom;
using BSerializer.Core.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace BSerializer.Core.Nodes
{
    public static class NodeUtils
    {
        

        private static NodeType[] TerminalNodeType = new NodeType[]
        {
            NodeType.COMMENT,
            NodeType.DATA,
            NodeType.METADATA,
            NodeType.SYMBOL
        };

        private static NodeType[] IgnoredOnDeserialization = new NodeType[]
        {
            NodeType.COMMENT,
            NodeType.METADATA
        };


        public static bool IsTerminal(NodeType nodeType)
        {
            for(int i = 0; i < TerminalNodeType.Length;i++)
            {
                if(TerminalNodeType[i] == nodeType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IgnoreOnDeserialization(NodeType nodeType)
        {
            for (int i = 0; i < IgnoredOnDeserialization.Length; i++)
            {
                if (IgnoredOnDeserialization[i] == nodeType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
