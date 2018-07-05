using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class MultiPartIdentifierTest
    {
        [TestMethod]
        public void CreateSingleTest()
        {
            var mpi = MultiPartIdentifier.Create("test1");
            Assert.AreEqual("test1", mpi.Value);
        }

        [TestMethod]
        public void CreateDoubleTest()
        {
            var mpi = MultiPartIdentifier.Create("test1", "test2");
            Assert.AreEqual("test1.test2", mpi.Value);
        }

        [TestMethod]
        public void CreateTripleTest()
        {
            var mpi = MultiPartIdentifier.Create("test1", "test2", "test3");
            Assert.AreEqual("test1.test2.test3", mpi.Value);
        }

        [TestMethod]
        public void CreateQuadrupleTest()
        {
            var mpi = MultiPartIdentifier.Create("test1", "test2", "test3", "test4");
            Assert.AreEqual("test1.test2.test3.test4", mpi.Value);
        }
    }
}
