using System;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public interface IGraywulfActivity
    {
        InArgument<JobInfo> JobInfo { get; set; }
    }
}
