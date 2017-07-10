using System;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class DropTable : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            var item = Item.Get(activityContext);
            var ds = item.Source.Dataset;
            var table = ds.Tables[ds.DatabaseName, item.Source.SchemaName, item.Source.ObjectName];

            table.Drop();
        }
    }
}
