using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace Jhu.Graywulf.Tasks
{
    public class CancelableProcess : CancelableTask
    {
        private ProcessStartInfo info;
        private int exitCode;

        private Process process;

        public ProcessStartInfo ProcessStartInfo
        {
            get { return info; }
            set { info = value; }
        }

        public int ExitCode
        {
            get { return exitCode; }
        }

        public CancelableProcess(ProcessStartInfo info)
        {
            InitializeMembers();

            this.info = info;
        }

        private void InitializeMembers()
        {
            this.info = null;
        }

        protected override void OnExecute()
        {
            info.CreateNoWindow = false;

            process = Process.Start(info);
            process.WaitForExit();

            exitCode = process.ExitCode;

            process = null;
        }

        public override void Cancel()
        {
            base.Cancel();

            if (process != null)
            {
                process.Kill();
            }
        }
    }
}
