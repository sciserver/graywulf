using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Tasks
{
    [TestClass]
    public class CancelableProcessTest
    {
        [TestMethod]
        public void CancelProcessTest()
        {
            var pinfo = new ProcessStartInfo()
            {
                FileName = "ping.exe",
                Arguments = "localhost -t",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var cp = new CancelableProcess(pinfo);

            cp.BeginExecute();

            Thread.Sleep(5000);

            cp.Cancel();

            cp.EndExecute();

            Assert.AreEqual(-1, cp.ExitCode);
        }

        [TestMethod]
        public void ExecuteEseutilTest()
        {
            var pinfo = new ProcessStartInfo()
            {
                FileName = @"..\..\..\graywulf\util\eseutil.exe",
                Arguments = "/y",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var cp = new CancelableProcess(pinfo);

            cp.Execute();

            Assert.AreEqual(-1003, cp.ExitCode);
        }
    }
}
