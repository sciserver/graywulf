using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    [Serializable]
    public class ParserException : Exception
    {
        private int pos;
        private int line;
        private int col;

        public int Pos
        {
            get { return pos; }
            internal set { pos = value; }
        }

        public int Line
        {
            get { return line; }
            internal set { line = value; }
        }

        public int Col
        {
            get { return col; }
            internal set { col = value; }
        }

        public ParserException()
            : base()
        {
        }

        public ParserException(string message)
            :base(message)
        {
        }

        public ParserException(string message, Exception innerException)
            :base(message, innerException)
        {
        }

        public ParserException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
