using System;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class DropTable : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<CopyTablesItem> Item { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            var item = Item.Get(activityContext);
            var ds = item.Source.Dataset;
            var table = ds.Tables[ds.DatabaseName, item.Source.SourceSchemaName, item.Source.SourceObjectName];

            table.Drop();
        }
    }
}
