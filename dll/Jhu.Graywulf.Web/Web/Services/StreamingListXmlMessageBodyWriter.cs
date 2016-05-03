using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    public class StreamingListXmlMessageBodyWriter : StreamBodyWriter
    {
        private object result;
        private Encoding encoding;

        public StreamingListXmlMessageBodyWriter(object result)
            : this(result, System.Text.Encoding.ASCII)
        {
            // overload
        }

        public StreamingListXmlMessageBodyWriter(object result, Encoding encoding)
            : base(false)
        {
            this.result = result;
            this.encoding = encoding;
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
            //writer.WriteStartElement("Binary");

            base.OnWriteBodyContents(writer);

            //writer.WriteEndElement();
        }

        protected override void OnWriteBodyContents(Stream stream)
        {
            var w = new StreamWriter(stream, encoding);
            var x = new StreamingListXmlWriter(w);

            var type = result.GetType();

            // Figure out list name
            string className, classNamespace;
            ReflectClass(type, out className, out classNamespace);

            x.WriteStartElement(className, classNamespace);
            x.WriteAttributeString("xmlns", classNamespace);
            x.WriteAttributeString("xmlns:i", "http://www.w3.org/2001/XMLSchema-instance");

            x.SkipNamespace = true;

            // Iterate through properties
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];

                string propName;
                ReflectProperty(prop, out propName);

                x.WriteStartElement(propName);

                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    
                    var itemType = prop.PropertyType.GetGenericArguments()[0];
                    WriteEnumerableItems(w, x, itemType, (IEnumerable)prop.GetValue(result));   
                }
                else
                {
                    // fallback to DataContractSerializer
                    var s = new DataContractSerializer(prop.PropertyType);
                    s.WriteObjectContent(x, prop.GetValue(result));
                }

                x.WriteEndElement();
            }

            x.SkipNamespace = false;

            x.WriteEndElement();
            w.WriteLine();
            w.Flush();
        }

        private void WriteEnumerableItems(StreamWriter w,  StreamingListXmlWriter x, Type itemType, IEnumerable items)
        {
            // Iterate through the collection
            var s = new DataContractSerializer(itemType);
            foreach (var i in items)
            {
                s.WriteObject(x, i);

                // For some reason, this is necessary here to flush buffer to output
                w.WriteLine();
                w.Flush();
            }
        }

        private void ReflectClass(Type type, out string name, out string ns)
        {
            // No attribute present
            // <ArrayOffootprint xmlns:i="http://www.w3.org/2001/XMLSchema-instance">

            // Inside class:
            // <footprintList xmlns="" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><footprints>
            // <footprint>....

            name = ns = null;

            // Look for any attributes that control serialization of name
            var attrs = type.GetCustomAttributes(true);

            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];

                if (attr is XmlRootAttribute)
                {
                    var a = (XmlRootAttribute)attr;
                    name = a.ElementName;
                    ns = a.Namespace;
                }
                else if (attr is DataContractAttribute)
                {
                    var a = (DataContractAttribute)attr;
                    name = a.Name;
                    ns = a.Namespace;
                }
            }

            name = name ?? type.Name;
            ns = ns ?? "http://schemas.datacontract.org/2004/07/" + type.Namespace;
        }

        private void ReflectProperty(PropertyInfo prop, out string name)
        {
            name = null;

            // Look for any attributes that control serialization of name
            var attrs = prop.GetCustomAttributes(true);

            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];

                if (attr is XmlElementAttribute)
                {
                    var a = (XmlElementAttribute)attr;
                    name = a.ElementName;
                }
                else if (attr is DataMemberAttribute)
                {
                    var a = (DataMemberAttribute)attr;
                    name = a.Name;
                }
            }
        }
    }
}
