using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Tasks
{
    [TestClass]
    public class TableCopyTest : TestClassBase
    {
        private ICopyTable GetTableCopy(string tableName, bool remote)
        {
            ICopyTable q = null;
            if (remote)
            {
                q = RemoteServiceHelper.CreateObject<ICopyTable>(Test.Constants.Localhost);
            }
            else
            {
                q = new CopyTable();
            }

            var ds = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.IOTestConnectionString)
            {
                IsMutable = true
            };

            var source = new SourceTableQuery()
            {
                Dataset = ds,
                Query = "SELECT 1 AS one, 2 AS two, 3 AS three"
            };

            q.Sources = new SourceTableQuery[] { source };

          
            var destination = new Jhu.Graywulf.Schema.Table()
            {
                Dataset = ds,
                SchemaName = "dbo",
                TableName = tableName
            };

            q.Destinations = new Schema.Table[] { destination };

            q.Options = TableInitializationOptions.Create;

            return q;
        }

        [TestMethod]
        public void ImportTableTest()
        {
            var q = GetTableCopy("TableCopyTest_ImportTableTest", false);

            q.Execute();

            DropTable(q.Destinations[0]);
        }

        [TestMethod]
        public void RemoteTableTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();
                var q = GetTableCopy("TableCopyTest_RemoteTableTest", true);

                q.Execute();

                DropTable(q.Destinations[0]);
            }
        }
    }
}
