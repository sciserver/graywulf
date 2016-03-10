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
    public class CopyTableTest : TestClassBase
    {
        private ICopyTable GetTableCopy(string tableName, bool remote)
        {
            ICopyTable q = null;
            if (remote)
            {
                q = RemoteServiceHelper.CreateObject<ICopyTable>(Test.Constants.Localhost, false);
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
                Query = "SELECT * FROM SampleData_PrimaryKey"
            };

            q.Source = source;

          
            var destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = tableName,
                Options = TableInitializationOptions.Create
            };

            q.Destination = destination;

            return q;
        }

        [TestMethod]
        public void ImportTableTest()
        {
            var table = GetTestUniqueName();
            var q = GetTableCopy(table, false);

            DropTable(q.Destination.GetTable());

            q.Execute();

            DropTable(q.Destination.GetTable());
        }

        [TestMethod]
        public void RemoteTableTest()
        {
            using (RemoteServiceTester.Instance.GetToken())
            {
                RemoteServiceTester.Instance.EnsureRunning();

                var table = GetTestUniqueName();
                var q = GetTableCopy(table, true);

                DropTable(q.Destination.GetTable());

                q.Execute();

                DropTable(q.Destination.GetTable());
            }
        }

    }
}
