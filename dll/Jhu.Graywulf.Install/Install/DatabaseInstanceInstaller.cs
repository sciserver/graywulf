using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class DatabaseInstanceInstaller : ContextObject
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

        public DatabaseInstance GenerateDatabaseInstance(ServerInstance serverInstance, List<DiskVolume> dataDiskVolumes, List<DiskVolume> logDiskVolumes, Slice slice, DatabaseVersion databaseVersion, string namePattern, string databaseNamePattern, double sizeFactor, bool generateFileGroups)
        {
            // --- Create the new database instance and set name
            DatabaseInstance ndi = new DatabaseInstance(databaseDefinition);

            ndi.ServerInstanceReference.Guid = serverInstance.Guid;
            ndi.SliceReference.Guid = slice.Guid;
            ndi.DatabaseVersionReference.Guid = databaseVersion.Guid;

            ndi.Name = Util.ResolveExpression(ndi, namePattern);
            ndi.DatabaseName = Util.ResolveExpression(ndi, databaseNamePattern);

            ndi.Save();

            if (generateFileGroups)
            {
                ndi.ServerInstance.Machine.LoadDiskVolumes(false);

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

                        nfg.GenerateInstanceFiles(dataDiskVolumes, sizeFactor);
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

                            nfg.GenerateInstanceFiles(dataDiskVolumes, sizeFactor);
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
    }
}
