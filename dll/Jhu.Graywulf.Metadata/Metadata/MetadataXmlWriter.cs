using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Jhu.Graywulf.Metadata
{
    public class MetadataXmlWriter : XmlTextWriter
    {
        Stack<int> children;
        int indentLevel;
        string indentString = "  ";

        public MetadataXmlWriter(string filename)
            : base(filename, Encoding.UTF8)
        {
            Formatting = Formatting.None;
        }

        private void Indent()
        {
            WriteRaw("\r\n");

            for (int i = 0; i < indentLevel; i++)
            {
                WriteRaw(indentString);
            }
        }

        public override void WriteWhitespace(string ws)
        {
            // Omit whitespace, do pretty identation instead
        }

        public override void WriteString(string text)
        {
            if (text.IndexOf('\n') > -1)
            {
                string ii = "";

                for (int i = 0; i < indentLevel; i++)
                {
                    ii += indentString;
                }

                var sb = new StringBuilder(text.Trim());
                sb.Replace("\n", "\n" + ii);

                base.WriteString("\r\n" + ii + sb.ToString());
            }
            else
            {
                base.WriteString(text);
            }
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            base.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteStartDocument()
        {
            indentLevel = 0;
            children = new Stack<int>();
            children.Push(0);

            base.WriteStartDocument();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            int c = children.Pop();
            children.Push(c + 1);

            Indent();
            children.Push(0);

            base.WriteStartElement(prefix, localName, ns);

            indentLevel++;
        }

        public override void WriteEndElement()
        {
            WriteEndElementImpl();
            base.WriteEndElement();
        }

        public override void WriteFullEndElement()
        {
            WriteEndElementImpl();
            base.WriteFullEndElement();
        }

        private void WriteEndElementImpl()
        {
            indentLevel--;

            int c = children.Pop();

            if (c > 0)
            {
                Indent();
            }
        }
    }
}
