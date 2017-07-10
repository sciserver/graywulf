using System;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class DropTable : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            var item = Item.Get(activityContext);
            var ds = item.Source.Dataset;
            var table = ds.Tables[ds.DatabaseName, item.Source.SchemaName, item.Source.ObjectName];

            table.Drop();
        }
    }
}
