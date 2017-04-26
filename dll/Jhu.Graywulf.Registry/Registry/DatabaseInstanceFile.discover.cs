using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstanceFile
    {
        protected override void OnDiscover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            switch (DatabaseFileType)
            {
                case DatabaseFileType.Data:
                    DiscoverFile(GetSmoFile(), update, delete, create);
                    break;
                case DatabaseFileType.Log:
                    DiscoverLogFile(GetSmoLogFile(), update, delete, create);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void DiscoverFile(smo::DataFile smofile, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);

            if (smofile != null)
            {
                LoadFromSmo(smofile);
            }

            DiscoverDiskVolume(smofile);
        }

        internal void DiscoverLogFile(smo::LogFile smofile, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);
            LoadFromSmo(smofile);
            DiscoverDiskVolume(smofile);
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

        private void DiscoverDiskVolume(smo::DatabaseFile smofile)
        {
            // TODO: prevent loading these all the time
            var disks = new List<DiskVolume>();
            var m = DatabaseInstanceFileGroup.DatabaseInstance.ServerInstance.Machine;
            m.LoadDiskGroups(false);

            foreach (var dg in m.DiskGroups.Values)
            {
                dg.LoadDiskVolumes(false);
                disks.AddRange(dg.DiskVolumes.Values);
            }

            var dv = disks.FirstOrDefault(i => Filename.StartsWith(i.LocalPath.ResolvedValue, StringComparison.InvariantCultureIgnoreCase));

            // Check if file could be associated with a disk volume
            if (smofile != null && dv == null)
            {
                // If the database exists but it's under a path that cannot be reached from
                // any of the known disk volumes, simply throw and exception

                throw new DiscoveryException("File cannot be associated with disk volume.");   // TODO
            }
            else if (dv != null)
            {
                // Only set the disk volume value if it could be associated with one.
                // If the database doesn't exists, we simply keep the old settings

                DiskVolume = dv;
            }

            // Make path relative to disk volume
            Filename = Path.GetFileName(Filename);
        }
    }
}
