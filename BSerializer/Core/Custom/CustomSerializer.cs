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
    public class CustomSerializer : ISerializerInternal
    {
        private const string NULL = "null";
        public Type CustomType { get; }
        private List<ISerializerInternal> Serializers { get; }
        private List<Action<object, object>> PropertieSetter { get; set; }
        private List<Func<object, object>> PropertieGetter { get; set; }
        public IReadOnlyList<Action<object, object>> PropertieSetters => PropertieSetter;
        public IReadOnlyList<Func<object, object>> PropertieGetters => PropertieGetter;
        private IList<string> PropertiesName { get; set; }
        private ISerializerInternal asInterface { get; }
        private int PropertiesCount { get; set; }
        public CustomSerializer(Type customType)
        {
            CustomType = customType;

            Serializers = new List<ISerializerInternal>();
            PropertieSetter = new List<Action<object, object>>();
            PropertieGetter = new List<Func<object, object>>();

            PropertiesName = new List<string>();
            asInterface = this;

            SerializerDependencies.SerializerCollection.GetOrAdd(CustomType, this);

            Initialize();
        }

        public Type Type => CustomType;

        public string EmptySymbol => NULL;

        public object EmptyValue => null;

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
            List<PropertyInfo> props = GetSerializableProperties(CustomType);

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
            List<FieldInfo> fields = GetSerializableFields(CustomType);

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

        private bool CanDeserialize(IList<INodeData> nodes, out Type type, out Metadata metadata, DeserializationContext context, out object cached)
        {
            if (nodes.Count != 1)
            {
                type = null;
                metadata = null;
                cached = null;
                return false;
            }

            if (nodes[0].Type != NodeType.OBJECT)
            {
                type = null;
                metadata = null;
                cached = null;
                return false;
            }

            INodeData typeNode = nodes[0].SubNodes[1].SubNodes.FirstOrDefault(n => n.Type == NodeType.METADATA);

            CustomSerializer serializer = new CustomSerializer(typeof(Metadata));
            metadata = (Metadata)serializer.DeserializeFromNodes(new List<INodeData>() { typeNode }, context , -1);
            Type typeFromString = Assembly.GetEntryAssembly().GetType(metadata.TypeFullName);

            if (context.TryGet(metadata.ReferenceTracker, out cached))
            {
                type = typeFromString;
                return true;
            }

            List<INodeData> validNodes = nodes[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            if (validNodes.Count != PropertiesCount)
            {
                type = null;
                metadata = null;
                cached = null;
                return false;
            }

            if (!CustomType.IsAssignableFrom(typeFromString))
            {
                type = typeFromString;

                return false;
            }

            type = typeFromString;
            return true;

        }

        internal object DeserializeFromNodes(IList<INodeData> list, DeserializationContext context , int currentIndex)
        {
            object instance = Activator.CreateInstance(CustomType);

            if (currentIndex != -1)
            {
                // note : we do this to count for the case where instance A has itself as one of its properties
                // so we have to preregister the parent to catch it back when we encounter it back as a property
                context.Register(currentIndex, instance);
            }

            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            int propIndex = 0;

            for (int i = 0; i < PropertiesCount; i++)
            {
                INodeData node = list[i];

                if (node.Type == Parser.NodeType.COMMENT)
                    continue;
                if (node.Type == Parser.NodeType.SYMBOL)
                    continue;

                object val = Serializers[propIndex].Deserialize(node.Data, context);

                PropertieSetter[i].Invoke(instance, val);

                propIndex++;
            }

            return instance;
        }

        internal void DeserializeFromNodesInto(IList<INodeData> list, DeserializationContext context, ref object instance)
        {
            list = list[0].SubNodes[1].SubNodes.Where(n => !NodeUtils.IgnoreOnDeserialization(n.Type)).ToList();

            int propIndex = 0;

            for (int i = 0; i < PropertiesCount; i++)
            {
                INodeData node = list[i];

                if (node.Type == Parser.NodeType.COMMENT)
                    continue;
                if (node.Type == Parser.NodeType.SYMBOL)
                    continue;

                object val = Serializers[propIndex].Deserialize(node.Data, context);

                PropertieSetter[i].Invoke(instance, val);

                propIndex++;
            }
        }


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



            ObjectNodeParser objectNodeParser = new ObjectNodeParser();
            StringBuilder sb = new StringBuilder();

            if (context.TryGet(obj, out int reference))
            {
                sb.Append(objectNodeParser.WrappingStart);
                sb.Append('\n');
                context.TabPadding++;
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                sb.Append("<");
                sb.Append(CustomType.FullName);
                sb.Append(SerializerConsts.DATA_SEPARATOR);
                sb.Append(reference);
                sb.Append(">");
                sb.Append('\n');
                context.TabPadding--;
                sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                sb.Append(objectNodeParser.WrappingEnd);
                return sb.ToString();
            }


            sb.Append(objectNodeParser.WrappingStart);
            sb.Append('\n');
            context.TabPadding++;
            sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
            sb.Append("<");
            sb.Append(CustomType.FullName);
            sb.Append(SerializerConsts.DATA_SEPARATOR);
            context.Register(obj, out int newRef);
            sb.Append(newRef);
            sb.Append(">");
            sb.Append('\n');

            if (PropertiesCount != 0)
            {
                for (int i = 0; i < PropertiesCount - 1; i++)
                {
                    object val = PropertieGetter[i].Invoke(obj);


                    string valAsString = Serializers[i].Serialize(val, context);

                    if (context.WithPropertiesComments)
                    {
                        sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                        sb.Append($"# { PropertiesName[i] } #");
                        sb.Append('\n');
                    }

                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append(valAsString);
                    sb.Append(SerializerConsts.DATA_SEPARATOR);
                    sb.Append('\n');
                }

                if (context.WithPropertiesComments)
                {
                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    sb.Append($"# { PropertiesName[PropertiesCount - 1] } #");
                    sb.Append('\n');
                }

                if (PropertiesCount - 1 >= 0)
                {

                    sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
                    object lastVal = PropertieGetter[PropertiesCount - 1].Invoke(obj);
                    string lastValAsString = Serializers[PropertiesCount - 1].Serialize(lastVal, context);
                    sb.Append(lastValAsString);
                }
            }

            sb.Append('\n');
            context.TabPadding--;
            sb.Append(SerializerUtils.GetTabSpaces(context.TabPadding));
            sb.Append(objectNodeParser.WrappingEnd);

            context.SaveValue(newRef, sb.ToString());

            return sb.ToString();
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

            Type instanceType;
            Metadata metadata;
            if (!CanDeserialize(list, out instanceType, out metadata, context , out var cached))
            {
                return EmptyValue;
            }

            if(cached != null)
            {
                return cached;
            }

            if (context.TryGet(metadata.ReferenceTracker, out object instance))
            {
                return instance;
            }

            var result = DeserializeFromNodes(list, context , metadata.ReferenceTracker);

            return result;
        }

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

            Type instanceType;
            Metadata metadata;
            if (!CanDeserialize(list, out instanceType, out metadata, context , out object cached))
            {
                instance = EmptyValue;
                return;
            }

            if (context.TryGet(metadata.ReferenceTracker, out object trackedRefInstance))
            {
                instance = trackedRefInstance;
            }

            DeserializeFromNodesInto(list, context, ref instance);

            context.Register(metadata.ReferenceTracker, instance);
        }
    }
}
