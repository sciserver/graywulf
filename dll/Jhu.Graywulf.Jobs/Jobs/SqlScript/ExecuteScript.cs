﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Data;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class ExecuteScript : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<SqlScriptParameters> Parameters { get; set; }

        [RequiredArgument]
        public InArgument<DatasetBase> Dataset { get; set; }

        [RequiredArgument]
        public InArgument<string> Script { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var parameters = Parameters.Get(activityContext);
            var dataset = Dataset.Get(activityContext);
            var script = Script.Get(activityContext);

            return delegate (AsyncJobContext asyncContext)
            {
                var cn = dataset.OpenConnection();
                var cmd = cn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = script;
                cmd.CommandTimeout = parameters.Timeout;
                cmd.Transaction = cn.BeginTransaction(parameters.IsolationLevel);

                var ccmd = new CancelableDbCommand(cmd);

                asyncContext.RegisterCancelable(ccmd);

                ccmd.ExecuteNonQuery();

                if (ccmd.Transaction != null)
                {
                    ccmd.Transaction.Commit();
                }

                asyncContext.UnregisterCancelable(ccmd);

                ccmd.Dispose();
            };
        }
    }
}
