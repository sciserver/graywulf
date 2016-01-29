using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class DatabaseInstanceInstaller : InstallerBase
    {
        private DatabaseDefinition databaseDefinition;

        public DatabaseInstanceInstaller(DatabaseDefinition databaseDefinition)
            :base(databaseDefinition.Context)
        {
            this.databaseDefinition = databaseDefinition;
        }

        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, Slice slice, DatabaseVersion databaseVersion)
        {
            return GenerateDatabaseInstance(
                serverInstance,
                slice,
                databaseVersion,
                databaseDefinition.DatabaseInstanceNamePattern,
                databaseDefinition.DatabaseNamePattern,
                databaseVersion.SizeMultiplier,
                true);
        }

        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, Slice slice, DatabaseVersion databaseVersion, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            return GenerateDatabaseInstance(serverInstance, null, null, slice, databaseVersion, namePattern, databaseNamePattern, sizeFactor, generateFileGroups);
        }

        private DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, List<DiskVolume> dataDiskVolumes, List<DiskVolume> logDiskVolumes, Slice slice, DatabaseVersion databaseVersion, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            // --- Create the new database instance and set name
            DatabaseInstance ndi = new DatabaseInstance(databaseDefinition);

            ndi.ServerInstanceReference.Guid = serverInstance.Guid;
            ndi.SliceReference.Guid = slice.Guid;
            ndi.DatabaseVersionReference.Guid = databaseVersion.Guid;

            ndi.Name = ExpressionProperty.ResolveExpression(ndi, namePattern);
            ndi.DatabaseName = ExpressionProperty.ResolveExpression(ndi, databaseNamePattern);

            ndi.Save();

            if (generateFileGroups)
            {
                ndi.ServerInstance.LoadDiskGroups(false);
                databaseDefinition.LoadFileGroups(false);

                slice.LoadPartitions(false);
                List<Partition> partitions = new List<Partition>(slice.Partitions.Values.OrderBy(i => i.Number));
                List<FileGroup> filegroups = new List<FileGroup>(databaseDefinition.FileGroups.Values.OrderBy(i => i.Number));

                for (int fi = 0; fi < filegroups.Count; fi++)
                {
                    // --- Create data and "log" file groups ---
                    if (filegroups[fi].LayoutType == FileGroupLayoutType.Monolithic ||
                        filegroups[fi].FileGroupType == FileGroupType.Log)
                    {
                        DatabaseInstanceFileGroup nfg = new DatabaseInstanceFileGroup(ndi);
                        nfg.FileGroupType = filegroups[fi].FileGroupType;
                        nfg.FileGroupName = nfg.Name = filegroups[fi].FileGroupName;
                        nfg.FileGroupReference.Guid = filegroups[fi].Guid;
                        nfg.PartitionReference.Guid = Guid.Empty;
                        nfg.AllocatedSpace = (long)(filegroups[fi].AllocatedSpace * sizeFactor);
                        nfg.Save();

                        GenerateInstanceFiles(nfg, dataDiskVolumes, sizeFactor);
                    }
                    else if (filegroups[fi].LayoutType == FileGroupLayoutType.Sliced)
                    {
                        for (int pi = 0; pi < partitions.Count; pi++)
                        {
                            DatabaseInstanceFileGroup nfg = new DatabaseInstanceFileGroup(ndi);
                            nfg.FileGroupType = filegroups[fi].FileGroupType;
                            nfg.FileGroupName = nfg.Name = string.Format("{0}_{1}", filegroups[fi].FileGroupName, pi);
                            nfg.FileGroupReference.Guid = filegroups[fi].Guid;
                            nfg.PartitionReference.Guid = partitions[pi].Guid;
                            nfg.AllocatedSpace = (long)(filegroups[fi].AllocatedSpace * sizeFactor);
                            nfg.Save();

                            GenerateInstanceFiles(nfg, dataDiskVolumes, sizeFactor);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            return ndi;
        }

        public List<DatabaseInstance> GenerateDatabaseInstances(ServerInstance[][] serverInstances)
        {
            return GenerateDatabaseInstances(
                serverInstances,
                databaseDefinition.DatabaseInstanceNamePattern,
                databaseDefinition.DatabaseInstanceNamePattern,
                1.0,
                true);
        }

        public List<DatabaseInstance> GenerateDatabaseInstances(ServerInstance[][] serverInstances, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            List<DatabaseInstance> instances = new List<DatabaseInstance>();

            databaseDefinition.LoadDatabaseVersions(false);
            databaseDefinition.LoadSlices(false);

            List<Slice> slices = new List<Slice>(databaseDefinition.Slices.Values.OrderBy(i => i.Number));

            for (int si = 0; si < slices.Count; si++)
            {
                Slice slice = slices[si];
                // **** TODO review this part and add [$Number] to pattern if mirrored
                // to avoid name collision under databaseinstance
                foreach (DatabaseVersion rs in databaseDefinition.DatabaseVersions.Values)
                {
                    // TODO: do not use rs.Number here!!!
                    DatabaseInstance ndi = GenerateDatabaseInstance(serverInstances[si][rs.Number], slices[si], rs, namePattern, databaseNamePattern, sizeFactor, generateFileGroups);

                    instances.Add(ndi);
                }
            }

            return instances;
        }

        public void GenerateInstanceFiles(DatabaseInstanceFileGroup fg, List<DiskVolume> dataDiskVolumes, double sizeFactor)
        {
            Dictionary<string, DatabaseInstanceFile> files = new Dictionary<string, DatabaseInstanceFile>();

            List<DiskVolume> diskVolumes = new List<DiskVolume>();
            if (dataDiskVolumes == null)
            {
                // Associate disks with file groups automatically
                // By default, all disk groups designetad as data are used

                fg.DatabaseInstance.ServerInstance.LoadDiskGroups(false);
                foreach (var dg in fg.DatabaseInstance.ServerInstance.DiskGroups.Values.Where(i => (i.DiskDesignation & fg.FileGroup.DiskDesignation) != 0))
                {
                    dg.DiskGroup.LoadDiskVolumes(false);
                    diskVolumes.AddRange(dg.DiskGroup.DiskVolumes.Values.OrderBy(i => i.Number));
                }
                
                // TODO: delete
                //fg.DatabaseInstance.ServerInstance.Machine.LoadDiskVolumes(false);
                //diskVolumes.AddRange(fg.DatabaseInstance.ServerInstance.Machine.DiskVolumes.Values.Where<DiskVolume>(d => (d.DiskVolumeType & fg.FileGroup.DiskGroupType) > 0).OrderBy(i => i.Number));
            }
            else
            {
                diskVolumes.AddRange(dataDiskVolumes);
            }

            int q = 0;
            bool primary;
            int filecount = fg.FileGroup.FileCount != 0 ? fg.FileGroup.FileCount : diskVolumes.Count;

            for (int i = 0; i < filecount; i++)
            {
                DatabaseInstanceFile nf = new DatabaseInstanceFile(fg);
                nf.DiskVolumeReference.Guid = diskVolumes[q % diskVolumes.Count].Guid;

                switch (fg.FileGroupType)
                {
                    case FileGroupType.Data:
                        nf.DatabaseFileType = DatabaseFileType.Data;
                        break;
                    case FileGroupType.Log:
                        nf.DatabaseFileType = DatabaseFileType.Log;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                nf.LogicalName = string.Format("{0}_{1}", fg.FileGroupName, i);
                nf.Name = nf.LogicalName;
                
                if (fg.FileGroupType == FileGroupType.Log)
                {
                    primary = false;
                    nf.Filename = nf.LogicalName + ".ldf";
                }
                else if (i == 0 && StringComparer.InvariantCultureIgnoreCase.Compare(fg.FileGroup.FileGroupName, "primary") == 0)
                {
                    primary = true;
                    nf.Filename = nf.LogicalName + ".mdf";
                }
                else
                {
                    primary = false;
                    nf.Filename = nf.LogicalName + ".ndf";
                }

                q++;

                // Set minimum file sizes
                nf.AllocatedSpace = Math.Max((long)(fg.FileGroup.AllocatedSpace / filecount * sizeFactor), 0x80000);

                if (primary)
                {
                    nf.AllocatedSpace = Math.Max(nf.AllocatedSpace, 0x300000);
                }

                nf.Save();

                files.Add(nf.Name, nf);
            }

            fg.Files = files;
        }
    }
}
