using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public enum ConnectionMode
    {
        None,
        AutoOpen,
    }

    public enum TransactionMode
    {
        None,
        AutoCommit,
        ManualCommit,
        DirtyRead,
    }

    public enum DiagnosticMessageStatus
    {
        OK,
        Error
    }

    public enum DuplicateMergeMethod
    {
        Ignore,
        Update,
        Fail
    }
}
