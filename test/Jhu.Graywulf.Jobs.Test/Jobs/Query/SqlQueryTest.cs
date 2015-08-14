using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Test.Jobs.Query
{
    [TestClass]
    public class SqlQueryTest : SqlQueryTestBase
    {
        [TestMethod]
        public void DiscoverParametersTest()
        {

            var jd = new JobDefinition();
            jd.WorkflowTypeName = Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(typeof(Jhu.Graywulf.Jobs.Query.SqlQueryJob));
            jd.DiscoverWorkflowParameters();
        }

        [TestMethod]
        public void SqlQuerySerializableTest()
        {
            var t = typeof(SqlQuery);

            var sc = new SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SimpleQueryTest()
        {
            var sql = "SELECT TOP 10 * INTO [$into] FROM TEST:SampleData";
            
            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void MyDBJoinQueryTest()
        {
            var sql =
@"SELECT TOP 100 a.objid, a.ra, a.dec, b.ObjID
INTO [$into]
FROM TEST:SDSSDR7PhotoObjAll a
CROSS JOIN MyCatalog b
";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void PartitionedQueryTest()
        {
            var sql =
@"SELECT TOP 100 a.objid, a.ra, a.dec 
INTO [$into] 
FROM TEST:SDSSDR7PhotoObjAll a PARTITION BY a.ra
WHERE ra BETWEEN 0 AND 5 AND dec BETWEEN 0 AND 5";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void PartitionedQueryNoWhereTest()
        {
            var sql = 
@"SELECT TOP 100 a.objid, a.ra, a.dec 
INTO [$into] 
FROM TEST:SDSSDR7PhotoObjAll a PARTITION BY a.ra";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void PartitionedMyDBJoinQueryTest()
        {
            var sql =
@"SELECT TOP 100 a.objid, a.ra, a.dec, b.ObjID
INTO [$into]
FROM TEST:SDSSDR7PhotoObjAll a PARTITION BY a.ra
CROSS JOIN MyCatalog b
";

            RunQuery(sql);
        }
        
        [TestMethod]
        [TestCategory("Query")]
        [ExpectedException(typeof(IO.Tasks.TableCopyException))]
        public void CheckDestinationTableNewTest()
        {
            var testname = GetTestUniqueName();

            DropUserDatabaseTable(testname);

            var sql = "SELECT TOP 10 * INTO [$into] FROM TEST:SampleData";
            sql = sql.Replace("[$into]", testname);

            var q = CreateQuery(sql);
            q.Destination.Options = Schema.TableInitializationOptions.Drop;

            // Should throw an exception because table isn't there
            q.Destination.CheckTableExistence();
        }

        [TestMethod]
        [TestCategory("Query")]
        [ExpectedException(typeof(IO.Tasks.TableCopyException))]
        public void CheckDestinationTableExistingTest()
        {
            var testname = GetTestUniqueName();

            DropUserDatabaseTable(testname);

            var sql = "SELECT TOP 10 * INTO [$into] FROM TEST:SampleData";
            sql = sql.Replace("[$into]", testname);

            var q = CreateQuery(sql);
            q.Destination.Options = Schema.TableInitializationOptions.Create;

            // Create destination table manually
            var table = q.Destination.GetTable();
            table.Initialize(
                new Schema.Column[] {
                    new Schema.Column("ID", Schema.DataTypes.Int32)
                },
                Schema.TableInitializationOptions.Create);

            // Now check if it's there which should throw an exception
            q.Destination.CheckTableExistence();
        }
    }
}
