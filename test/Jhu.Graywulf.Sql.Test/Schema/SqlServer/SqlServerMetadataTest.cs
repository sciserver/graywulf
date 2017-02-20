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

        protected override SqlServerDataset CreateIOTestDataset()
        {
            return new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);
        }

        [TestMethod]
        public void TableMetadataQuantityTest()
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
        public void QuantitiesLoadTest()
        {
            var ds = IOTestDataset;

            var tvf = ds.TableValuedFunctions["SqlInlineTableValuedFunction|TEST|Graywulf_Schema_Test|dbo|TestTableValuedFunction"];

            Assert.IsTrue(tvf.Quantities.Count == 0);
        }


    }
}
