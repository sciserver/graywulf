using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Install
{
    public class SqlQueryJobInstaller : ContextObject
    {
        private Federation federation;

        public SqlQueryJobInstaller(Federation federation)
            : base(federation.Context)
        {
            this.federation = federation;
        }

        public void Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = typeof(Jobs.Query.SqlQueryJob).Name,
                System = federation.System,
                WorkflowTypeName = typeof(Jobs.Query.SqlQueryJob).AssemblyQualifiedName,
                Settings = Util.SaveSettings(new Dictionary<SqlQueryFactory.Settings, string>()
                {
                    // TODO: update these in the query factory
                    {SqlQueryFactory.Settings.HotDatabaseVersionName, Registry.Constants.HotDatabaseVersionName},
                    {SqlQueryFactory.Settings.StatDatabaseVersionName, Registry.Constants.StatDatabaseVersionName},
                    {SqlQueryFactory.Settings.DefaultSchemaName, Registry.Constants.DefaultSchemaName},
                    {SqlQueryFactory.Settings.DefaultDatasetName, Registry.Constants.MyDbName},
                    {SqlQueryFactory.Settings.DefaultTableName, "outputtable"}, // TODO
                    {SqlQueryFactory.Settings.TemporarySchemaName, Registry.Constants.DefaultSchemaName},
                    {SqlQueryFactory.Settings.LongQueryTimeout, "7200"},    // TODO
                }),
            };
            jd.Save();
        }
    }
}
