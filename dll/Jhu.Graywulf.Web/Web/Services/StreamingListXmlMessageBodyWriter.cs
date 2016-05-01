using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    public class StreamingListXmlMessageBodyWriter : StreamBodyWriter
    {
        private IEnumerable items;
        private Type itemType;
        private Encoding encoding;

        public StreamingListXmlMessageBodyWriter(IEnumerable items, Type itemType)
            : this(items, itemType, System.Text.Encoding.ASCII)
        {
            // overload
        }

        public StreamingListXmlMessageBodyWriter(IEnumerable items, Type itemType, Encoding encoding)
            : base(false)
        {
            this.items = items;
            this.itemType = itemType;
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
            // Figure out list name

            // No attribute present
            // <ArrayOffootprint xmlns:i="http://www.w3.org/2001/XMLSchema-instance">

            // Inside class:
            // <footprintList xmlns="http://schemas.datacontract.org/2004/07/Jhu.Footprint.Web.Api.V1" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><footprints>
            // <footprint>....

            var s = new DataContractSerializer(itemType);
            var w = new StreamWriter(stream, encoding);
            var x = new StreamingListXmlWriter(w);

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
