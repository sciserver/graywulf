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
        public override void Discover(List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            if (IsExisting)
            {
                Discover(GetSmoDatabase(), update, delete, create);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        internal void Discover(smo::Database smodb, List<Entity> update, List<Entity> delete, List<Entity> create)
        {
            InitializeDiscovery(update, delete, create);
            LoadFromSmo(smodb);

            // Process file groups
            // --- add standard file groups
            foreach (var smofg in smodb.FileGroups.Cast<smo::FileGroup>())
            {
                var fg = FileGroups.Values.FirstOrDefault(i => Entity.StringComparer.Compare(i.FileGroupName, smofg.Name) == 0);
                if (fg == null)
                {
                    fg = new DatabaseInstanceFileGroup(Context, this);
                }

                fg.DiscoverFileGroup(smofg, update, delete, create);
            }

            // --- add dummy file group for logs
            {
                var fg = FileGroups.Values.FirstOrDefault(i => i.FileGroupType == FileGroupType.Log);
                if (fg == null)
                {
                    fg = new DatabaseInstanceFileGroup(Context, this);
                }

                fg.DiscoverLogFileGroup(smodb, update, delete, create);
            }

            DiscoverDeletedFileGroups(update, delete, create);
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
