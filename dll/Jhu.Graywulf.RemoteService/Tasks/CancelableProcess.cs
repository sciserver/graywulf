using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Tasks
{
    /// <summary>
    /// Implements a wrapper around a process to
    /// provide async execution and cancelation logic.
    /// </summary>
    public class CancelableProcess : CancelableTask
    {
        #region Private members

        private ProcessStartInfo info;
        private int exitCode;

        private Process process;

        #endregion
        #region Properties

        public ProcessStartInfo ProcessStartInfo
        {
            get { return info; }
            set { info = value; }
        }

        public int ExitCode
        {
            get { return exitCode; }
        }

        #endregion
        #region Constructors and initializers

        public CancelableProcess(ProcessStartInfo info)
        {
            InitializeMembers();

            this.info = info;
        }

        public CancelableProcess(CancellationContext cancellationContext, ProcessStartInfo info)
            : base(cancellationContext)
        {
            InitializeMembers();

            this.info = info;
        }

        private void InitializeMembers()
        {
            this.info = null;
        }

        #endregion

        /// <summary>
        /// Executes the process.
        /// </summary>
        /// <remarks>
        /// This method is called by the infrastructure.
        /// </remarks>
        protected override async Task OnExecuteAsync()
        {
            var tcs = new TaskCompletionSource<object>();

            process = new Process();
            process.StartInfo = info;
            process.EnableRaisingEvents = true;
            process.Exited += delegate (object sender, EventArgs args)
            {
                exitCode = process.ExitCode;

                if (IsCancellationRequested)
                {
                    tcs.TrySetCanceled();
                }
                else if (process.ExitCode == 0)
                {
                    tcs.TrySetResult(null);
                }
                else
                {
                    var ex = new Exception(String.Format("Process exited with error code {0}", process.ExitCode));
                    tcs.TrySetException(ex);
                }

                process.Dispose();
                process = null;
            };

            process.Start();

            await tcs.Task;
        }

        /// <summary>
        /// Cancels the operation by killing the process
        /// </summary>
        protected override void OnCancelling()
        {
            if (process != null)
            {
                process.Kill();
            }

            base.OnCancelling();
        }
    }
}
