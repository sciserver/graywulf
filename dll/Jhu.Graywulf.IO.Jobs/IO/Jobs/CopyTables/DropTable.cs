using System;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Jobs.CopyTables
{
    public class DropTable : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            var item = Item.Get(activityContext);
            var ds = item.Source.Dataset;
            var table = ds.Tables[ds.DatabaseName, item.Source.Table.SchemaName, item.Source.Table.ObjectName];

            table.Drop();
        }
    }
}
