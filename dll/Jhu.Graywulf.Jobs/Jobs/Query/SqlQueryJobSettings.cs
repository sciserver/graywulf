using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryJobSettings : ParameterCollection
    {
        public string HotDatabaseVersionName
        {
            get { return GetValue<string>("HotDatabaseVersionName"); }
            set { SetValue("HotDatabaseVersionName", value); }
        }

        public string StatDatabaseVersionName
        {
            get { return GetValue<string>("StatDatabaseVersionName"); }
            set { SetValue("StatDatabaseVersionName", value); }
        }

        public string DefaultDatasetName
        {
            get { return GetValue<string>("DefaultDatasetName"); }
            set { SetValue("DefaultDatasetName", value); }
        }

        public string DefaultSchemaName
        {
            get { return GetValue<string>("DefaultSchemaName"); }
            set { SetValue("DefaultSchemaName", value); }
        }

        public int QueryTimeout
        {
            get { return GetValue<int>("QueryTimeout"); }
            set { SetValue("QueryTimeout", value); }
        }

        public SqlQueryJobSettings()
        {
        }

        public SqlQueryJobSettings(ParameterCollection old)
            : base(old)
        {
        }
    }
}
