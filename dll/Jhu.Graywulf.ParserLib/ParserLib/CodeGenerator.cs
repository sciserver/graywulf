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

        /// <summary>
        /// Writes a node by visiting its children.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <remarks>
        /// Depending on the behavior of derived classes, this function
        /// might or might not traverse the parsing tree further.
        /// </remarks>
        protected virtual void WriteNode(Token token)
        {
            if (token is Node)
            {
                // Traverse tree
                WriteChildren((Node)token);
            }
            else
            {
                // Write terminal
                WriteToken(token);
            }
        }

        /// <summary>
        /// Writes all children of a parsing tree node
        /// </summary>
        /// <param name="node"></param>
        private void WriteChildren(Node node)
        {
            foreach (var n in node.Nodes)
            {
                WriteNode(n);
            }
        }

        /// <summary>
        /// Writes the value of a token to the stream.
        /// </summary>
        /// <param name="token"></param>
        /// <remarks>
        /// This function is called only for terminals
        /// </remarks>
        protected void WriteToken(Token token)
        {
            writer.Write(token.Value);
        }
    }
}
