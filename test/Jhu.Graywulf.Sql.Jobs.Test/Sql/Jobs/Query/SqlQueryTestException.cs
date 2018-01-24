using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [Serializable]
    public class SqlQueryTestException : Exception
    {
        private string site;
        private string stackTrace;

        public override string StackTrace
        {
            get { return this.stackTrace; }
        }

        public string Site
        {
            get { return site; }
        }

        public SqlQueryTestException()
        {
        }

        public SqlQueryTestException(string message, string site, string stackTrace)
            : base(message)
        {
            this.site = site;
            this.stackTrace = stackTrace;
        }
    }
}
