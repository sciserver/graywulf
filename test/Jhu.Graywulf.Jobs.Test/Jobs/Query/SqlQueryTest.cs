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
            using (SchedulerTester.Instance.GetToken())
            {
                DropUserDatabaseTable("dbo", "SqlQueryTest_SimpleQueryTest");

                SchedulerTester.Instance.EnsureRunning();
                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    var sql = "SELECT TOP 10 * INTO SqlQueryTest_SimpleQueryTest FROM TEST:SampleData";

                    var guid = ScheduleQueryJob(sql, QueueType.Long);

                    WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                    var ji = LoadJob(guid);
                    Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                }
            }
        }

        [TestMethod]
        [TestCategory("Query")]
        [ExpectedException(typeof(IO.Tasks.TableCopyException))]
        public void CheckDestinationTableNewTest()
        {
            DropUserDatabaseTable("dbo", "SqlQueryTest_CheckDestinationTableTest");

            var sql = "SELECT TOP 10 * INTO SqlQueryTest_CheckDestinationTableTest FROM TEST:SampleData";
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
            DropUserDatabaseTable("dbo", "SqlQueryTest_CheckDestinationTableTest");

            var sql = "SELECT TOP 10 * INTO SqlQueryTest_CheckDestinationTableTest FROM TEST:SampleData";
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
