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
        private ICopyFile GetFileCopy(string name, bool remote)
        {
            ICopyFile fc;
            if (remote)
            {
                fc = RemoteServiceHelper.CreateObject<ICopyFile>(Test.Constants.Localhost);
            }
            else
            {
                fc = new CopyFile();
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
            var fc = GetFileCopy("FileCopyTest_ExecuteTest", false);

            File.WriteAllText(fc.Source, "test data");

            fc.Execute();

            Assert.IsTrue(File.Exists(fc.Destination));

            File.Delete(fc.Source);
            File.Delete(fc.Destination);
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
                var fc = GetFileCopy("FileCopyTest_RemoteExecuteTest", true);

                File.WriteAllText(fc.Source, "test data");

                fc.Execute();

                Assert.IsTrue(File.Exists(fc.Destination));

                File.Delete(fc.Source);
                File.Delete(fc.Destination);
            }
        }
    }
}
