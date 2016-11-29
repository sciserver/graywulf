using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public interface ISqlScriptJob : IJob
    {
        InArgument<SqlScriptParameters> Parameters { get; set; }
    }
}
