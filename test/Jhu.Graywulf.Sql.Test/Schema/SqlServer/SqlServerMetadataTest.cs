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
            
        }



    }
}
