using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Test.Jobs.Query
{
    public class QueryTestBase : TestClassBase
    {
        protected Guid ScheduleQueryJob(string query, QueueType queueType)
        {
            var queue = String.Format("QueueInstance:Graywulf.Controller.Controller.{0}", queueType.ToString());

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                SignInTestUser(context);

                var f = new SqlQueryFactory(context);

                var q = f.CreateQuery(query);
                var ji = f.ScheduleAsJob(q, queue, "testjob");

                ji.Save();

                return ji.Guid;
            }
        }
    }
}
