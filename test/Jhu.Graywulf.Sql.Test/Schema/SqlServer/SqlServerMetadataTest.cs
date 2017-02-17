using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.Schema.SqlServer.Test
{
    [TestClass]
    public class SqlServerMetadataTest : SqlServerTestBase
    {

        [TestMethod]
        public void TableMetadataTest()
        {
            var ds = IOTestDataset;

            var t = ds.Tables[ds.DatabaseName, Jhu.Graywulf.Schema.SqlServer.Constants.DefaultSchemaName, "CatalogA"];

            // TODO: check metadata


            Assert.IsTrue(t.Columns["objId"].Metadata.Quantity.Parts.Count == 1);
            Assert.IsTrue(t.Columns["ra"].Metadata.Quantity.Parts.Count == 2); ;
            Assert.IsTrue(t.Columns["dec"].Metadata.Quantity.Parts.Count == 2);
            Assert.IsTrue(t.Columns["astroErr"].Metadata.Quantity.Parts.Count == 2);
            Assert.IsTrue(t.Columns["cx"].Metadata.Quantity.Parts.Count == 3);
            Assert.IsTrue(t.Columns["cy"].Metadata.Quantity.Parts.Count == 3);
            Assert.IsTrue(t.Columns["cz"].Metadata.Quantity.Parts.Count == 3);
            Assert.IsTrue(t.Columns["htmId"].Metadata.Quantity.Parts.Count == 3);
            Assert.IsTrue(t.Columns["mag_1"].Metadata.Quantity.Parts.Count == 1);
            Assert.IsTrue(t.Columns["mag_2"].Metadata.Quantity.Parts.Count == 1);
            Assert.IsTrue(t.Columns["mag_3"].Metadata.Quantity.Parts.Count == 1);
        }

        [TestMethod]
        public void QuantiesLoadTest()
        {
            var ds = IOTestDataset;
            var TVF = new TableValuedFunction(ds);

            Assert.IsTrue(TVF.Quantities.Count == 7);
        } 


    }
}
