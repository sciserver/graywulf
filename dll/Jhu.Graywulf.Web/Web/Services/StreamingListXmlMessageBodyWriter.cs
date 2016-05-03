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
            Dictionary<string, PropertyInfo> properties;
            StreamingListFormatter.ReflectClass(type, out className, out classNamespace, out properties);

            x.WriteStartElement(className, classNamespace);
            x.WriteAttributeString("xmlns", classNamespace);
            x.WriteAttributeString("xmlns:i", "http://www.w3.org/2001/XMLSchema-instance");

            x.SkipNamespace = true;

            foreach (var propName in properties.Keys)
            {
                var prop = properties[propName];

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
    }
}
