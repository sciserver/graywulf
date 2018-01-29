using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    public static class Error
    {
        public static InvalidOperationException SourceNull()
        {
            return new InvalidOperationException(ExceptionMessages.SourceNull);
        }

        public static InvalidOperationException DestinationNull()
        {
            return new InvalidOperationException(ExceptionMessages.DestinationNull);
        }

        public static InvalidOperationException StreamNull()
        {
            return new InvalidOperationException(ExceptionMessages.StreamNull);
        }

        public static TableCopyException FileNotArchine()
        {
            return new TableCopyException(ExceptionMessages.FileNotArchine);
        }
    }
}
