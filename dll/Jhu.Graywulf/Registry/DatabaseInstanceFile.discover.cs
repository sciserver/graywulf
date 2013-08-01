using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstanceFile
    {
        public override void Discover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            switch (DatabaseFileType)
            {
                case Registry.DatabaseFileType.Data:
                    DiscoverFile(GetSmoFile(), update, delete, create);
                    break;
                case Registry.DatabaseFileType.Log:
                    DiscoverLogFile(GetSmoLogFile(), update, delete, create);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void DiscoverFile(smo::DataFile smofile, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);
            LoadFromSmo(smofile);
            DiscoverDiskVolume();
        }

        internal void DiscoverLogFile(smo::LogFile smofile, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);
            LoadFromSmo(smofile);
            DiscoverDiskVolume();
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
        }

        private void DiscoverDiskVolume()
        {
            var m = DatabaseInstanceFileGroup.DatabaseInstance.ServerInstance.Machine;
            m.LoadDiskVolumes(true);

            DiskVolume = m.DiskVolumes.Values.FirstOrDefault(i => Filename.StartsWith(i.LocalPath.ResolvedValue, StringComparison.InvariantCultureIgnoreCase));

            if (DiskVolumeReference.IsEmpty)
            {
                throw new DiscoveryException("File cannot be associated with disk volume.");   // TODO
            }
        }
    }
}
