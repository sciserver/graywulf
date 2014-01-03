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

        private bool TestType(Type type)
        {
            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            return sc.Execute(type);
        }

        [TestMethod]
        public void DelimitedTextDataFileSerializableTest()
        {
            Assert.IsTrue(TestType(typeof(DelimitedTextDataFile)));
            Assert.IsTrue(TestType(typeof(DelimitedTextDataFileBlock)));
        }

        [TestMethod]
        public void SqlServerNativeDataFileSerializableTest()
        {
            Assert.IsTrue(TestType(typeof(SqlServerNativeDataFile)));
            Assert.IsTrue(TestType(typeof(SqlServerNativeDataFileBlock)));
        }
    }
}
