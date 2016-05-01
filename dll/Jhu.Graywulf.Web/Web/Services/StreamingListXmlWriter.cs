using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Jhu.Graywulf.Web.Services
{
    class StreamingListXmlWriter : XmlTextWriter
    {
        private bool ignoreAttr = false;

        public StreamingListXmlWriter(TextWriter writer)
            : base(writer)
        {
        }

        public StreamingListXmlWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        public override string LookupPrefix(string ns)
        {
            return string.Empty;
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (String.Compare(prefix, "xmlns", true) == 0)
            {
                this.ignoreAttr = true;
                return;
            }
            
            base.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteEndAttribute()
        {
            if (this.ignoreAttr)
            {
                this.ignoreAttr = false;
                return;
            }
            
            base.WriteEndAttribute();
        }

        public override void WriteString(string text)
        {
            if (String.Compare(text, "http://www.w3.org/2001/XMLSchema-instance", true) == 0)
            {
                return;
            }
            
            base.WriteString(text);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            base.WriteStartElement(null, localName, null);
        }
    }
}
