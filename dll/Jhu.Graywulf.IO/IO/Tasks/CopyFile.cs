using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyFile))]
    public interface ICopyFile : IRemoteService
    {
        string Source
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }

        string Destination
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }

        bool Overwrite
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }
    }

    /// <summary>
    /// Implements a delegated, robust file copy by wrapping eseutil.exe
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults=true)]
    public class CopyFile : RemoteServiceBase, ICopyFile
    {
        #region Private members for property storage

        private string source;
        private string destination;
        private bool overwrite;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the source file path.
        /// </summary>
        public string Source
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return source; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the destination file path.
        /// </summary>
        public string Destination
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return destination; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { destination = value; }
        }

        /// <summary>
        /// Gets or sets whether the destination file should be overwritten.
        /// </summary>
        public bool Overwrite
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return overwrite; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { overwrite = value; }
        }

        #endregion
        #region Constructors and initializers

        public CopyFile()
        {
            InitializeMembers();
        }

        public CopyFile(string source, string destination, bool overwrite)
        {
            InitializeMembers();

            this.source = source;
            this.destination = destination;
            this.overwrite = overwrite;
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
            this.overwrite = false;
        }

        #endregion

        protected override void OnExecute()
        {
            // Check if file can be overwritten
            if (File.Exists(Destination))
            {
                if (!Overwrite)
                {
                    throw new IOException(ExceptionMessages.FileAlreadyExists);
                }
                else
                {
                    File.Delete(Destination);
                }
            }

            // Create destination folder
            if (!String.IsNullOrEmpty(Path.GetDirectoryName(destination)) && !Directory.Exists(Path.GetDirectoryName(destination)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
            }

            // Figure out the working directory from the service's exe
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Execute eseutil to perform copy
            var info = new ProcessStartInfo(
                Path.Combine(path, "eseutil.exe"),
                String.Format("/y \"{0}\" /d \"{1}\"", source, destination));

            // These are important to run program under the delegated account
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var guid = Guid.NewGuid();
            var cproc = new CancelableProcess(info);
            RegisterCancelable(guid, cproc);

            cproc.Execute();

            UnregisterCancelable(guid);

            if (cproc.IsCanceled || cproc.ExitCode == -1073741510)
            {
                throw new OperationCanceledException(ExceptionMessages.FileCopyCanceled);
            }
            else if (cproc.ExitCode > 0)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, cproc.ExitCode));
            }
        }
    }
}
