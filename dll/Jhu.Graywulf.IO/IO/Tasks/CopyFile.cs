using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CopyFile))]
    public interface ICopyFile : IRemoteService
    {
        [OperationContract(Name = "ExecuteAsyncEx")]
        Task ExecuteAsyncEx(string source, string destination, bool overwrite, FileCopyMethod method);
    }

    /// <summary>
    /// Implements a delegated, robust file copy by wrapping eseutil.exe
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
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
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets or sets the destination file path.
        /// </summary>
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        /// <summary>
        /// Gets or sets whether the destination file should be overwritten.
        /// </summary>
        public bool Overwrite
        {
            get { return overwrite; }
            set { overwrite = value; }
        }

        public FileCopyMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        #endregion
        #region Constructors and initializers

        public CopyFile()
        {
            InitializeMembers();
        }

        public CopyFile(CancellationContext cancellationContext)
            : base(cancellationContext)
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

        [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
        [LimitedAccessOperation(RemoteService.Constants.DefaultRole)]
        public Task ExecuteAsyncEx(string source, string destination, bool overwrite, FileCopyMethod method)
        {
            this.source = source;
            this.destination = destination;
            this.overwrite = overwrite;
            this.method = method;

            return ExecuteAsync();
        }

        protected override async Task OnExecuteAsync()
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
                    await ExecuteWin32FileCopyAsync();
                    break;
                case FileCopyMethod.AsyncFileCopy:
                    await ExecuteAsyncFileCopyAsync();
                    break;
                case FileCopyMethod.EseUtil:
                    await ExecuteEseUtilAsync();
                    break;
                case FileCopyMethod.Robocopy:
                    await ExecuteRobocopyAsync();
                    break;
                case FileCopyMethod.FastDataTransfer:
                default:
                    throw new NotImplementedException();
            }
        }

        private Task ExecuteWin32FileCopyAsync()
        {
            return Task.Factory.StartNew(() => File.Copy(source, destination, true));
        }

        private async Task ExecuteAsyncFileCopyAsync()
        {
            var afc = new AsyncFileCopy(CancellationContext)
            {
                Source = source,
                Destination = destination,
            };

            await afc.ExecuteAsync();
        }

        private async Task ExecuteEseUtilAsync()
        {
            // Figure out the working directory from the service's exe
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // Execute eseutil to perform copy
            var info = new ProcessStartInfo(
                Path.Combine(path, "eseutil.exe"),
                String.Format("/y \"{0}\" /d \"{1}\"", source, destination));

            var exit = await ExecuteProcess(info);

            if (exit > 0)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, exit));
            }
        }

        private async Task ExecuteRobocopyAsync()
        {
            // Execute robocopy to perform copy
            var info = new ProcessStartInfo(
                "robocopy.exe",
                String.Format(
                    "\"{0}\" \"{1}\" \"{2}\" /Z /MT /R:1",
                    Path.GetDirectoryName(source),
                    Path.GetDirectoryName(destination),
                    Path.GetFileName(source)));

            var exit = await ExecuteProcess(info);

            if (exit != 1)
            {
                throw new Exception(String.Format(ExceptionMessages.FileCopyFailed, exit));
            }

        }

        private async Task<int> ExecuteProcess(ProcessStartInfo info)
        {
            // These are important to run program under the delegated account
            info.UseShellExecute = false;
            info.CreateNoWindow = true;

            var guid = Guid.NewGuid();
            var cproc = new CancelableProcess(CancellationContext, info);

            try
            {
                await cproc.ExecuteAsync();
            }
            catch (Exception)
            {

            }

            if (cproc.IsCancellationRequested || cproc.ExitCode == -1073741510)
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
