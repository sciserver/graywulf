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
using Jhu.Graywulf.Sql.Jobs.Query;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    [TestClass]
    public class SqlQueryTest : SqlQueryTestBase
    {
        // TODO: rewrite all these to use a TEST database instead of SkyQuery stuff

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            StartLogger();
            InitializeJobTests();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            CleanupJobTests();
            StopLogger();
        }

        [TestMethod]
        public void DiscoverParametersTest()
        {
            var jd = new JobDefinition();
            jd.WorkflowTypeName = Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(typeof(Jhu.Graywulf.Sql.Jobs.Query.SqlQueryJob));
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
        public void SqlQueryParametersSerializableTest()
        {
            var t = typeof(SqlQueryParameters);

            var sc = new SerializableChecker();
            Assert.IsTrue(sc.Execute(t));
        }

        #region Simple SQL query tests

        [TestMethod]
        [TestCategory("Query")]
        public void ConstantQueryTest()
        {
            var sql = "SELECT 1 INTO [$into]";

            RunQuery(sql);
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
        public void SimpleQueryWithoutIntoTest()
        {
            var sql = "SELECT TOP 10 objid, ra, dec FROM SDSSDR7:PhotoObj";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SimpleQueryWithPrimaryKeyTest()
        {
            var sql = "SELECT TOP 10 * INTO [$into] FROM TEST:SampleData_PrimaryKey";

            RunQuery(sql);
        }

        /// <summary>
        /// This is to test round-robin scheduling over a set of servers
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MultipleQueriesTest()
        {
            var sql = "SELECT TOP 10 * INTO [$into] FROM TEST:SampleData";

            for (int i = 0; i < 5; i++)
            {
                RunQuery(sql);
            }
        }
        
        /// <summary>
        /// Joins two tables of the same dataset.
        /// This one won't create a primary key on the target table because there's no
        /// unique combination of columns.
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void JoinQueryTest()
        {
            var sql = @"
SELECT TOP 10 p.objid, p.ra, p.dec, s.ra, s.dec
INTO [$into]
FROM SDSSDR7:PhotoPrimary p
INNER JOIN SDSSDR7:SpecObjAll s
    ON p.objID = s.bestObjID
ORDER BY p.objid";

            RunQuery(sql);
        }

        /// <summary>
        /// Joins two tables of the same dataset.
        /// This one does create a primary key on the target table.
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void JoinQueryTest2()
        {
            var sql = @"
SELECT TOP 10 p.objid, s.specObjID, p.ra, p.dec, s.ra, s.dec
INTO [$into]
FROM SDSSDR7:PhotoObj p
INNER JOIN SDSSDR7:SpecObjAll s
    ON p.objID = s.bestObjID";

            RunQuery(sql);
        }

        /// <summary>
        /// Joins two table from different mirrored datasets.
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void JoinQueryTest3()
        {
            var sql = @"
SELECT TOP 100 s.objID, g.ObjID
INTO [$into]
FROM SDSSDR7:PhotoObjAll s
CROSS JOIN Galex:PhotoObjAll g";

            RunQuery(sql);
        }

        /// <summary>
        /// Executes a self-join
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void SelfJoinQueryTest()
        {
            var sql = @"
SELECT TOP 100 a.objID, b.ObjID
INTO SqlQueryTest_SelfJoinQueryTest
FROM SDSSDR7:PhotoObjAll a
CROSS JOIN SDSSDR7:PhotoObjAll b";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void SimpleSelectStarQueryTest()
        {
            var sql = "SELECT TOP 10 * INTO [$into] FROM SDSSDR7:PhotoObj";

            RunQuery(sql);
        }


        [TestMethod]
        [TestCategory("Query")]
        public void AliasSelectStarQueryTest()
        {
            var sql = "SELECT TOP 10 p.* INTO [$into] FROM SDSSDR13:PhotoObj p";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void TableValuedFunctionTest()
        {
            var sql = "SELECT * INTO [$into] FROM htm.CoverCircleEq(0, 0, 10) AS htm";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void TableValuedFunctionJoinTest()
        {
            var sql = @"
SELECT TOP 100 objid, ra, dec
INTO [$into]
FROM htm.CoverCircleEq(0, 0, 10) htm
INNER JOIN SDSSDR7:PhotoObj p
    ON p.htmid BETWEEN htm.htmidstart AND htm.htmidend";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void TableValuedFunctionCrossApplyTest()
        {
            var sql = @"
SELECT htm.htmidstart, htm.htmidend
INTO [$into]
FROM (SELECT TOP 10 ra, dec FROM SDSSDR7:PhotoObj) p
CROSS APPLY htm.CoverCircleEq(p.ra, p.dec, 10) htm";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void ScalarFunctionTest()
        {
            var sql = @"
SELECT point.GetAngleEq(0, 0, 1, 1)
INTO [$into]";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void ScalarFunctionOnTableTest()
        {
            var sql = @"
SELECT TOP 100 point.GetAngleEq(0, 0, p.ra, p.dec) sep
INTO [$into]
FROM SDSSDR7:PhotoObj p";

            RunQuery(sql);
        }

        [TestMethod]
        [TestCategory("Query")]
        public void ScalarFunctionInWhereTest()
        {
            var sql = @"
SELECT p.objid, p.ra, p.dec
INTO [$into]
FROM (SELECT TOP 100 * FROM SDSSDR7:PhotoObj) p
WHERE point.GetAngleEq(0, 0, p.ra, p.dec) > 1000";

            RunQuery(sql);
        }

#endregion
#region MyDB query tests

        /// <summary>
        /// Executes a simple query on a single MyDB table
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MyDBTableQueryTest()
        {
            var sql = "SELECT TOP 10 objid INTO [$into] FROM MYDB:MyCatalog";
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

        /// <summary>
        /// Joins two MyDB tables
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MyDBTableJoinedQueryTest1()
        {
            var sql =
@"SELECT a.objid
INTO [$into]
FROM MYDB:MyCatalog a
INNER JOIN MYDB:MySDSSSample b ON a.ObjID = b.objID";

            RunQuery(sql);
        }

        /// <summary>
        /// Joins a MyDB table with a mirrored catalog table
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MyDBTableJoinedQueryTest2()
        {
            var sql =
@"SELECT a.objid
INTO [$into]
FROM SDSSDR7:PhotoObjAll a
INNER JOIN MYDB:MySDSSSample b ON a.ObjID = b.ObjID";

            RunQuery(sql);
        }

        /// <summary>
        /// Executes a self-join on a MyDB table
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MyDBSelfJoinQueryTest()
        {
            var sql = @"
SELECT TOP 100 a.objID, b.ObjID
INTO [$into]
FROM MyCatalog a
CROSS JOIN MyCatalog b";

            RunQuery(sql);
        }

        /// <summary>
        /// Executes a self-join on a MyDB table while also joining in a
        /// catalog table. This one is interesting because it can test name
        /// collision issues of cached remote tables.
        /// </summary>
        [TestMethod]
        [TestCategory("Query")]
        public void MyDBSelfJoinQueryTest2()
        {
            var sql = @"
SELECT TOP 100 p.ObjID, a.objID, b.ObjID
INTO [$into]
FROM SDSSDR7:PhotoObj p
CROSS JOIN MyCatalog a
CROSS JOIN MyCatalog b";

            RunQuery(sql);
        }

#endregion
#region Partitioned query tests

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
            q.Parameters.Destination.Options = Schema.TableInitializationOptions.Drop;

            // Should throw an exception because table isn't there
            q.Parameters.Destination.CheckTableExistence();
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
            q.Parameters.Destination.Options = Schema.TableInitializationOptions.Create;

            // Create destination table manually
            var table = q.Parameters.Destination.GetTable();
            table.Initialize(
                new Schema.Column[] {
                    new Schema.Column("ID", Schema.DataTypes.Int32)
                },
                Schema.TableInitializationOptions.Create);

            // Now check if it's there which should throw an exception
            q.Parameters.Destination.CheckTableExistence();
        }

#endregion
    }
}
