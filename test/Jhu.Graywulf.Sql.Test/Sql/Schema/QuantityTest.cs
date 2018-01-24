using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Schema
{
    [TestClass]
    public class QuantityTest: Jhu.Graywulf.Test.TestClassBase
    {
        [TestMethod]
        public void ParseQuantityTest()
        {
            var qString = "pos.cartesian.x; pos.eq; pos.frame=j2000";
            var quantity = Quantity.Parse(qString);

            Assert.AreEqual(qString, quantity.ToString());
        }
    }
}
