using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    class DatasetFactory
    {
        public static GraywulfDataset CreateDataset(DatabaseDefinition dd)
        {
            if (dd.RunningState != RunningState.Running)
            {
                throw new SchemaException(String.Format(ExceptionMessages.AccessDeniedToDataset, dd.Name));
            }

            GraywulfDataset ds = new GraywulfDataset();
            ds.Name = dd.Name;
            ds.DatabaseDefinitionName = dd.GetFullyQualifiedName();
            ds.ConnectionString = dd.GetConnectionString().ConnectionString;
            ds.IsCacheable = true;

            return ds;
        }

        public static DatasetBase CreateDataset(RemoteDatabase rd)
        {
            switch (rd.ProviderName)
            {
                case Constants.SqlServerProviderName:
                    return CreateSqlServerDataset(rd);
                case Constants.MySqlProviderName:
                    return CreateMySqlDataset(rd);
                default:
                    throw new NotImplementedException();
            }
        }

        private static SqlServerDataset CreateSqlServerDataset(RemoteDatabase rd)
        {
            SqlServerDataset ds = new SqlServerDataset()
            {
                Name = rd.Name,
                //IsRemoteDataset = true,
                IsOnLinkedServer = false,
                IsCacheable = true,
            };

            ds.ConnectionString = ds.GetSpecializedConnectionString(
                rd.ConnectionString,
                rd.IntegratedSecurity,
                rd.Username,
                rd.Password,
                false);

            return ds;
        }

        private static MySqlDataset CreateMySqlDataset(RemoteDatabase rd)
        {
            MySqlDataset ds = new MySqlDataset()
            {
                Name = rd.Name,
                IsCacheable = true,
            };

            ds.ConnectionString = ds.GetSpecializedConnectionString(
                rd.ConnectionString,
                rd.IntegratedSecurity,
                rd.Username,
                rd.Password,
                false);

            return ds;
        }
    }
}
