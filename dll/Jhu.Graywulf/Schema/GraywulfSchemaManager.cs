using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Implements a schema manager that works with a fully configured
    /// Graywulf cluster
    /// </summary>
    /// <remarks>
    /// List of datasets is taken from the SkyQuery database
    /// </remarks>
    public class GraywulfSchemaManager : SchemaManager
    {
        private static HashSet<string> ReservedDatabaseDefinitions = new HashSet<string>(SchemaManager.Comparer)
        {
            // *** TODO: Add these to the config file instead
            "MYDB", "Temp", "Code"
        };

        private Context context;
        private string federationName;

        public Context Context
        {
            get { return context; }
            set { context = value; }
        }

        //TODO: change to entityreference
        public string FederationName
        {
            get { return federationName; }
            set { federationName = value; }
        }

        public GraywulfSchemaManager(Context context, string federationName)
        {
            InitializeMembers();

            this.context = context;
            this.federationName = federationName;
        }

        private void InitializeMembers()
        {
        }

        protected override void OnDatasetAdded(string datasetName, DatasetBase ds)
        {
            // If a Graywulf dataset is added, it has to be updated and registry
            // objects loaded
            if (ds is GraywulfDataset)
            {
                var gwds = (GraywulfDataset)ds;
                var ef = new EntityFactory(Context);

                if (!String.IsNullOrEmpty(gwds.DatabaseInstanceName))
                {
                    var di = ef.LoadEntity<DatabaseInstance>(gwds.DatabaseInstanceName);

                    gwds.ConnectionString = di.GetConnectionString().ConnectionString;
                }
                else if (!String.IsNullOrEmpty(gwds.DatabaseDefinitionName))
                {
                    var dd = ef.LoadEntity<DatabaseDefinition>(gwds.DatabaseDefinitionName);

                    gwds.ConnectionString = dd.GetConnectionString().ConnectionString;
                }
            }
        }

        /// <summary>
        /// Loads a dataset from the registry based on the dataset name
        /// </summary>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        protected override DatasetBase LoadDataset(string datasetName)
        {
            var ef = new EntityFactory(context);
            var ddrd = ef.LoadEntity(federationName, datasetName);

            if (ddrd is DatabaseDefinition)
            {
                return DatasetFactory.CreateDataset((DatabaseDefinition)ddrd);
            }
            else if (ddrd is RemoteDatabase)
            {
                return DatasetFactory.CreateDataset((RemoteDatabase)ddrd);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected override IEnumerable<KeyValuePair<string, DatasetBase>> LoadAllDatasets()
        {
            var ef = new EntityFactory(context);
            var federation = ef.LoadEntity<Federation>(federationName);

            federation.LoadDatabaseDefinitions(true);

            // Load database definitions
            foreach (var dd in federation.DatabaseDefinitions.Values.Where(d => d.RunningState == RunningState.Running))
            {
                // Make sure it's not a reserved database definition
                if (!ReservedDatabaseDefinitions.Contains(dd.Name))
                {
                    var ds = DatasetFactory.CreateDataset(dd);

                    yield return new KeyValuePair<string, DatasetBase>(ds.Name, ds);
                }
            }

            federation.LoadRemoteDatabases(true);

            // Load remote databases
            foreach (var rd in federation.RemoteDatabases.Values.Where(d => d.RunningState == RunningState.Running))
            {
                // Make sure it's not a reserved database definition
                if (!ReservedDatabaseDefinitions.Contains(rd.Name))
                {
                    var ds = DatasetFactory.CreateDataset(rd);

                    yield return new KeyValuePair<string, DatasetBase>(ds.Name, ds);
                }
            }
        }


    }
}
