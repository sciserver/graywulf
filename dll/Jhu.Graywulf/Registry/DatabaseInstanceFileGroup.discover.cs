using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstanceFileGroup
    {
        public override void Discover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            switch (FileGroupType)
            {
                case Registry.FileGroupType.Data:
                    DiscoverFileGroup(GetSmoFileGroup(), update, delete, create);
                    break;
                case Registry.FileGroupType.Log:
                    DiscoverLogFileGroup(GetSmoDatabase(), update, delete, create);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void DiscoverFileGroup(smo::FileGroup smofg, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);
            
            LoadFromSmo(smofg);

            DiscoverDatabaseDefinitionFileGroup();

            // Query database definition for filegroups
            // --- add files
            foreach (var smofile in smofg.Files.Cast<smo::DataFile>())
            {
                var file = Files.Values.FirstOrDefault(i => Entity.StringComparer.Compare(i.LogicalName, smofile.Name) == 0);
                if (file == null)
                {
                    file = new DatabaseInstanceFile(Context, this);
                }

                file.DiscoverFile(smofile, update, delete, create);
            }

            DiscoverDeletedFiles(update, delete, create);
        }

        internal void DiscoverLogFileGroup(smo::Database smodb, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);

            LoadFromSmo(smodb);

            DiscoverDatabaseDefinitionFileGroup();

            foreach (var smofile in smodb.LogFiles.Cast<smo::LogFile>())
            {
                var file = Files.Values.FirstOrDefault(i => Entity.StringComparer.Compare(i.LogicalName, smofile.Name) == 0);
                if (file == null)
                {
                    file = new DatabaseInstanceFile(Context, this);
                }

                file.DiscoverLogFile(smofile, update, delete, create);
            }

            DiscoverDeletedFiles(update, delete, create);
        }

        private void InitializeDiscovery(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            if (IsExisting)
            {
                update.Add(this);
            }
            else
            {
                create.Add(this);
            }

            LoadFiles(true);
        }

        private void DiscoverDatabaseDefinitionFileGroup()
        {
            // Find database definition file group this file group belongs to
            // TODO: this only works with monolithic (non-partitioned databases)
            if (DatabaseInstance.DatabaseDefinition.LayoutType == DatabaseLayoutType.Sliced)
            {
                throw new InvalidOperationException("Discovery not supported for sliced DBs");  // TODO
            }

            DatabaseInstance.DatabaseDefinition.LoadFileGroups(true);
            FileGroup = DatabaseInstance.DatabaseDefinition.FileGroups.Values.FirstOrDefault(i => Entity.StringComparer.Compare(this.FileGroupName, i.FileGroupName) == 0);

            if (FileGroupReference.IsEmpty)
            {
                throw new DiscoveryException("File group cannot be associated with file group of database definition.");   // TODO
            }
        }

        private void DiscoverDeletedFiles(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            foreach (var file in Files.Values)
            {
                if (!update.Contains(file))
                {
                    delete.Add(file);
                }
            }
        }

    }
}
