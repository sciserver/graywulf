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
        private ServiceModel.ServiceProxy<ICopyFile> GetFileCopy(CancellationContext cancellationContext, string name, bool remote)
        {
            ServiceModel.ServiceProxy<ICopyFile> fc;
            if (remote)
            {
                fc = RemoteServiceHelper.CreateObject<ICopyFile>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                fc = new ServiceModel.ServiceProxy<ICopyFile>(new CopyFile(cancellationContext));
            }

            fc.Value.Source = String.Format(@"\\{0}\{1}\{2}.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);
            fc.Value.Destination = String.Format(@"\\{0}\{1}\{2}_2.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);

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
                using (var fc = GetFileCopy(cancellationContext, "FileCopyTest_ExecuteTest", false))
                {
                    fc.Value.Overwrite = true;
                    fc.Value.Method = FileCopyMethod.Win32FileCopy;

                    File.WriteAllText(fc.Value.Source, "test data");

                    fc.Value.ExecuteAsync().Wait();

                    Assert.IsTrue(File.Exists(fc.Value.Destination));

                    File.Delete(fc.Value.Source);
                    File.Delete(fc.Value.Destination);
                }
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
                    using (var fc = GetFileCopy(cancellationContext, "FileCopyTest_RemoteExecuteTest", true))
                    {
                        fc.Value.Overwrite = true;
                        fc.Value.Method = FileCopyMethod.Win32FileCopy;

                        File.WriteAllText(fc.Value.Source, "test data");

                        fc.Value.ExecuteAsync().Wait();

                        Assert.IsTrue(File.Exists(fc.Value.Destination));

                        File.Delete(fc.Value.Source);
                        File.Delete(fc.Value.Destination);
                    }
                }
            }
        }
    }
}
