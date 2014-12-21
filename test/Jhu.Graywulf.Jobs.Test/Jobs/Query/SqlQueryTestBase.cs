using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryTestBase : TestClassBase
    {
        protected virtual UserDatabaseFactory CreateUserDatabaseFactory(Context context)
        {
            return UserDatabaseFactory.Create(
                typeof(GraywulfUserDatabaseFactory).AssemblyQualifiedName,
                context);
        }

        protected virtual QueryFactory CreateQueryFactory(Context context)
        {
            return QueryFactory.Create(
                typeof(SqlQueryFactory).AssemblyQualifiedName,
                context);
        }

        protected Guid ScheduleQueryJob(string query, QueueType queueType)
        {
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());  // *** TODO

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var user = SignInTestUser(context);

                var udf = CreateUserDatabaseFactory(context);
                var mydb = udf.GetUserDatabase(user);

                var qf = CreateQueryFactory(context);
                var q = qf.CreateQuery(query);
                qf.AppendUserDatabase(q, mydb);

                q.Destination = new Jhu.Graywulf.IO.Tasks.DestinationTable()
                {
                    Dataset = mydb,
                    DatabaseName = mydb.DatabaseName,
                    SchemaName = mydb.DefaultSchemaName,
                    TableNamePattern = "testtable",     // will be overwritten by INTO queries
                    Options = TableInitializationOptions.Create | TableInitializationOptions.Drop
                };

                var ji = qf.ScheduleAsJob(null, q, queue, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }
    }
}
