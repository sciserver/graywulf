using System;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public interface IJobActivity
    {
        InArgument<JobInfo> JobInfo { get; set; }
    }
}
