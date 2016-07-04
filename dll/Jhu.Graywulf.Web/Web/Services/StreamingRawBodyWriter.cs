using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Web.Services
{
    class StreamingRawBodyWriter<T> : BodyWriter
    {
        private StreamingRawAdapter<T> adapter;
        private string contentType;
        private T value;

        public StreamingRawBodyWriter(StreamingRawAdapter<T> adapter, string contentType, T value)
            : base(true)
        {
            this.adapter = adapter;
            this.contentType = contentType;
            this.value = value;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            using (var ms = new MemoryStream())
            {
                adapter.WriteToStream(ms, contentType, value);
                var bytes = ms.ToArray();

                writer.WriteStartElement("Binary");
                writer.WriteBase64(bytes, 0, bytes.Length);
                writer.WriteEndElement();
            }
        }
    }
}
