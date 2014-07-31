using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Api
{
    public enum JobType
    {
        Unknown,
        Query,
        Export,
        Import
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

    public enum JobQueue
    {
        Unknown,
        Any,
        Quick,
        Long
    }
}
