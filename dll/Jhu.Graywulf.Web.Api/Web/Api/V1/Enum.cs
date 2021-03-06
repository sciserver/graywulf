﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Flags]
    public enum JobType : int
    {
        Unknown = 0,
        Query = 1,
        Export = 2,
        Import = 4,
        Copy = 8,
        SqlScript = 16,
        All = 0xFFFF,
    }

    public enum JobStatus
    {
        Unknown,
        Waiting,
        Executing,
        Completed,
        Canceled,
        Failed,
        TimedOut,
    }

    [Flags]
    public enum JobQueue : int
    {
        Unknown = 0,
        Quick = 1,
        Long = 2,
        All = Quick | Long
    }

    public enum JobDependencyCondition : int
    {
        Unknown = 0,
        Completed = 8,
        Failed = 64,
        Cancelled = 512,
        TimedOut = 1024,
    }
}
