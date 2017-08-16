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

    [Flags]
    public enum TransactionMode : int
    {
        None = 0,

        DirtyRead = 1,
        ReadOnly = 2,
        ReadWrite = 4,

        AutoCommit = 8,
        ManualCommit = 16,
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

    public enum Operation
    {
        Start,
        Stop,
        Deploy,
        Undeploy,
        Allocate,
        Attach,
        Detach,
        Drop,
        Discover
    }
}
