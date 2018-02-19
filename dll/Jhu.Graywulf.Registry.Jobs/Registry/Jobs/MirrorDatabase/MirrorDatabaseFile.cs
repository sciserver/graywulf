using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Registry.Jobs.MirrorDatabase
{
    public class MirrorDatabaseFile : JobAsyncCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }
        public OutArgument<Guid> EntityGuidFrom { get; set; }
        public OutArgument<Guid> EntityGuidTo { get; set; }

        [RequiredArgument]
        public InArgument<Guid> SourceFileGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> DestinationDatabaseInstanceGuid { get; set; }
        [RequiredArgument]
        public InArgument<FileCopyDirection> FileCopyDirection { get; set; }
        [RequiredArgument]
        public InArgument<bool> SkipExistingFile { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            Guid sourcefileguid = SourceFileGuid.Get(activityContext);
            Guid destinationdatabaseinstanceguid = DestinationDatabaseInstanceGuid.Get(activityContext);
            FileCopyDirection filecopydirection = FileCopyDirection.Get(activityContext);
            bool skipExistingFile = SkipExistingFile.Get(activityContext);

            string sourcefilename, destinationfilename;
            string hostname;

            // Load files
            using (RegistryContext context = ContextManager.Instance.CreateReadOnlyContext())
            {
                var ef = new EntityFactory(context);

                // Load destination database instance
                var di = ef.LoadEntity<DatabaseInstance>(destinationdatabaseinstanceguid);
                di.LoadFileGroups(false);

                // Math source file with destination file
                DatabaseInstanceFile df;
                var sf = ef.LoadEntity<DatabaseInstanceFile>(sourcefileguid);

                DatabaseInstanceFileGroup fg = di.FileGroups[sf.DatabaseInstanceFileGroup.Name];
                fg.LoadFiles(false);
                df = fg.Files[sf.Name];

                DatabaseInstanceFile ssf = filecopydirection == Jhu.Graywulf.Registry.FileCopyDirection.Push ? sf : df;
                hostname = ssf.DatabaseInstanceFileGroup.DatabaseInstance.ServerInstance.Machine.HostName.ResolvedValue;

                sourcefilename = sf.GetFullUncFilename();
                destinationfilename = df.GetFullUncFilename();

                EntityGuid.Set(activityContext, di.Guid);
                EntityGuidFrom.Set(activityContext, sourcefileguid);
                EntityGuidTo.Set(activityContext, df.Guid);
            }

            if (!skipExistingFile ||
                !File.Exists(destinationfilename) ||
                new FileInfo(sourcefilename).Length != new FileInfo(destinationfilename).Length)
            {
                using (var fc = RemoteServiceHelper.CreateObject<ICopyFile>(cancellationContext, hostname, true))
                {
                    await fc.Value.ExecuteAsyncEx(sourcefilename, destinationfilename, true, FileCopyMethod.AsyncFileCopy);
                }
            }
        }
    }
}
