using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Tasks
{
    // TODO: move to tasks
    [TestClass]
    public class CancelableProcessTest : Jhu.Graywulf.Test.TestClassBase
    {
        [TestMethod]
        public void ExecuteProcessTest()
        {
            using (var cc = new CancellationContext())
            {
                var pinfo = new ProcessStartInfo()
                {
                    FileName = "ping.exe",
                    Arguments = "localhost",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                var cp = new CancelableProcess(cc, pinfo);

                var task = cp.ExecuteAsync();
                Util.TaskHelper.Wait(task);

                Assert.AreEqual(0, cp.ExitCode);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public void CancelProcessTest()
        {
            using (var cc = new CancellationContext())
            {
                var pinfo = new ProcessStartInfo()
                {
                    FileName = "ping.exe",
                    Arguments = "localhost -t",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                var cp = new CancelableProcess(cc, pinfo);

                var task = cp.ExecuteAsync();

                Thread.Sleep(5000);

                cc.Cancel();

                Util.TaskHelper.Wait(task);
            }
        }

        [TestMethod]
        public void ExecuteEseutilTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var pinfo = new ProcessStartInfo()
                {
                    FileName = GetTestFilePath(@"modules\graywulf\util\eseutil.exe"),
                    Arguments = "/y",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                var cp = new CancelableProcess(cancellationContext, pinfo);

                try
                {
                    Util.TaskHelper.Wait(cp.ExecuteAsync());
                }
                catch (Exception)
                {
                }

                Assert.AreEqual(-1003, cp.ExitCode);
            }
        }
    }
}
