using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Sql.Jobs.SqlScript
{
    public interface ISqlScriptJob : IJob
    {
        InArgument<SqlScriptParameters> Parameters { get; set; }
    }
}
