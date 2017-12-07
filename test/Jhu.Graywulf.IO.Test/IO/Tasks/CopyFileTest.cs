using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    [DeploymentItem("ese.dll")]
    [DeploymentItem("eseutil.exe")]
    public class CopyFileTest : TestClassBase
    {
        private ServiceModel.ServiceProxy<ICopyFile> GetFileCopy(string name, bool remote)
        {
            ServiceModel.ServiceProxy<ICopyFile> fc;
            if (remote)
            {
                fc = RemoteServiceHelper.CreateObject<ICopyFile>(Test.Constants.Localhost, false);
            }
            else
            {
                fc = new ServiceModel.ServiceProxy<ICopyFile>(new CopyFile());
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
            using (var fc = GetFileCopy("FileCopyTest_ExecuteTest", false))
            {
                fc.Value.Overwrite = true;
                fc.Value.Method = FileCopyMethod.Win32FileCopy;

                File.WriteAllText(fc.Value.Source, "test data");

                fc.Value.Execute();

                Assert.IsTrue(File.Exists(fc.Value.Destination));

                File.Delete(fc.Value.Source);
                File.Delete(fc.Value.Destination);
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

                using (var fc = GetFileCopy("FileCopyTest_RemoteExecuteTest", true))
                {
                    fc.Value.Overwrite = true;
                    fc.Value.Method = FileCopyMethod.Win32FileCopy;

                    File.WriteAllText(fc.Value.Source, "test data");

                    fc.Value.Execute();

                    Assert.IsTrue(File.Exists(fc.Value.Destination));

                    File.Delete(fc.Value.Source);
                    File.Delete(fc.Value.Destination);
                }
            }
        }
    }
}
