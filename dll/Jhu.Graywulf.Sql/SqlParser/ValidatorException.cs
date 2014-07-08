using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser
{
    [Serializable]
    public class ValidatorException : Exception
    {
        private Token token;

        public Token Token
        {
            get { return token; }
            set { token = value; }
        }

        public ValidatorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ValidatorException()
            :base()
        {
            InitializeMembers();
        }

        public ValidatorException(string message)
            : base(message)
        {
            InitializeMembers();
        }

        public ValidatorException(string message, Exception innerException)
            :base(message, innerException)
        {
            InitializeMembers();
        }

        public ValidatorException(string message, Token token)
            :base(message)
        {
            this.token = token;
        }

        private void InitializeMembers()
        {
            this.token = null;
        }
    }
}
