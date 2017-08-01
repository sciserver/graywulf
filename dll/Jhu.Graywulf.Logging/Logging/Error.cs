using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    static class Error
    {
        public static LoggingException AsyncTimeout(LogWriterBase writer)
        {
            return new LoggingException(String.Format(ExceptionMessages.AsyncTimeout, writer.GetType().FullName));
        }
    }
}
