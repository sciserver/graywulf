using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestJob : Activity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }


    }
}
