using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    class RawBodyWriter : BodyWriter
    {
        private RawMessageFormatterBase formatter;
        private string contentType;
        private object value;

        public RawBodyWriter(RawMessageFormatterBase formatter, string contentType, object value)
            : base(true)
        {
            this.formatter = formatter;
            this.contentType = contentType;
            this.value = value;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                formatter.WriteToStream(ms, contentType, value);
                var bytes = ms.ToArray();

                writer.WriteStartElement("Binary");
                writer.WriteBase64(bytes, 0, bytes.Length);
                writer.WriteEndElement();
            }
        }
    }
}
