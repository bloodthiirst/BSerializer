using BSerializer.Core.Collection;
using BSerializer.Core.Custom;
using BSerializer.Core.Parser.SerializationNodes;
using ConsoleUI.Model;
using Library.Extractors;
using System.Collections.Generic;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            SerializerCollection collection = new SerializerCollection();
            CustomSerializer customSerializer = new CustomSerializer(typeof(Person), collection);

            MainParser parser = new MainParser();

            string data = "{ SomeData, { Child , otherData , 3 } , Bloodthirst }";

            IList<INodeData> nodes = null;
            
            parser.ExtractNodeData(data, out nodes);
        }
    }
}
