using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    [Flags]
    public enum ParameterDirection : byte
    {
        Unknown = 0,
        In = 1,
        Out = 2,
        InOut = In | Out
    }

    public enum AsyncQueueUnhandledExceptionLocation
    {
        BatchStart,
        BatchEnd,
        ItemProcessing
    }

    [Flags]
    public enum AmbientContextSupport : UInt32
    {
        None = 0,

        ThreadLocal = 1,
        AsyncLocal = 2,
        WebHttpContext = 4,
        WcfOperationContext = 8,

        Default = AsyncLocal | WebHttpContext | WcfOperationContext,

        All = 0xFFFF
    }
}
