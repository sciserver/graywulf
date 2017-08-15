using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    [Serializable]
    public class LoggingException : Exception
    {
        public LoggingException(string message)
            : base(message)
        {
        }
    }
}
