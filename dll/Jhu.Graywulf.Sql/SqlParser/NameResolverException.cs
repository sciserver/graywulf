using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.SqlParser
{
    [Serializable]
    public class NameResolverException : Exception
    {
        private Token token;

        public Token Token
        {
            get { return token; }
            set { token = value; }
        }

        public NameResolverException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public NameResolverException()
            :base()
        {
            InitializeMembers();
        }

        public NameResolverException(string message)
            : base(message)
        {
            InitializeMembers();
        }

        public NameResolverException(string message, Exception innerException)
            :base(message, innerException)
        {
            InitializeMembers();
        }

        public NameResolverException(string message, Token token)
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
