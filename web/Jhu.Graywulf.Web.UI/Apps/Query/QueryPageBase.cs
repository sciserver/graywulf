﻿using System;
using System.Drawing;
using System.Web;
using System.Web.UI;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public class QueryPageBase : FederationPageBase
    {
        protected QueryPageBase()
            : base(false)
        {
        }

        /// <summary>
        /// Creates a query job from the query string.
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        protected QueryJob CreateQueryJob(string sql, JobQueue queue)
        {
            var queryJob = new QueryJob(sql)
            {
                Queue = queue
            };
            var query = queryJob.CreateQuery(FederationContext);
            query.Verify();

            return queryJob;
        }


        /// <summary>
        /// Schedules a query for execution.
        /// </summary>
        /// <param name="queryJob"></param>
        /// <returns></returns>
        protected JobInstance ScheduleQuery(QueryJob queryJob)
        {
            new JobsService(FederationContext).SubmitJob(queryJob);
            return queryJob.JobInstance;
        }
    }
}