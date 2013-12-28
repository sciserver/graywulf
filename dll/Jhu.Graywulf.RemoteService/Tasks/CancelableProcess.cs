using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO;

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
        protected override void OnExecute()
        {
            //info.CreateNoWindow = false;

            process = Process.Start(info);
            process.WaitForExit();

            exitCode = process.ExitCode;

            process = null;
        }

        /// <summary>
        /// Cancels the operation by killing the process
        /// </summary>
        public override void Cancel()
        {
            if (process != null)
            {
                process.Kill();
            }

            base.Cancel();
        }
    }
}
