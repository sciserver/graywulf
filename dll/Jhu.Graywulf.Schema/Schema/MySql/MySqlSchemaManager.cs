using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Schema.MySql
{
    class MySqlSchemaManager
    {
        public class SqlServerSchemaManager : SchemaManager
        {
            private const string prefix = "Jhu.SkyServer.Schema.Dataset.";

            public SqlServerSchemaManager()
                : base()
            {
            }

            protected override DatasetBase LoadDataset(string datasetName)
            {
                string key = String.Format("{0}{1}", prefix, datasetName);

                MySqlDataset ds =
                    new MySqlDataset(
                        datasetName,
                        ConfigurationManager.ConnectionStrings[key].ConnectionString);

                return ds;
            }

            protected override IEnumerable<KeyValuePair<string, DatasetBase>> LoadAllDatasets()
            {
                foreach (ConnectionStringSettings s in ConfigurationManager.ConnectionStrings)
                {
                    if (s.Name.StartsWith(prefix))
                    {
                        string name = s.Name.Substring(prefix.Length);

                        MySqlDataset ds =
                        new MySqlDataset(
                            name,
                            s.ConnectionString);

                        yield return new KeyValuePair<string, DatasetBase>(name, ds);
                    }
                }
            }
        }
    }
}