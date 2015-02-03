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
            var ds = CreateTestDataset();

            var m = ds.Tables[ds.DatabaseName, "dbo", "Author"].Metadata;
        }
    }
}
