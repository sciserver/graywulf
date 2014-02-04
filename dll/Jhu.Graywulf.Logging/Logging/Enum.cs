using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Logging
{
    [Flags]
    public enum EventSource : uint
    {
        None = 0,
        Scheduler = 1,
        Job = 2,
        Workflow = 4,
        Registry = 8,
        StoredProcedure = 16,
        UserCode = 32,
        WebUI = 64,
        WebAdmin = 128,
        WebService = 256,
        All = 0xFFFFFFFF,
    }

    [Flags]
    public enum EventSeverity : uint
    {
        None = 0,
        Status = 1,
        Warning = 2,
        Error = 4,
        All = 0xFFFFFFFF,
    }

    // Must conform to ActivityExecutionStatus in the WF foundation
    public enum ExecutionStatus : int
    {
        Unknown = -1,

        Initialized = 0,
        Executing = 1,
        Canceled = 2,
        Closed = 3,
        Compensating = 4,
        Faulted = 5,

        // ....
    }

    public enum EventColumn
    {
        EventId,
        UserGuid,
        JobGuid,
        ContextGuid,
        ParentContextGuid,
        EventSource,
        EventSeverity,
        EventDateTime,
        EventOrder,
        ExecutionStatus,
        Operation,
        EntityGuid,
        EntityGuidFrom,
        EntityGuidTo,
        ExceptionType,
        Message,
        StackTrace,
    }
}
