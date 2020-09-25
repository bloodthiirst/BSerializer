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
            NodeType.SYMBOL
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
    }
}
