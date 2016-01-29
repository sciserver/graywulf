using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class FindSourcesAndDestinations : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid[]> SourceDatabaseInstanceGuids { get; set; }

        [RequiredArgument]
        public InArgument<Guid[]> DestinationDatabaseInstanceGuids { get; set; }

        [RequiredArgument]
        public OutArgument<Queue<Guid>> SourceDatabaseQueue { get; set; }
        [RequiredArgument]
        public OutArgument<Queue<Guid>> DestinationDatabaseQueue { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            var sourceDatabaseInstanceGuids = SourceDatabaseInstanceGuids.Get(activityContext);
            var destinationDatabaseInstanceGuids = DestinationDatabaseInstanceGuids.Get(activityContext);

            SourceDatabaseQueue.Set(activityContext, new Queue<Guid>(sourceDatabaseInstanceGuids));
            DestinationDatabaseQueue.Set(activityContext, new Queue<Guid>(destinationDatabaseInstanceGuids));

            if (sourceDatabaseInstanceGuids.Length == 0 ||
                destinationDatabaseInstanceGuids.Length == 0)
            {
                throw new InvalidOperationException(ExceptionMessages.NoDatabasesToCopy);
            }
        }
    }
}
