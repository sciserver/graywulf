using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class FederationInstaller : InstallerBase
    {
        #region Private member variables

        private Cluster cluster;
        private Domain domain;
        private Federation federation;

        #endregion
        #region Properties

        protected Cluster Cluster
        {
            get { return cluster; }
        }

        protected Domain Domain
        {
            get { return domain; }
        }

        protected Federation Federation
        {
            get { return federation; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Initializes the class to create a new federation
        /// within a domain.
        /// </summary>
        /// <param name="domain"></param>
        public FederationInstaller(Domain domain)
            : base(domain.RegistryContext)
        {
            InitializeMembers();

            this.cluster = domain.Cluster;
            this.domain = domain;
        }

        /// <summary>
        /// Initializes the class to manage an existing federation.
        /// </summary>
        /// <param name="federation"></param>
        public FederationInstaller(Federation federation)
            : base(federation.RegistryContext)
        {
            InitializeMembers();

            this.cluster = federation.Domain.Cluster;
            this.domain = federation.Domain;
            this.federation = federation;
        }

        private void InitializeMembers()
        {
            this.cluster = null;
            this.domain = null;
            this.federation = null;
        }

        #endregion

        /// <summary>
        /// Loads the default database version for the TEMP database
        /// </summary>
        /// <returns></returns>
        protected virtual DatabaseVersion GetTempDatabaseVersion()
        {
            cluster.LoadMachineRoles(false);

            cluster.LoadDomains(false);
            var shareddomain = cluster.Domains[Constants.SystemDomainName];

            shareddomain.LoadFederations(false);
            var sharedfederation = shareddomain.Federations[Constants.SystemFederationName];

            sharedfederation.LoadDatabaseDefinitions(false);

            var tempdbdd = sharedfederation.DatabaseDefinitions[Constants.TempDbName];
            tempdbdd.LoadDatabaseVersions(false);

            var tempDatabaseVersion = tempdbdd.DatabaseVersions[Constants.TempDbName];

            return tempDatabaseVersion;
        }

        protected virtual MachineRole GetControllerMachineRole()
        {
            var controllerRole = cluster.MachineRoles[Constants.ControllerMachineRoleName];
            controllerRole.LoadServerVersions(false);
            controllerRole.LoadMachines(false);
            return controllerRole;
        }

        protected virtual ServerVersion GetNodeServerVersion()
        {
            cluster.LoadMachineRoles(true);
            var nodeRole = cluster.MachineRoles[Constants.NodeMachineRoleName];
            nodeRole.LoadServerVersions(false);
            var nodeServerVersion = nodeRole.ServerVersions[Constants.ServerVersionName];
            return nodeServerVersion;
        }

        protected virtual ServerVersion GetUserDatabaseServerVersion()
        {
            return GetNodeServerVersion();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <remarks>
        /// This function is primarily used by the test routine, as federations are
        /// not often created with default settings
        /// </remarks>
        public Federation Install(string name)
        {
            GenerateCluster();

            var tempDatabaseVersion = GetTempDatabaseVersion();
            var controllerRole = GetControllerMachineRole();
            var nodeServerVersion = GetNodeServerVersion();
            var userDatabaseServerVersion = GetUserDatabaseServerVersion();

            GenerateFederation(name, controllerRole, tempDatabaseVersion);
            GenerateDefaultSettings();
            GenerateDefaultChildren(userDatabaseServerVersion, nodeServerVersion);

            return federation;
        }

        public Federation Install(
            string name,
            MachineRole controllerRole,
            DatabaseVersion tempDatabaseVersion,
            ServerVersion userDatabaseServerVersion,
            ServerVersion nodeServerVersion)
        {
            GenerateCluster();
            GenerateFederation(name, controllerRole, tempDatabaseVersion);
            GenerateDefaultSettings();
            GenerateDefaultChildren(userDatabaseServerVersion, nodeServerVersion);

            return federation;
        }

        protected virtual void GenerateCluster()
        {
        }

        protected virtual void GenerateFederation(
            string name,
            MachineRole controllerMachineRole,
            DatabaseVersion tempDatabaseVersion)
        {
            federation = new Federation(domain)
            {
                Name = name,
                ControllerMachineRole = controllerMachineRole,
                TempDatabaseVersion = tempDatabaseVersion,
            };

            federation.Save();
        }

        public virtual void GenerateDefaultSettings()
        {
            federation.SchemaManager = GetUnversionedTypeName(typeof(Jhu.Graywulf.Schema.GraywulfSchemaManager));
            federation.UserDatabaseFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.Schema.UserDatabaseFactory));
            federation.QueryFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.Jobs.Query.SqlQueryFactory));
            federation.FileFormatFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.Format.FileFormatFactory));
            federation.StreamFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.IO.StreamFactory));
            federation.ImportTablesJobFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.Jobs.ImportTables.ImportTablesJobFactory));
            federation.ExportTablesJobFactory = GetUnversionedTypeName(typeof(Jhu.Graywulf.Jobs.ExportTables.ExportTablesJobFactory));

            federation.ShortTitle = "";
            federation.LongTitle = "";
            federation.Copyright = Util.AssemblyReflector.GetCopyright();
            federation.Disclaimer = domain.Disclaimer;
            federation.Email = domain.Email;
        }

        public virtual void GenerateDefaultChildren(ServerVersion myDbServerVersion, ServerVersion nodeServerVersion)
        {
            // Generate database definitions
            GenerateUserDatabaseDefinition(myDbServerVersion);
            GenerateCodeDatabaseDefinition(nodeServerVersion);

            GenerateDefaultJobs();
            GenerateQueryJobs();

            federation.Save();
        }

        protected virtual void GenerateDefaultJobs()
        {
            var sji = new Jobs.SqlScript.SqlScriptJobInstaller(federation);
            sji.Install();

            var eji = new Jobs.ExportTables.ExportTablesJobInstaller(federation);
            eji.Install();

            var emji = new Jobs.ExportTables.ExportMaintenanceJobInstaller(federation);
            emji.Install();

            var iji = new Jobs.ImportTables.ImportTablesJobInstaller(federation);
            iji.Install();

            var cji = new Jobs.CopyTables.CopyTablesJobInstaller(federation);
            cji.Install();
        }

        protected virtual void GenerateQueryJobs()
        {
            var jdi = new Jobs.Query.SqlQueryJobInstaller(federation);
            jdi.Install();
        }

        protected void GenerateUserDatabaseDefinition(ServerVersion userDatabaseServerVersion)
        {
            DatabaseDefinition mydbdd = new DatabaseDefinition(federation)
            {
                Name = Constants.UserDbName,
                System = federation.System,
                LayoutType = DatabaseLayoutType.Monolithic,
                DatabaseInstanceNamePattern = Constants.UserDbInstanceNamePattern,
                DatabaseNamePattern = Constants.UserDbNamePattern,
                SliceCount = 1,
                PartitionCount = 1,
            };
            mydbdd.Save();

            var mydbddi = new DatabaseDefinitionInstaller(mydbdd);
            mydbddi.GenerateDefaultChildren(userDatabaseServerVersion, Constants.UserDbName);

            mydbdd.LoadDatabaseVersions(true);
            federation.UserDatabaseVersion = mydbdd.DatabaseVersions[Constants.UserDbName];
        }

        protected void GenerateCodeDatabaseDefinition(ServerVersion nodeServerVersion)
        {
            DatabaseDefinition codedbdd = new DatabaseDefinition(federation)
            {
                Name = Constants.CodeDbName,
                System = federation.System,
                LayoutType = DatabaseLayoutType.Mirrored,
                DatabaseInstanceNamePattern = Constants.CodeDbInstanceNamePattern,
                DatabaseNamePattern = Constants.CodeDbNamePattern,
                SliceCount = 1,
                PartitionCount = 1,
            };
            codedbdd.Save();

            var codedbddi = new DatabaseDefinitionInstaller(codedbdd);
            codedbddi.GenerateDefaultChildren(nodeServerVersion, Constants.CodeDbName);

            codedbdd.LoadDatabaseVersions(true);
            federation.CodeDatabaseVersion = codedbdd.DatabaseVersions[Constants.CodeDbName];
        }
    }
}
