using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class FederationInstaller : ContextObject
    {
        private Cluster cluster;
        private Federation federation;

        public FederationInstaller(Cluster cluster)
            : base(cluster.Context)
        {
            this.cluster = cluster;
        }

        public FederationInstaller(Federation federation)
            : base(federation.Context)
        {
            this.federation = federation;
        }

        public Federation Install()
        {
            // TODO *** use default name constant
            return Install("MyFederation");
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
            // TODO: fill in other settings from default constants.
            
            cluster.LoadMachineRoles(false);

            cluster.LoadDomains(false);
            var shareddomain = cluster.Domains[Constants.SharedDomainName];

            shareddomain.LoadFederations(false);
            var sharedfederation = shareddomain.Federations[Constants.SharedFederationName];

            sharedfederation.LoadDatabaseDefinitions(false);

            var tempdbdd = sharedfederation.DatabaseDefinitions[Constants.TempDbName];
            tempdbdd.LoadDatabaseVersions(false);
            
            var controllerRole = cluster.MachineRoles[Constants.ControllerMachineRoleName];
            controllerRole.LoadServerVersions(false);
            controllerRole.LoadMachines(false);

            var controller = controllerRole.Machines[Constants.ControllerMachineName];
            controller.LoadServerInstances(false);

            var nodeRole = cluster.MachineRoles[Constants.NodeMachineRoleName];
            nodeRole.LoadServerVersions(false);

            federation = new Federation(Context)
            {
                Name = name,

                ControllerMachine = controller,
                TempDatabaseVersion = tempdbdd.DatabaseVersions[Constants.TempDbName],
                SchemaSourceServerInstance = controller.ServerInstances[Constants.ServerInstanceName],
            };

            GenerateDefaultSettings();

            // Generate MyDB, CodeDB and jobs
            GenerateDefaultChildren(controllerRole.ServerVersions[Constants.ServerVersionName], nodeRole.ServerVersions[Constants.ServerVersionName]);

            return federation;
        }

        public void GenerateDefaultSettings()
        {
            // TODO ***

            //ShortTitle
            //LongTitle
            //Email

            federation.QueryFactory = typeof(Jhu.Graywulf.Jobs.Query.SqlQueryFactory).AssemblyQualifiedName;
            federation.FileFormatFactory = typeof(Jhu.Graywulf.Format.FileFormatFactory).AssemblyQualifiedName;
            federation.StreamFactory = typeof(Jhu.Graywulf.IO.StreamFactory).AssemblyQualifiedName;
            federation.Copyright = Jhu.Graywulf.Copyright.InfoCopyright;
            federation.Disclaimer = ""; // TODO ***
        }

        public void GenerateDefaultChildren(ServerVersion myDbServerVersion, ServerVersion nodeServerVersion)
        {
            // Generate database definitions
            GenerateMyDBDefinition(myDbServerVersion);
            GenerateCodeDBDefinition(nodeServerVersion);

            // Job definitions
            var eji = new ExportTablesJobInstaller(federation);
            eji.Install();

            var emji = new ExportMaintenanceJobInstaller(federation);
            emji.Install();

            var jdi = new SqlQueryJobInstaller(federation);
            jdi.Install();

            federation.Save();
        }

        private void GenerateMyDBDefinition(ServerVersion myDbServerVersion)
        {
            DatabaseDefinition mydbdd = new DatabaseDefinition(federation)
            {
                Name = Constants.MyDbName,
                System = federation.System,
                LayoutType = DatabaseLayoutType.Monolithic,
                DatabaseInstanceNamePattern = Constants.MyDbInstanceNamePattern,
                DatabaseNamePattern = Constants.MyDbNamePattern,
                SliceCount = 1,
                PartitionCount = 1,
            };
            mydbdd.Save();

            var mydbddi = new DatabaseDefinitionInstaller(mydbdd);
            mydbddi.GenerateDefaultChildren(myDbServerVersion, Constants.MyDbName);

            mydbdd.LoadDatabaseVersions(true);
            federation.MyDBDatabaseVersion = mydbdd.DatabaseVersions[Constants.MyDbName];
        }

        private void GenerateCodeDBDefinition(ServerVersion nodeServerVersion)
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
