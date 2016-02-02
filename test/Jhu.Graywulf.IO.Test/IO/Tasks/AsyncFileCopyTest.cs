using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class AsyncFileCopyTest : TestClassBase
    {
        private void CreateTestFile(string path)
        {
            if (!File.Exists(path))
            {
                var buffer = new byte[0x100000];        // 1MB
                File.WriteAllBytes(path, buffer);
            }
        }

        private void DeleteTestFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void DefaultTest()
        {
            var name = GetTestUniqueName();

            var afc = new AsyncFileCopy()
            {
                Source = String.Format(@"\\{0}\{1}\{2}.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name),
                Destination = String.Format(@"\\{0}\{1}\{2}_2.txt", Test.Constants.RemoteHost1, Test.Constants.TestDirectory, name)
            };

            CreateTestFile(afc.Source);
            DeleteTestFile(afc.Destination);

            afc.Execute();
        }
    }
}
