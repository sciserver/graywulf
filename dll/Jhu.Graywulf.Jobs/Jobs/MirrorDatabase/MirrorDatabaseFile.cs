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

namespace Jhu.Graywulf.Jobs.MirrorDatabase
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

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            Guid sourcefileguid = SourceFileGuid.Get(activityContext);
            Guid destinationdatabaseinstanceguid = DestinationDatabaseInstanceGuid.Get(activityContext);
            FileCopyDirection filecopydirection = FileCopyDirection.Get(activityContext);
            bool skipExistingFile = SkipExistingFile.Get(activityContext);

            string sourcefilename, destinationfilename;
            string hostname;

            // Load files
            using (RegistryContext context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                // Load destination database instance
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = destinationdatabaseinstanceguid;
                di.Load();
                di.LoadFileGroups(false);

                EntityGuid.Set(activityContext, di.Guid);

                // Math source file with destination file
                DatabaseInstanceFile df;

                DatabaseInstanceFile sf = new DatabaseInstanceFile(context);
                sf.Guid = sourcefileguid;
                sf.Load();

                EntityGuidFrom.Set(activityContext, sourcefileguid);

                sourcefilename = sf.GetFullUncFilename();

                DatabaseInstanceFileGroup fg = di.FileGroups[sf.DatabaseInstanceFileGroup.Name];
                fg.LoadFiles(false);
                df = fg.Files[sf.Name];

                EntityGuidTo.Set(activityContext, df.Guid);

                destinationfilename = df.GetFullUncFilename();

                DatabaseInstanceFile ssf = filecopydirection == Jhu.Graywulf.Registry.FileCopyDirection.Push ? sf : df;
                hostname = ssf.DatabaseInstanceFileGroup.DatabaseInstance.ServerInstance.Machine.HostName.ResolvedValue;
            }

            return delegate (JobContext asyncContext)
            {
                if (!skipExistingFile ||
                    !File.Exists(destinationfilename) ||
                    new FileInfo(sourcefilename).Length != new FileInfo(destinationfilename).Length)
                {
                    var fc = RemoteServiceHelper.CreateObject<ICopyFile>(hostname, true);

                    fc.Source = sourcefilename;
                    fc.Destination = destinationfilename;
                    fc.Overwrite = true;
                    fc.Method = FileCopyMethod.AsyncFileCopy;

                    asyncContext.RegisterCancelable(fc);
                    fc.Execute();
                    asyncContext.UnregisterCancelable(fc);
                }
            };
        }
    }
}
