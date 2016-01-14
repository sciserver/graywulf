using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Install
{
    public class DatabaseDefinitionInstaller : InstallerBase
    {
        private DatabaseDefinition databaseDefinition;

        public DatabaseDefinitionInstaller(DatabaseDefinition databaseDefinition)
            : base(databaseDefinition.Context)
        {
            this.databaseDefinition = databaseDefinition;
        }


        /// <summary>
        /// Initializes a database definition.
        /// </summary>
        /// <param name="serverVersion"></param>
        public void GenerateDefaultChildren(ServerVersion serverVersion, string databaseVersionName)
        {
            // If not sliced, then create a default slice of FULL
            if (databaseDefinition.LayoutType == DatabaseLayoutType.Mirrored ||
                databaseDefinition.LayoutType == DatabaseLayoutType.Monolithic)
            {
                Slice sl = new Slice(databaseDefinition)
                {
                    Name = Constants.FullSliceName,
                    System = databaseDefinition.System,
                };
                sl.Save();
            }
            else
            {
                throw new InvalidOperationException();
                // Use slicing wizard instead
            }

            // Add primary filegroup
            FileGroupLayoutType fglayout;
            switch (databaseDefinition.LayoutType)
            {
                case DatabaseLayoutType.Sliced:
                    fglayout = FileGroupLayoutType.Sliced;
                    break;
                case DatabaseLayoutType.Monolithic:
                case DatabaseLayoutType.Mirrored:
                    fglayout = FileGroupLayoutType.Monolithic;
                    break;
                default:
                    throw new NotImplementedException();
            }

            FileGroup fg = new FileGroup(databaseDefinition)
            {
                Name = Constants.PrimaryFileGroupName,
                System = databaseDefinition.System,
                FileGroupType = FileGroupType.Data,
                LayoutType = fglayout,
                AllocationType = FileGroupAllocationType.CrossVolume,
                DiskDesignation = DiskDesignation.Data,
                FileGroupName = Constants.PrimaryFileGroupName,
                AllocatedSpace = Constants.PrimaryFileGroupSize,
                FileCount = 0,
            };
            fg.Save();

            // Add "log" file group
            fg = new FileGroup(databaseDefinition)
            {
                Name = Constants.LogFileGroupName,
                System = databaseDefinition.System,
                FileGroupType = FileGroupType.Log,
                LayoutType = FileGroupLayoutType.Monolithic,
                AllocationType = FileGroupAllocationType.CrossVolume,
                DiskDesignation = DiskDesignation.Log,
                FileGroupName = Constants.LogFileGroupName,
                AllocatedSpace = Constants.LogFileGroupSize,
                FileCount = 0
            };
            fg.Save();

            // Create default database version
            DatabaseVersion dv = new DatabaseVersion(databaseDefinition)
            {
                Name = databaseVersionName,
                System = databaseDefinition.System,
                ServerVersion = serverVersion,
                SizeMultiplier = 1.0f,
            };
            dv.Save();
        }

        public List<Slice> GenerateSlices(string[] sliceNames, long[][] sliceLimits, string[][] partitionNames, long[][][] partitionLimits)
        {
            List<Slice> slices = new List<Slice>();

            for (int si = 0; si < sliceNames.Length; si++)
            {
                Slice ns = new Slice(databaseDefinition);

                ns.Name = sliceNames[si];
                ns.From = sliceLimits[si][0];
                ns.To = sliceLimits[si][1];
                ns.Save();

                ns.GeneratePartitions(partitionNames[si], partitionLimits[si]);

                slices.Add(ns);
            }

            return slices;
        }

        
    }
}
