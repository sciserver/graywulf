using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Schema
{
    [TestClass]
    public class QuantityIndexTest : Jhu.Graywulf.Test.TestClassBase
    {
        
        private List<Column> columns = new List<Column>()
            {
                new Column() { ID=0, ColumnName = "ra", Metadata = new VariableMetadata() { Quantity = Quantity.Parse("pos.eq.ra; pos.frame=j2000")} },
                new Column() { ID=1, ColumnName = "dec", Metadata = new VariableMetadata() { Quantity = Quantity.Parse("pos.eq.dec; pos.frame=j2000")} },
                new Column() { ID=2, ColumnName = "j_m_2mass", Metadata = new VariableMetadata() { Quantity = Quantity.Parse("phot.mag; em.IR.J; stat.max")} },
                new Column() { ID=3, ColumnName = "j_msig_2mass", Metadata = new VariableMetadata() { Quantity = Quantity.Parse("stat.error; phot.mag; em.IR.J")} }
            };

        [TestMethod]
        public void QuantityCreateTest()
        {

            var qi = QuantityIndex.Create(columns);

            Assert.IsTrue(qi.Count() == 7);

        }



        [TestMethod]
        public void SearchQuantityTest1()
        {
            var res = QuantityIndex.SearchQuantity(columns, "pos.frame=j2000");

            Assert.IsTrue(res[0].Name == "ra");
            Assert.IsTrue(res.Count == 2);
        }
        
        [TestMethod]
        [ExpectedException(typeof(SchemaException))]
        public void SearchQuantityTest2()
        {
            var res = QuantityIndex.SearchQuantity(columns, "pos.gal.l");
        }

    }
}
