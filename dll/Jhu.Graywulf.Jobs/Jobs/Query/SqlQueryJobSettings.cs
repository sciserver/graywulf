using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryJobSettings : EntitySettings
    {
        public string HotDatabaseVersionName
        {
            get { return GetValue("HotDatabaseVersionName"); }
            set { SetValue("HotDatabaseVersionName", value); }
        }

        public string StatDatabaseVersionName
        {
            get { return GetValue("StatDatabaseVersionName"); }
            set { SetValue("StatDatabaseVersionName", value); }
        }

        public string DefaultDatasetName
        {
            get { return GetValue("DefaultDatasetName"); }
            set { SetValue("DefaultDatasetName", value); }
        }

        public string DefaultSchemaName
        {
            get { return GetValue("DefaultSchemaName"); }
            set { SetValue("DefaultSchemaName", value); }
        }

        public int QueryTimeout
        {
            get { return int.Parse(GetValue("QueryTimeout")); }
            set { SetValue("QueryTimeout", value.ToString()); }
        }

        public SqlQueryJobSettings()
        {
        }

        public SqlQueryJobSettings(EntitySettings old)
            : base(old)
        {
        }
    }
}
