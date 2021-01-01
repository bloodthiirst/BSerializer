using BSerializer.Core.Base;
using BSerializer.Core.Collection;
using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using Library.Extractors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace BSerializer.Core.Custom
{
    public abstract class CustomSerializerBase : ISerializerInternal
    {
        private const string NULL = "null";
        public Type CustomType { get; }
        private ISerializerInternal asInterface => this;
        public CustomSerializerBase(Type customType)
        {
            CustomType = customType;

            SerializerDependencies.SerializerCollection.GetOrAdd(CustomType, this);
        }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;
        public abstract INodeParser NodeParser { get; }

        /// <summary>
        /// Validates the nodes and returns the instance if it is already in cache
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="type"></param>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        internal abstract bool ValidateNodes(IList<INodeData> nodes);
        internal abstract object ReadObjectData(IList<INodeData> list, DeserializationContext context, int currentIndex);
        internal abstract string WriteObjectData(object obj, SerializationContext context, StringBuilder sb);

        public object Deserialize(string s)
        {
            return asInterface.Deserialize(s, new DeserializationContext());
        }

        public string Serialize(object obj)
        {
            return asInterface.Serialize(obj, new SerializationContext());
        }

        public bool TryDeserialize(string s, ref object obj)
        {
            throw new NotImplementedException();
        }

        public bool TrySerialize(object obj, ref string s)
        {
            throw new NotImplementedException();
        }

        string ISerializerInternal.Serialize(object obj, SerializationContext context)
        {
            if (obj == null)
                return EmptySymbol;

            if (obj.Equals(EmptyValue))
                return EmptySymbol;

            StringBuilder sb = new StringBuilder();

            // if the object is already cached
            // then only put the id and data necessary to get it back from cache on deserialization
            if (context.TryGet(obj, out int reference))
            {
                sb.Append(NodeParser.WrappingStart);
                sb.Append('\n');
                context.TabPadding++;
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                WriteHeader(sb, reference);
                sb.Append('\n');
                context.TabPadding--;
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                sb.Append(NodeParser.WrappingEnd);
                return sb.ToString();
            }

            // else write the object
            return WriteObjectData(obj, context, sb);
        }

        internal void WriteHeader(StringBuilder sb, int reference)
        {
            sb.Append("<");
            sb.Append(CustomType.FullName);
            sb.Append(SerializerConsts.DATA_SEPARATOR);
            sb.Append(reference);
            sb.Append(">");
        }

        object ISerializerInternal.Deserialize(string data, DeserializationContext context)
        {
            if (data.Equals(EmptySymbol))
            {
                return EmptyValue;
            }

            MainParser parser = new MainParser();
            IList<INodeData> list;
            parser.ExtractNodeData(data, out list);

            // get the metadata
            List<INodeData> validNodes = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            INodeData typeNode = list[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            CustomSerializer serializer = SerializerUtils.MetadataSerializer;

            Metadata metadata = (Metadata)serializer.ReadObjectData(new List<INodeData>() { typeNode }, context, -1);

            // check if the object is already in cache
            // if that's the case then return it
            if (context.TryGet(metadata.ReferenceTracker, out object cacheFound))
            {
                return cacheFound;
            }

            // validate the node structure
            if (!ValidateNodes(list))
            {
                return EmptyValue;
            }

            // else do the normal deserialization
            object result = ReadObjectData(list, context , metadata.ReferenceTracker);

            return result;
        }
    }
}
