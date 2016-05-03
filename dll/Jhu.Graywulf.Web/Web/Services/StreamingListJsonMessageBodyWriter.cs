using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    public class StreamingListJsonMessageBodyWriter : StreamingListXmlMessageBodyWriter
    {
        public StreamingListJsonMessageBodyWriter(object result)
            : base(result)
        {
            // overload
        }

        protected override System.Xml.XmlWriter CreateXmlWriter(Stream stream)
        {
            return JsonReaderWriterFactory.CreateJsonWriter(stream, System.Text.Encoding.UTF8);
        }

        protected override void WriteStartRoot(System.Xml.XmlWriter x, string name, string ns)
        {
            x.WriteStartElement("root");
            x.WriteAttributeString("type", "object");
        }

        protected override void WriteEndRoot(System.Xml.XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected override void WriteStartObject(System.Xml.XmlWriter x, string name)
        {
            x.WriteStartElement(name);
        }

        protected override void WriteEndObject(System.Xml.XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected override void WriteStartArray(System.Xml.XmlWriter x, string name)
        {
            x.WriteStartElement(name);
            x.WriteAttributeString("type", "array");
        }

        protected override void WriteEndArray(System.Xml.XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected override void WriteStartArrayItem(System.Xml.XmlWriter x, string name)
        {
            x.WriteStartElement("item");
        }

        protected override void WriteEndArrayItem(System.Xml.XmlWriter x)
        {
            x.WriteEndElement();
        }

        protected override XmlObjectSerializer CreateSerializer(Type type)
        {
            return new DataContractJsonSerializer(type);
        }
    }
}
