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
using System.Xml;

namespace Jhu.Graywulf.Web.Services
{
    public class StreamingListXmlMessageBodyWriter : StreamBodyWriter
    {
        private object result;
        private Encoding encoding;

        protected object Result
        {
            get { return result; }
        }

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
            base.OnWriteBodyContents(writer);
        }

        protected StreamWriter CreateStreamWriter(Stream stream)
        {
            return new StreamWriter(stream, encoding);
        }

        protected virtual XmlWriter CreateXmlWriter(Stream stream)
        {
            return new StreamingListXmlWriter(stream, encoding);
        }

        protected virtual XmlObjectSerializer CreateSerializer(Type type)
        {
            return new DataContractSerializer(type);
        }

        protected virtual void WriteStartRoot(XmlWriter x, string name, string ns)
        {
            x.WriteStartElement(name, ns);
            x.WriteAttributeString("xmlns", ns);
            x.WriteAttributeString("xmlns:i", "http://www.w3.org/2001/XMLSchema-instance");

            ((StreamingListXmlWriter)x).SkipNamespace = true;
        }

        protected virtual void WriteEndRoot(XmlWriter x)
        {
            ((StreamingListXmlWriter)x).SkipNamespace = false;

            x.WriteEndElement();
        }

        protected virtual void WriteStartObject(XmlWriter x, string name)
        {
            x.WriteStartElement(name);
        }

        protected virtual void WriteEndObject(XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected virtual void WriteStartArray(XmlWriter x, string name)
        {
            x.WriteStartElement(name);
        }

        protected virtual void WriteEndArray(XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected virtual void WriteStartArrayItem(XmlWriter x, string name)
        {
            x.WriteStartElement(name);
        }

        protected virtual void WriteEndArrayItem(XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected override void OnWriteBodyContents(Stream stream)
        {
            using (var w = CreateStreamWriter(stream))
            {
                using (var x = CreateXmlWriter(stream))
                {
                    var type = result.GetType();

                    // Figure out list name
                    string className, classNamespace;
                    Dictionary<string, PropertyInfo> properties;
                    StreamingListFormatter.ReflectClass(type, out className, out classNamespace, out properties);

                    WriteStartRoot(x, className, classNamespace);

                    foreach (var propName in properties.Keys)
                    {
                        var prop = properties[propName];

                        if (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            var itemType = prop.PropertyType.GetGenericArguments()[0];

                            WriteStartArray(x, propName);
                            WriteEnumerableItems(w, x, itemType, (IEnumerable)prop.GetValue(result));
                            WriteEndArray(x);
                        }
                        else
                        {
                            // fallback to DataContractSerializer
                            var s = CreateSerializer(prop.PropertyType);

                            WriteStartObject(x, propName);
                            s.WriteObjectContent(x, prop.GetValue(result));
                            WriteEndObject(x);
                        }
                    }

                    WriteEndRoot(x);
                    Flush(w, x);
                }
            }
        }

        private void WriteEnumerableItems(StreamWriter w,  XmlWriter x, Type itemType, IEnumerable items)
        {
            string name, ns;
            var s = CreateSerializer(itemType);
            StreamingListFormatter.ReflectClass(itemType, out name, out ns);
            
            var ienum = items.GetEnumerator();

            try
            {
                while (ienum.MoveNext())
                {
                    WriteStartArrayItem(x, name);
                    s.WriteObjectContent(x, ienum.Current);
                    WriteEndArrayItem(x);

                    Flush(w, x);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ienum != null && ienum is IDisposable)
                {
                    ((IDisposable)ienum).Dispose();
                }
            }
        }

        private void Flush(StreamWriter w, XmlWriter x)
        {
            x.Flush();

            // For some reason, a new line is necessary to force flushing the
            // output to the web server. It is likely in connection with chunked
            // HTTP response.
            w.WriteLine();
            w.Flush();
        }
    }
}
