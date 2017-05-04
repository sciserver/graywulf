using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.Serialization;
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
        /// <summary>
        /// List of datasets that are handled separately
        /// </summary>
        private static HashSet<string> ReservedDatabaseDefinitions = new HashSet<string>(SchemaManager.Comparer)
        {
            Registry.Constants.UserDbName,
            Registry.Constants.CodeDbName,
            Registry.Constants.TempDbName,
        };

        private FederationContext federationContext;
        
        #region Constructors and initializers

        // TODO: add assigned server instance
        public GraywulfSchemaManager(FederationContext federationContext)
        {
            InitializeMembers(new StreamingContext());

            this.federationContext = federationContext;
        }

        public static GraywulfSchemaManager Create(FederationContext federationContext)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(federationContext.Federation.SchemaManager))
            {
                type = Type.GetType(federationContext.Federation.SchemaManager);
            }

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(GraywulfSchemaManager);
            }

            return (GraywulfSchemaManager)Activator.CreateInstance(type, new object[] { federationContext });
        }

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.federationContext = null;
        }

        #endregion

        protected override void OnDatasetAdded(string datasetName, DatasetBase ds)
        {
            // If a Graywulf dataset is added, it has to be updated and registry
            // objects loaded
            if (ds is GraywulfDataset)
            {
                var gwds = (GraywulfDataset)ds;
                gwds.Context = federationContext.RegistryContext;
            }

            base.OnDatasetAdded(datasetName, ds);
        }

        protected override IEnumerable<KeyValuePair<string, DatasetBase>> LoadAllDatasets()
        {
            federationContext.Federation.LoadDatabaseDefinitions(true);

            // Load database definitions
            foreach (var dd in federationContext.Federation.DatabaseDefinitions.Values.Where(d => d.RunningState == RunningState.Running))
            {
                // Make sure it's not a reserved database definition
                if (!ReservedDatabaseDefinitions.Contains(dd.Name))
                {
                    // This will retrieve the dataset from the cache or force loading it
                    var ds = Datasets[dd.Name];
                    yield return new KeyValuePair<string, DatasetBase>(ds.Name, ds);
                }
            }

            federationContext.Federation.LoadRemoteDatabases(true);

            // Load remote databases
            foreach (var rd in federationContext.Federation.RemoteDatabases.Values.Where(d => d.RunningState == RunningState.Running))
            {
                // Make sure it's not a reserved database definition
                if (!ReservedDatabaseDefinitions.Contains(rd.Name))
                {
                    var ds = CreateDataset(rd);

                    yield return new KeyValuePair<string, DatasetBase>(ds.Name, ds);
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
            var ef = new EntityFactory(federationContext.RegistryContext);
            var ddrd = ef.LoadEntity(EntityType.Unknown, federationContext.Federation.GetFullyQualifiedName(), datasetName);

            if (ddrd is DatabaseDefinition)
            {
                return CreateDataset((DatabaseDefinition)ddrd);
            }
            else if (ddrd is RemoteDatabase)
            {
                return CreateDataset((RemoteDatabase)ddrd);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        protected GraywulfDataset CreateDataset(DatabaseDefinition dd)
        {
            if (dd.RunningState != RunningState.Running)
            {
                throw new SchemaException(String.Format(ExceptionMessages.AccessDeniedToDataset, dd.Name));
            }

            GraywulfDataset ds = new GraywulfDataset(dd.Context);
            ds.Name = dd.Name;
            ds.DatabaseDefinitionReference.Value = dd;
            ds.IsCacheable = true;

            ds.CacheSchemaConnectionString();

            return ds;
        }

        protected DatasetBase CreateDataset(RemoteDatabase rd)
        {
            switch (rd.ProviderName)
            {
                case Schema.SqlServer.Constants.SqlServerProviderName:
                    return CreateSqlServerDataset(rd);
                case Schema.MySql.Constants.MySqlProviderName:
                    return CreateMySqlDataset(rd);
                case Schema.PostgreSql.Constants.PostgreSqlProviderName:
                    return CreatePostgreSqlDataset(rd);
                default:
                    throw new NotImplementedException();
            }
        }

        private Schema.SqlServer.SqlServerDataset CreateSqlServerDataset(RemoteDatabase rd)
        {
            var ds = new Schema.SqlServer.SqlServerDataset()
            {
                Name = rd.Name,
                IsOnLinkedServer = false,
                IsCacheable = true,
            };

            ds.ConnectionString = ds.GetSpecializedConnectionString(
                rd.ConnectionString,
                rd.IntegratedSecurity,
                rd.Username,
                rd.Password,
                false);

            return ds;
        }

        private Schema.MySql.MySqlDataset CreateMySqlDataset(RemoteDatabase rd)
        {
            var ds = new Schema.MySql.MySqlDataset()
            {
                Name = rd.Name,
                IsCacheable = true,
            };

            ds.ConnectionString = ds.GetSpecializedConnectionString(
                rd.ConnectionString,
                rd.IntegratedSecurity,
                rd.Username,
                rd.Password,
                false);

            return ds;
        }

        private Schema.PostgreSql.PostgreSqlDataset CreatePostgreSqlDataset(RemoteDatabase rd)
        {
            var ds = new Schema.PostgreSql.PostgreSqlDataset()
            {
                Name = rd.Name,
                IsCacheable = true,
                DefaultSchemaName = "public",
            };

            ds.ConnectionString = ds.GetSpecializedConnectionString(
                rd.ConnectionString,
                rd.IntegratedSecurity,
                rd.Username,
                rd.Password,
                false);

            return ds;
        }

        public void AddUserDatabases(User user)
        {
            var uf = UserDatabaseFactory.Create(federationContext);
            var mydbds = uf.GetUserDatabases(user);

            foreach (var key in mydbds.Keys)
            {
                this.Datasets[key] = mydbds[key];
            }
        }

        public IEnumerable<DatasetBase> EnumerateDatasets(bool includeUserDatabase, bool includeCodeDb)
        {
            var res = new List<DatasetBase>();

            if (includeUserDatabase)
            {
                foreach (var ds in Datasets.Values.Where(k => IsUserDb(k)).OrderBy(k => k.Name))
                {
                    res.Add(ds);
                }
            }

            foreach (var ds in Datasets.Values.Where(k => !IsSystemDataset(k)).OrderBy(k => k.Name))
            {
                res.Add(ds);
            }

            if (includeCodeDb)
            {
                res.Add(Datasets[Graywulf.Registry.Constants.CodeDbName]);
            }

            return res;
        }

        public bool IsUserDb(DatasetBase ds)
        {
            return ds.IsMutable;
        }

        public bool IsCodeDb(DatasetBase ds)
        {
            return Comparer.Compare(ds.Name, Graywulf.Registry.Constants.CodeDbName) == 0;
        }

        public bool IsSystemDataset(DatasetBase ds)
        {
            return Comparer.Compare(ds.Name, Graywulf.Registry.Constants.CodeDbName) == 0 ||
                   Comparer.Compare(ds.Name, Graywulf.Registry.Constants.TempDbName) == 0;
        }
    }
}
