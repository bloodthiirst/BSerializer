using BSerializer.Core.Base;
using BSerializer.Core.Nodes;
using BSerializer.Core.Parser;
using BSerializer.Core.Parser.SerializationNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BSerializer.Core.Custom
{
    public class CustomSerializer : CustomSerializerBase
    {
        private const string NULL = "0";
        public override object EmptyValue => null;
        public override string EmptySymbol => NULL;
        private List<ISerializerInternal> Serializers { get; }
        private List<Action<object, object>> PropertieSetter { get; set; }
        private List<Func<object, object>> PropertieGetter { get; set; }
        private IList<string> PropertiesName { get; set; }
        private int PropertiesCount { get; set; }
        public override INodeParser NodeParser => ObjectNodeParser.Instance;
        public CustomSerializer(Type customType) : base(customType)
        {
            Serializers = new List<ISerializerInternal>();

            PropertieSetter = new List<Action<object, object>>();
            PropertieGetter = new List<Func<object, object>>();

            PropertiesName = new List<string>();

            Initialize();
        }

        private List<PropertyInfo> GetSerializableProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(p => p.GetAccessors().Length == 2)
                .ToList();
        }

        private List<FieldInfo> GetSerializableFields(Type type)
        {
            return type
                .GetFields()
                .ToList();
        }

        private void Initialize()
        {
            // get properties
            List<PropertyInfo> props = GetSerializableProperties(Type);

            PropertiesCount = props.Count;

            for (int i = 0; i < props.Count; i++)
            {
                PropertyInfo prop = props[i];
                Func<object, object> getter = SerializerUtils.PropertyGetterToDelegate(prop.GetMethod);
                Action<object, object> setter = SerializerUtils.PropertySetterToDelegate(prop.SetMethod);


                ISerializerInternal serializer = SerializerDependencies.SerializerCollection.GetOrAdd(prop.PropertyType);

                PropertieGetter.Add(getter);
                PropertieSetter.Add(setter);
                Serializers.Add(serializer);
                PropertiesName.Add(prop.Name);
            }

            // get fields
            List<FieldInfo> fields = GetSerializableFields(Type);

            var FieldsCount = fields.Count;

            PropertiesCount += FieldsCount;

            for (int i = 0; i < fields.Count; i++)
            {
                FieldInfo field = fields[i];
                Func<object, object> getter = SerializerUtils.FieldGetterToDelegate(field);
                Action<object, object> setter = SerializerUtils.FieldSetterToDelegate(field);

                ISerializerInternal serializer = SerializerDependencies.SerializerCollection.GetOrAdd(field.FieldType);

                PropertieGetter.Add(getter);
                PropertieSetter.Add(setter);
                Serializers.Add(serializer);
                PropertiesName.Add(field.Name);
            }
        }

        /// <summary>
        /// Validates the nodes and returns the instance if it is already in cache
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="type"></param>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        internal override bool ValidateNodes(IList<INodeData> nodes)
        {
            if (nodes.Count != 1)
            {
                return false;
            }

            if (nodes[0].Type != NodeType.OBJECT)
            {
                return false;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            if (validNodes.Count != PropertiesCount)
            {
                return false;
            }

            return true;
        }

        internal override object ReadObjectData(IList<INodeData> list, DeserializationContext context, int currentIndex)
        {
            // prepare the instance to deserialize into
            object instance = Activator.CreateInstance(Type);

            if (currentIndex != -1)
            {
                // note : we do this to count for the case where instance A has itself as one of its properties
                // so we have to preregister the parent to catch it back when we encounter it back as a property
                context.Register(currentIndex, instance);
            }

            //list = list[0].SubNodes[1].SubNodes.Where(n => !).ToList();

            list = list[0].SubNodes[1].SubNodes;

            int propIndex = 0;

            int nodeIndex = -1;

            for (int i = 0; i < PropertiesCount; i++)
            {
                while (nodeIndex < list.Count)
                {
                    nodeIndex++;
                    if (!NodeUtils.IgnoreOnDeserialization(list[nodeIndex].Type))
                    {
                        break;
                    }
                }

                INodeData node = list[nodeIndex];

                /*
                if (node.Type == Parser.NodeType.COMMENT)
                    continue;
                if (node.Type == Parser.NodeType.SYMBOL)
                    continue;
                */
                object val = Serializers[propIndex].DeserializeInternal(node.Data, context);

                PropertieSetter[i].Invoke(instance, val);

                propIndex++;
            }

            return instance;
        }

        internal void InjectDataIntoInstance(IList<INodeData> list, DeserializationContext context, ref object instance)
        {
            list = list[0].SubNodes[1].SubNodes;

            int propIndex = 0;
            int nodeIndex = -1;

            for (int i = 0; i < PropertiesCount; i++)
            {
                while (nodeIndex < list.Count)
                {
                    nodeIndex++;
                    if (!NodeUtils.IgnoreOnDeserialization(list[nodeIndex].Type))
                    {
                        break;
                    }
                }

                INodeData node = list[nodeIndex];

                /*
                if (node.Type == Parser.NodeType.COMMENT)
                    continue;
                if (node.Type == Parser.NodeType.SYMBOL)
                    continue;
                */
                object val = Serializers[propIndex].DeserializeInternal(node.Data, context);

                PropertieSetter[i].Invoke(instance, val);

                propIndex++;
            }
        }
        internal override void WriteObjectData(object obj, SerializationContext context, StringBuilder sb)
        {

            // else then we deserialize the data inside
            sb.Append(NodeParser.WrappingStart);
            sb.Append('\n');
            context.TabPadding++;
            SerializerUtils.GetTabSpaces(context.TabPadding, sb);

            context.Register(obj, out int newRef);

            WriteHeader(sb, newRef);
            sb.Append('\n');

            if (PropertiesCount != 0)
            {
                for (int i = 0; i < PropertiesCount - 1; i++)
                {
                    object val = PropertieGetter[i].Invoke(obj);



                    if (context.WithPropertiesComments)
                    {
                        SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                        sb.Append($"# { PropertiesName[i] } #");
                        sb.Append('\n');
                    }

                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);

                    Serializers[i].SerializeInternal(val, context, sb);

                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    sb.Append('\n');
                }

                if (context.WithPropertiesComments)
                {
                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    sb.Append($"# { PropertiesName[PropertiesCount - 1] } #");
                    sb.Append('\n');
                }

                if (PropertiesCount - 1 >= 0)
                {

                    SerializerUtils.GetTabSpaces(context.TabPadding, sb);
                    object lastVal = PropertieGetter[PropertiesCount - 1].Invoke(obj);
                    Serializers[PropertiesCount - 1].SerializeInternal(lastVal, context, sb);

                }
            }

            sb.Append('\n');
            context.TabPadding--;
            SerializerUtils.GetTabSpaces(context.TabPadding, sb);
            sb.Append(NodeParser.WrappingEnd);
        }

        /*
        /// <summary>
        /// TODO : test this method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="data"></param>
        public void DeserializeInto(object instance, string data)
        {
            var context = new DeserializationContext();

            if (data.Equals(EmptySymbol))
            {
                instance = EmptyValue;
                return;
            }


            MainParser parser = new MainParser();
            IList<INodeData> list;
            parser.ExtractNodeData(data, out list);

            if (!ValidateNodes(list))
            {
                instance = EmptyValue;
                return;
            }

            if (context.TryGet(metadata.ReferenceTracker, out object trackedRefInstance))
            {
                instance = trackedRefInstance;
            }

            InjectDataIntoInstance(list, context, ref instance);
        }
                */
    }
}
