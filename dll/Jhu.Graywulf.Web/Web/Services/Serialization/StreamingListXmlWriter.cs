using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    class StreamingListXmlWriter : XmlTextWriter
    {
        private bool skipNamespace;
        private bool ignoreAttr;

        public bool SkipNamespace
        {
            get { return skipNamespace; }
            set { skipNamespace = value; }
        }

        public StreamingListXmlWriter(TextWriter writer)
            : base(writer)
        {
            InitializeMembers();
        }

        public StreamingListXmlWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.skipNamespace = false;
            this.ignoreAttr = false;
        }

        public override string LookupPrefix(string ns)
        {
            if (skipNamespace)
            {
                return string.Empty;
            }
            else
            {
                return base.LookupPrefix(ns);
            }
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if (skipNamespace && String.Compare(prefix, "xmlns", true) == 0)
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
            if (skipNamespace && String.Compare(text, "http://www.w3.org/2001/XMLSchema-instance", true) == 0)
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
