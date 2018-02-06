using System;
using System.Threading.Tasks;
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
        private ServiceModel.ServiceProxy<ICopyFile> GetFileCopy(CancellationContext cancellationContext, string name, bool remote, out string source, out string destination)
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

            source = String.Format(@"\\{0}\{1}\{2}.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);
            destination = String.Format(@"\\{0}\{1}\{2}_2.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name);

            return fc;
        }

        /// <summary>
        /// Copies a file from one UNC share to another, using the local machine.
        /// </summary>
        [TestMethod]
        public async Task ExecuteTest()
        {
            using (var cancellationContext = new CancellationContext())
            {
                using (var fc = GetFileCopy(cancellationContext, "FileCopyTest_ExecuteTest", false, out string source, out string destination))
                {
                    File.WriteAllText(source, "test data");

                    await fc.Value.ExecuteAsync(source, destination, true, FileCopyMethod.Win32FileCopy);

                    Assert.IsTrue(File.Exists(destination));

                    File.Delete(source);
                    File.Delete(destination);
                }
            }
        }

        /// <summary>
        /// Copies a file from one UNC share to another, using a remote machine.
        /// </summary>
        [TestMethod]
        public async Task RemoteExecuteTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                using (var cancellationContext = new CancellationContext())
                {
                    using (var fc = GetFileCopy(cancellationContext, "FileCopyTest_RemoteExecuteTest", true, out string source, out string destination))
                    {
                        File.WriteAllText(source, "test data");

                        await fc.Value.ExecuteAsync(source, destination, true, FileCopyMethod.Win32FileCopy);

                        Assert.IsTrue(File.Exists(destination));

                        File.Delete(source);
                        File.Delete(destination);
                    }
                }
            }
        }
    }
}
