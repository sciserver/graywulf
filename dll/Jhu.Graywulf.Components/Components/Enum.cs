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
    public enum AmbientContextStoreLocation : UInt32
    {
        GlobalStatic = 0x0001,
        ThreadLocal = 0x0002,
        AsyncLocal = 0x0004,
        WebHttpContext = 0x0008,
        WcfOperationContext = 0x0010,

        Default = AsyncLocal,
        All = 0xFFFF
    }
}
