using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Logging
{
    [TestClass]
    public class LogWriterFactoryTest
    {
        [TestMethod]
        public void GetLogWritersTest()
        {
            var f = new LogWriterFactory();
            var writers = f.GetLogWriters();
            Assert.AreEqual(5, writers.Length);
        }
    }
}
