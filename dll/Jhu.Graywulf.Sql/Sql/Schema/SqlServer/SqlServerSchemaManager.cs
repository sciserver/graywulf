using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Sql.Schema.SqlServer
{
    /// <summary>
    /// Implements a schema manager that works with a single SQL Server instance
    /// </summary>
    /// <remarks>
    /// It takes the list of Datasets from the application configuration file and
    /// handles only SqlServerDatasets.
    /// </remarks>
    public class SqlServerSchemaManager : SchemaManager
    {
        public const string ConnectionStringNamePrefix = "Jhu.Graywulf.Schema.Dataset";

        public SqlServerSchemaManager()
            : base()
        {
        }

        protected override DatasetBase LoadDataset(string datasetName)
        {
            string key = String.Format("{0}.{1}", ConnectionStringNamePrefix, datasetName);

            SqlServerDataset ds =
                new SqlServerDataset(
                    datasetName,
                    ConfigurationManager.ConnectionStrings[key].ConnectionString);

            return ds;
        }

        protected override IEnumerable<KeyValuePair<string, DatasetBase>> LoadAllDatasets()
        {
            foreach (ConnectionStringSettings s in ConfigurationManager.ConnectionStrings)
            {
                if (s.Name.StartsWith(ConnectionStringNamePrefix))
                {
                    string name = s.Name.Substring(ConnectionStringNamePrefix.Length + 1);

                    SqlServerDataset ds =
                    new SqlServerDataset(
                        name,
                        s.ConnectionString);

                    yield return new KeyValuePair<string, DatasetBase>(name, ds);
                }
            }
        }
    }
}
