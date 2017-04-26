using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseDefinition
    {
        protected override void OnDiscover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            DeploymentState = DeploymentState.Deployed;
            update.Add(this);

            // Load existing file groups
            LoadFileGroups(true);

            // Query database definition for filegroups
            var smodb = GetSchemaSmoDatabase();

            // --- add standard file groups
            foreach (var smofg in smodb.FileGroups.Cast<smo::FileGroup>())
            {
                var fg = FileGroups.Values.FirstOrDefault(i => Entity.StringComparer.Compare(i.FileGroupName, smofg.Name) == 0);

                if (fg == null)
                {
                    create.Add(DiscoverFileGroup(smofg, fg));
                }
                else
                {
                    update.Add(DiscoverFileGroup(smofg, fg));
                }
            }

            // --- add dummy file group for logs
            {
                var fg = FileGroups.Values.FirstOrDefault(i => i.FileGroupType == FileGroupType.Log);

                if (fg == null)
                {
                    create.Add(DiscoverLogFileGroup(smodb, fg));
                }
                else
                {
                    update.Add(DiscoverLogFileGroup(smodb, fg));
                }
            }

            // Delete those file groups that are not in the real database
            foreach (var fg in FileGroups.Values)
            {
                if (!update.Contains(fg))
                {
                    delete.Add(fg);
                }
            }
        }

        private FileGroup DiscoverFileGroup(smo::FileGroup smofg, FileGroup fg)
        {
            if (fg == null)
            {
                fg = new FileGroup(this);
            }

            fg.Name = smofg.Name;
            fg.FileGroupName = smofg.Name;
            fg.FileGroupType = FileGroupType.Data;
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.DiskDesignation = DiskDesignation.Data;
            fg.FileCount = 0;   // TODO: check this to be one file/volume

            fg.DeploymentState = DeploymentState.Deployed;
            fg.RunningState = RunningState.Running;

            fg.AllocatedSpace = (long)Math.Ceiling(smofg.Size * 0x400L);     // given in KB, bug in docs!
            fg.AllocatedSpace = Math.Max(0x1000000L, fg.AllocatedSpace);        // 16 MB minimum

            return fg;
        }

        private FileGroup DiscoverLogFileGroup(smo::Database smodb, FileGroup fg)
        {
            if (fg == null)
            {
                fg = new FileGroup(this);
            }

            fg.Name = "LOG";
            fg.FileGroupName = "LOG";
            fg.FileGroupType = FileGroupType.Log;
            fg.AllocationType = FileGroupAllocationType.CrossVolume;
            fg.LayoutType = FileGroupLayoutType.Monolithic;
            fg.DiskDesignation = DiskDesignation.Log;
            fg.FileCount = 0;   // TODO: check this to be one file/volume

            fg.DeploymentState = DeploymentState.Deployed;
            fg.RunningState = RunningState.Running;

            // Calculate log size
            fg.AllocatedSpace = 0;
            foreach (var smolf in smodb.LogFiles.Cast<smo::LogFile>())
            {
                fg.AllocatedSpace += (long)Math.Ceiling(smolf.Size * 0x400L);    // given in KB, bug in docs!
            }
            fg.AllocatedSpace = Math.Max(0x1000000L, fg.AllocatedSpace);            // 16 MB minimum

            return fg;
        }
    }
}
