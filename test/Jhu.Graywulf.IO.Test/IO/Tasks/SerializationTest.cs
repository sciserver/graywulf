using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void CopyFileSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.CopyFile);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void CopyTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.CopyTable);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ExportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.ExportTable);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ExportTableArchiveSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.ExportTableArchive);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ImportTableSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.ImportTable);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void ImportTableArchiveSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.IO.Tasks.ImportTableArchive);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }
    }
}
