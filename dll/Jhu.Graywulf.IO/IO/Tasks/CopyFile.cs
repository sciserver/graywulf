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

        FileCopyMethod Method
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
        private FileCopyMethod method;

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

        public FileCopyMethod Method
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return method; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { method = value; }
        }

        #endregion
        #region Constructors and initializers

        public CopyFile()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.source = null;
            this.destination = null;
            this.overwrite = false;
            this.method = FileCopyMethod.AsyncFileCopy;
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

            switch (method)
            {
                case FileCopyMethod.Win32FileCopy:
                    ExecuteWin32FileCopy();
                    break;
                case FileCopyMethod.AsyncFileCopy:
                    ExecuteAsyncFileCopy();
                    break;
                case FileCopyMethod.EseUtil:
                    ExecuteEseUtil();
                    break;
                case FileCopyMethod.Robocopy:
                    ExecuteRobocopy();
                    break;
                case FileCopyMethod.FastDataTransfer:
                default:
                    throw new NotImplementedException();
            }
        }

        private void ExecuteWin32FileCopy()
        {
            File.Copy(source, destination, true);
        }

        private void ExecuteAsyncFileCopy()
        {
            var afc = new AsyncFileCopy()
            {
                Source = source,
                Destination = destination,
            };

            var guid = Guid.NewGuid();
            RegisterCancelable(guid, afc);
            afc.Execute();
            UnregisterCancelable(guid);
        }

        private void ExecuteEseUtil()
        {
            // Figure out the working directory from the service's exe
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Execute eseutil to perform copy
            var info = new ProcessStartInfo(
                Path.Combine(path, "eseutil.exe"),
                String.Format("/y \"{0}\" /d \"{1}\"", source, destination));

            var exit = ExecuteProcess(info);

            if (exit > 0)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, exit));
            }
        }

        private void ExecuteRobocopy()
        {
            // Execute robocopy to perform copy
            var info = new ProcessStartInfo(
                "robocopy.exe",
                String.Format(
                    "\"{0}\" \"{1}\" \"{2}\" /Z /MT /R:1",
                    Path.GetDirectoryName(source), 
                    Path.GetDirectoryName(destination),
                    Path.GetFileName(source)));

            var exit = ExecuteProcess(info);

            if (exit != 1)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, exit));
            }
            
        }

        private int ExecuteProcess(ProcessStartInfo info)
        {
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
            else 
            {
                return cproc.ExitCode;
            }
        }
    }
}
