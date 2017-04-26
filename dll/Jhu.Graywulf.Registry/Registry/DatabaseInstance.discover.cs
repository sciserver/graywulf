using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstance
    {
        protected override void OnDiscover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            if (IsExisting)
            {
                var smodb = GetSmoDatabase();

                Discover(smodb, update, delete, create);

                if (smodb != null)
                {
                    this.DeploymentState = DeploymentState.Deployed;
                    this.RunningState = RunningState.Attached;
                }
                else
                {
                    this.DeploymentState = DeploymentState.Undeployed;
                    this.RunningState = RunningState.Detached;     // TODO
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void Discover(smo::Database smodb, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);

            if (smodb != null)
            {
                LoadFromSmo(smodb);

                // Process file groups
                // --- add standard file groups
                foreach (var smofg in smodb.FileGroups.Cast<smo::FileGroup>())
                {
                    var fg = FileGroups.Values.FirstOrDefault(i => Entity.StringComparer.Compare(i.FileGroupName, smofg.Name) == 0);
                    if (fg == null)
                    {
                        fg = new DatabaseInstanceFileGroup(this);
                    }

                    fg.DiscoverFileGroup(smofg, update, delete, create);
                }

                // --- add dummy file group for logs
                {
                    var fg = FileGroups.Values.FirstOrDefault(i => i.FileGroupType == FileGroupType.Log);
                    if (fg == null)
                    {
                        fg = new DatabaseInstanceFileGroup(this);
                    }

                    fg.DiscoverLogFileGroup(smodb, update, delete, create);
                }

                DiscoverDeletedFileGroups(update, delete, create);
            }
            else
            {
                foreach (var fg in FileGroups.Values)
                {
                    fg.DiscoverFileGroup(null, update, delete, create);
                }
            }
        }

        private void InitializeDiscovery(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            if (IsExisting)
            {
                update.Add(this);
            }
            else
            {
                throw new InvalidOperationException();
            }

            LoadFileGroups(true);
        }

        private void DiscoverDeletedFileGroups(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            // Delete those file groups that are not in the real database
            foreach (var fg in FileGroups.Values)
            {
                if (!update.Contains(fg))
                {
                    delete.Add(fg);
                }
            }
        }
    }
}
