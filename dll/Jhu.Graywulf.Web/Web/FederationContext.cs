using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web
{
    public class FederationContext
    {
        #region Private member variables

        private Context registryContext;
        private User registryUser;

        private GraywulfSchemaManager schemaManager;

        private DatabaseDefinition myDBDatabaseDefinition;
        private DatabaseVersion myDBDatabaseVersion;
        private DatabaseInstance myDBDatabaseInstance;
        private DatasetBase myDBDataset;

        #endregion

        public Cluster Cluster
        {
            get { return registryContext.Cluster; }
        }

        public Domain Domain
        {
            get { return registryContext.Domain; }
        }

        public Federation Federation
        {
            get { return registryContext.Federation; }
        }

        public FileFormatFactory FileFormatFactory
        {
            get { return FileFormatFactory.Create(registryContext.Federation.FileFormatFactory); }
        }

        public StreamFactory StreamFactory
        {
            get { return StreamFactory.Create(registryContext.Federation.StreamFactory); }
        }

        public DatabaseDefinition MyDBDatabaseDefinition
        {
            get
            {
                if (myDBDatabaseDefinition == null)
                {
                    myDBDatabaseDefinition = MyDBDatabaseVersion.DatabaseDefinition;
                }

                return myDBDatabaseDefinition;
            }
        }

        public DatabaseVersion MyDBDatabaseVersion
        {
            get
            {
                if (myDBDatabaseVersion == null)
                {
                    myDBDatabaseVersion = registryContext.Federation.MyDBDatabaseVersion;
                }

                return myDBDatabaseVersion;
            }
        }

        public DatabaseInstance MyDBDatabaseInstance
        {
            get
            {
                if (myDBDatabaseInstance == null)
                {
                    myDBDatabaseInstance = MyDBDatabaseVersion.GetUserDatabaseInstance(registryUser);
                }

                return myDBDatabaseInstance;
            }
        }

        /// <summary>
        /// Get a schema manager that provides access to the databases
        /// of the federation plus the user's mydb
        /// </summary>
        public GraywulfSchemaManager SchemaManager
        {
            get
            {
                if (schemaManager == null)
                {
                    schemaManager = new GraywulfSchemaManager(registryContext, Jhu.Graywulf.Registry.AppSettings.FederationName);

                    // Add custom datasets (MYDB)
                    var mydb = MyDBDatabaseInstance;

                    var mydbds = new Jhu.Graywulf.Schema.SqlServer.SqlServerDataset()
                    {
                        ConnectionString = mydb.GetConnectionString().ConnectionString,
                        Name = mydb.DatabaseDefinition.Name,
                        IsCacheable = false,
                        IsMutable = true,
                    };

                    schemaManager.Datasets[mydbds.Name] = mydbds;
                }

                return schemaManager;
            }
        }

        public Jhu.Graywulf.Schema.SqlServer.SqlServerDataset MyDBDataset
        {
            get
            {
                if (myDBDataset == null)
                {
                    myDBDataset = SchemaManager.Datasets[MyDBDatabaseDefinition.Name];
                }

                return (Jhu.Graywulf.Schema.SqlServer.SqlServerDataset)myDBDataset;
            }
        }

        public FederationContext(Context registryContext, User registryUser)
        {
            this.registryContext = registryContext;
            this.registryUser = registryUser;
        }

        private void InitializeMembers()
        {
            this.registryContext = null;
            this.registryUser = null;

            this.schemaManager = null;

            this.myDBDatabaseDefinition = null;
            this.myDBDatabaseVersion = null;
            this.myDBDatabaseInstance = null;
            this.myDBDataset = null;
        }
    }
}