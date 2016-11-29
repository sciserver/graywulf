using System.Activities;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public interface ICopyTablesJob : IJob
    {
        InArgument<CopyTablesParameters> Parameters { get; set; }
    }
}
