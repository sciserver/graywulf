﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.IO.Jobs.ImportTables
{
    public class ImportTablesJob : JobAsyncCodeActivity, IJobActivity, IImportTablesJob
    {
        [RequiredArgument]
        public InArgument<ImportTablesParameters> Parameters { get; set; }

        protected override async Task OnExecuteAsync(AsyncCodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var parameters = Parameters.Get(activityContext);

            // Make sure connection strings are cached
            using (var context = ContextManager.Instance.CreateReadOnlyContext())
            {
                for (int i = 0; i < parameters.Destinations.Length; i++)
                {
                    if (parameters.Destinations[i].Dataset is GraywulfDataset)
                    {
                        var gwds = (GraywulfDataset)parameters.Destinations[i].Dataset;
                        gwds.RegistryContext = context;
                        gwds.CacheSchemaConnectionString();
                    }
                }
            }

            // Create table importer
            using (var importer = parameters.GetInitializedTableImportTask(cancellationContext))
            {
                try
                {
                    await importer.Value.OpenAsync();
                    await importer.Value.ExecuteAsync();
                }
                finally
                {
                    importer.Value.Close();
                }
            }
        }
    }
}