using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.IO.Jobs.CopyTables
{
    public interface ICopyTablesJob : IJob
    {
        InArgument<CopyTablesParameters> Parameters { get; set; }
    }
}
