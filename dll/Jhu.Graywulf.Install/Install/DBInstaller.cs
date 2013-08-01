/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    /// <summary>
    /// This class collects function for creating the database for the cluster schema.
    /// </summary>
    /// <remarks>
    /// The class is used by the command-line utility schemautil and the test projects.
    /// It takes the database scripts from Scripts.resx.
    /// </remarks>
    public class DBInstaller
    {
        /// <summary>
        /// Creates a new database on the server and with the name specified in the app.config file.
        /// </summary>
        public void CreateDatabase()
        {
            string catalog;

            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(AppSettings.ConnectionString);
            catalog = csb.InitialCatalog;
            csb.InitialCatalog = string.Empty;
            
            string sql = string.Format(@"CREATE DATABASE {0}", catalog);

            using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Drops the database specified in the app.config file.
        /// </summary>
        public void DropDatabase()
        {
            DropDatabase(null);
        }

        /// <summary>
        /// Drops the database specified in the app.config file.
        /// </summary>
        /// <param name="checkExistance">If true: If the database does not exist, the drop is not attempted. False: Fails when database is absent.</param>
        public void DropDatabase(bool checkExistance)
        {
            DropDatabase(null, checkExistance);
        }


        /// <summary>
        /// Drops the database on the server specified in the app.config file with the name passed as a parameter.
        /// Throws an exception if the database is absent.
        /// </summary>
        /// <param name="catalog">Name of the database to drop. If null, the database
        /// specified in the app.config file is dropped.</param>
        /// <remarks>
        /// This function is used for deleting test databases too, not just the cluster schema database.
        /// </remarks>
        public void DropDatabase(string catalog)
        {
            DropDatabase(catalog, false);
        }

        /// <summary>
        /// Drops the database on the server specified in the app.config file with the name passed as a parameter.
        /// </summary>
        /// <param name="catalog">Name of the database to drop. If null, the database
        /// specified in the app.config file is dropped.</param>
        /// <param name="checkExistance">If true: If the database does not exist, the drop is not attempted. False: Fails when database is absent.</param>
        /// <remarks>
        /// This function is used for deleting test databases too, not just the cluster schema database.
        /// </remarks>
        public void DropDatabase(string catalog, bool checkExistance)
        {
            SqlConnection.ClearAllPools();

            string dropcatalog;

            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(AppSettings.ConnectionString);
            if (catalog == null)
            {
                dropcatalog = csb.InitialCatalog;
            }
            else
            {
                dropcatalog = catalog;
            }
            csb.InitialCatalog = string.Empty;

            string sql;
            if(checkExistance)
                sql = string.Format(@"IF EXISTS (SELECT * FROM master..sysdatabases WHERE name = '{0}') DROP DATABASE {0}", dropcatalog);
            else
                sql = string.Format(@"DROP DATABASE {0}", dropcatalog);

            using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Creates the schema required by this library to store its state in a database.
        /// </summary>
        /// <remarks>
        /// The database name is taken from the app.config file. It preparses the script,
        /// identifies the GO statements and executest the script in chunks, just like
        /// SQL Management Studio does. Also removes lines starting with USE to avoid executing
        /// script against the wrong database.
        /// </remarks>
        public void CreateSchema()
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                string[] scripts = SplitSqlScript(Scripts.Jhu_Graywulf_Registry);

                foreach (string sql in scripts)
                {
                    if (!sql.StartsWith("USE"))
                    {
                        using (SqlCommand cmd = context.CreateTextCommand(sql))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executest a transact SQL script on the database specified in the app.config file.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        /// <remarks>
        /// The database name is taken from the app.config file. It preparses the script,
        /// identifies the GO statements and executesb the script in chunks, just like
        /// SQL Management Studio does. It does not remove lines starting with USE!
        /// </remarks>
        public void ExecuteScript(string script)
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            
            string s = AppSettings.ConnectionString;



            csb.ConnectionString = s;
            
           
            csb.InitialCatalog = string.Empty;

            using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
            {
                cn.Open();

                foreach (string sql in SplitSqlScript(script))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, cn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// Splits SQL script into chunks base on the GO statements.
        /// </summary>
        /// <param name="script">The script to chunk up.</param>
        /// <returns>The script chunks.</returns>
        private string[] SplitSqlScript(string script)
        {
            // Look for rows starting with GO
            return script.Split(new string[] { "\r\nGO", "\nGO" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
