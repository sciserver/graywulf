using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Registry
{
    public class FederationContext
    {
        #region Private member variables

        private RegistryContext registryContext;
        private User registryUser;
        private GraywulfSchemaManager schemaManager;

        #endregion
        #region Properties

        public RegistryContext RegistryContext
        {
            get { return registryContext; }
        }

        public User RegistryUser
        {
            get { return registryUser; }
        }

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
                    schemaManager = GraywulfSchemaManager.Create(this);

                    if (registryUser != null)
                    {
                        schemaManager.AddUserDatabases(registryUser);
                    }
                }

                return schemaManager;
            }
        }

        #endregion
        #region Constructors and initializers

        public FederationContext(RegistryContext registryContext, User registryUser)
        {
            this.registryContext = registryContext;
            this.registryUser = registryUser;
        }

        private void InitializeMembers()
        {
            this.registryContext = null;
            this.registryUser = null;

            this.schemaManager = null;
        }

        #endregion
    }
}