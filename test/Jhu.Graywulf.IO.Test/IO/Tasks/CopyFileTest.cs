using System;
using System.Threading;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    [DeploymentItem("ese.dll")]
    [DeploymentItem("eseutil.exe")]
    public class CopyFileTest : TestClassBase
    {
        private ICopyFile GetFileCopy(CancellationContext cancellationContext, string name, bool remote)
        {
            ICopyFile fc;
            if (remote)
            {
                fc = RemoteServiceHelper.CreateObject<ICopyFile>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                fc = new CopyFile(cancellationContext);
            }

            fc.Source = String.Format(@"\\{0}\{1}\{2}.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);
            fc.Destination = String.Format(@"\\{0}\{1}\{2}_2.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);

            return fc;
        }

        /// <summary>
        /// Copies a file from one UNC share to another, using the local machine.
        /// </summary>
        [TestMethod]
        public void ExecuteTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                var fc = GetFileCopy(cancellationContext, "FileCopyTest_ExecuteTest", false);
                fc.Overwrite = true;
                fc.Method = FileCopyMethod.Win32FileCopy;

                File.WriteAllText(fc.Source, "test data");

                fc.ExecuteAsync().Wait();

                Assert.IsTrue(File.Exists(fc.Destination));

                File.Delete(fc.Source);
                File.Delete(fc.Destination);
            }
        }

        /// <summary>
        /// Copies a file from one UNC share to another, using a remote machine.
        /// </summary>
        [TestMethod]
        public void RemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    var fc = GetFileCopy(cancellationContext, "FileCopyTest_RemoteExecuteTest", true);
                    fc.Overwrite = true;
                    fc.Method = FileCopyMethod.Win32FileCopy;

                    File.WriteAllText(fc.Source, "test data");

                    fc.ExecuteAsync().Wait();

                    Assert.IsTrue(File.Exists(fc.Destination));

                    File.Delete(fc.Source);
                    File.Delete(fc.Destination);
                }
            }
        }
    }
}
