using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Logging
{
    public enum LoggerStatus
    {
        Stopped,
        Started
    }

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
        RemoteService = 512,
        CommandLineTool = 1024,

        Test = 0x10000000,
        All = 0xFFFFFFFF,
    }

    [Flags]
    public enum EventSeverity : byte
    {
        None = 0,
        Debug = 1,      // debug messages
        Operation = 2,  // operation, such as web service method access
        Status = 4,     // important status change of services etc.
        Warning = 8,    // 
        Error = 16,
        All = 0xFF,
    }

    [Flags]
    public enum ExecutionStatus : byte
    {
        Unknown = 0,

        Initialized = 1,
        Executing = 2,
        Canceled = 4,
        Closed = 8,
        Compensating = 16,
        Faulted = 32,

        All = 0xFF,
    }

    public enum EventColumn
    {
        Id,
        UserGuid,
        JobGuid,
        SessionGuid,
        ContextGuid,
        Source,
        Severity,
        DateTime,
        Order,
        ExecutionStatus,
        Operation,
        Server,
        Client,
        Message,
        Exception,
        ExceptionType,
        StackTrace,
    }
}
