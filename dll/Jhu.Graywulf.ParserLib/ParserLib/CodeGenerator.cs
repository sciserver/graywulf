using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.ParserLib
{
    public class CodeGenerator
    {
        private TextWriter writer;

        protected TextWriter Writer
        {
            get { return writer; }
        }

        public CodeGenerator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.writer = null;
        }

        public virtual void Execute(TextWriter writer, Node node)
        {
            this.writer = writer;
            WriteNode(node);
        }

        protected internal virtual bool WriteNode(Token node)
        {
            var res = node.AcceptCodeGenerator(this);

            if (res && node is Node)
            {
                WriteChildren((Node)node);
            }
            
            return res;
        }

        protected void WriteChildren(Node node)
        {
            foreach (var n in node.Nodes)
            {
                WriteNode(n);
            }
        }

        protected internal virtual bool WriteToken(Token t)
        {
            writer.Write(t.Value);
            return true;
        }
    }
}
