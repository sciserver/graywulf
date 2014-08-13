using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public class TableCopyException : Exception
    {
        public TableCopyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
