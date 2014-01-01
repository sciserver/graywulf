using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void DelimitedTextDataFileSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Format.DelimitedTextDataFile);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void DelimitedTextDataFileBlockSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Format.DelimitedTextDataFileBlock);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void SqlServerNativeDataFileSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Format.SqlServerNativeDataFile);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        public void SqlServerNativeDataFileBlockSerializableTest()
        {
            var t = typeof(Jhu.Graywulf.Format.SqlServerNativeDataFileBlock);

            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }
    }
}
