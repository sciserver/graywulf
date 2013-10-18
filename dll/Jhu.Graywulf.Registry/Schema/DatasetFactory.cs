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
            ds.DatabaseDefinition.Value = dd;
            ds.IsCacheable = true;

            ds.CacheSchemaConnectionString();

            return ds;
        }

        public static DatasetBase CreateDataset(RemoteDatabase rd)
        {
            switch (rd.ProviderName)
            {
                case Schema.SqlServer.Constants.SqlServerProviderName:
                    return CreateSqlServerDataset(rd);
                case Schema.MySql.Constants.MySqlProviderName:
                    return CreateMySqlDataset(rd);
                case Schema.PostgreSql.Constants.PostgreSqlProviderName:
                    return CreatePostgreSqlDataset(rd);
                default:
                    throw new NotImplementedException();
            }
        }

        private static Schema.SqlServer.SqlServerDataset CreateSqlServerDataset(RemoteDatabase rd)
        {
            var ds = new Schema.SqlServer.SqlServerDataset()
            {
                Name = rd.Name,
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

        private static Schema.MySql.MySqlDataset CreateMySqlDataset(RemoteDatabase rd)
        {
            var ds = new Schema.MySql.MySqlDataset()
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

        private static Schema.PostgreSql.PostgreSqlDataset CreatePostgreSqlDataset(RemoteDatabase rd)
        {
            var ds = new Schema.PostgreSql.PostgreSqlDataset()
            {
                Name = rd.Name,
                IsCacheable = true,
                DefaultSchemaName = "public",
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
