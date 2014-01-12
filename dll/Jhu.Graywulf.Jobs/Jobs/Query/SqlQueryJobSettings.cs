using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryJobSettings : ParameterCollection
    {
        public string HotDatabaseVersionName
        {
            get { return (string)this["HotDatabaseVersionName"].Value; }
            set { this["HotDatabaseVersionName"].Value = value; }
        }

        public string StatDatabaseVersionName
        {
            get { return (string)this["StatDatabaseVersionName"].Value; }
            set { this["StatDatabaseVersionName"].Value = value; }
        }

        public string DefaultDatasetName
        {
            get { return (string)this["DefaultDatasetName"].Value; }
            set { this["DefaultDatasetName"].Value = value; }
        }

        public string DefaultSchemaName
        {
            get { return (string)this["DefaultSchemaName"].Value; }
            set { this["DefaultSchemaName"].Value = value; }
        }

        public int QueryTimeout
        {
            get { return (int)this["QueryTimeout"].Value; }
            set { this["QueryTimeout"].Value = value; }
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
