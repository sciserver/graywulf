using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.IO.Tasks
{
    public enum FileCopyMethod
    {
        Win32FileCopy,
        EseUtil,
        AsyncFileCopy,
        FastDataTransfer,
        Robocopy
    }

    public enum TableCopyStatus
    {
        Success,
        Skipped,
        Failed,
    }
}
