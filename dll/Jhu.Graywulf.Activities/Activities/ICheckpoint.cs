using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public interface ICheckpoint
    {
        [RequiredArgument]
        InArgument<string> CheckpointName { get; set; }
    }
}
