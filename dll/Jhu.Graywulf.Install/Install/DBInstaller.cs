/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using smo = Microsoft.SqlServer.Management.Smo;
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
        private SqlConnectionStringBuilder connectionString;
        private string path;
        private uint dataSize;       // MB
        private uint logSize;        // MB
        private bool simpleRecovery;

        public SqlConnectionStringBuilder ConnectionString
        {
            get { return connectionString; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public uint DataSize
        {
            get { return dataSize; }
            set { dataSize = value; }
        }

        public uint LogSize
        {
            get { return logSize; }
            set { logSize = value; }
        }

        public bool SimpleRecovery
        {
            get { return simpleRecovery; }
            set { simpleRecovery = value; }
        }

        public DBInstaller()
        {
            InitializeMembers();
        }

        public DBInstaller(string connectionString)
        {
            InitializeMembers();
            this.connectionString.ConnectionString = connectionString;
        }

        private void InitializeMembers()
        {
            this.connectionString = new SqlConnectionStringBuilder();
            this.path = null;
            this.dataSize = 0;
            this.logSize = 0;
            this.simpleRecovery = false;
        }

        private smo::Server GetSmoServer()
        {
            var s = new smo::Server(connectionString.DataSource);
            return s;
        }

        private smo::Database GetSmoDatabase()
        {
            var s = GetSmoServer();
            var db = s.Databases[connectionString.InitialCatalog];
            return db;
        }

        private smo::FileGroup AddFileGroup(smo::Database db, string name)
        {
            var fg = new smo::FileGroup(db, name);
            db.FileGroups.Add(fg);
            return fg;
        }

        private smo::DataFile AddDataFile(smo::FileGroup fg)
        {
            var name = String.Format(
                "{0}_{1}_{2}",
                fg.Parent.Name,
                fg.Name,
                fg.Files.Count + 1);
            var dir = String.IsNullOrWhiteSpace(path) ? fg.Parent.Parent.DefaultFile : path;
            var datafile = new smo::DataFile(fg, name);
            datafile.FileName = System.IO.Path.Combine(dir, name + ".mdf");
            
            if (dataSize > 0)
            {
                datafile.Size = Math.Max(dataSize, 16) * 1024;
                datafile.Growth = 0;
                datafile.GrowthType = smo::FileGrowthType.None;
            }
            else
            {
                datafile.Size = 16 * 1024;
                datafile.Growth = 10;
                datafile.GrowthType = smo.FileGrowthType.Percent;
            }

            fg.Files.Add(datafile);
            return datafile;
        }

        private smo::LogFile AddLogFile(smo::Database db)
        {
            var name = String.Format(
                "{0}_log_{1}",
                db.Name,
                db.LogFiles.Count + 1);
            var dir = String.IsNullOrWhiteSpace(path) ? db.Parent.DefaultLog : path;
            var logfile = new smo::LogFile(db, name);
            logfile.FileName = System.IO.Path.Combine(dir, name + ".ldf");

            if (logSize > 0)
            {
                logfile.Size = Math.Max(logSize, 16) * 1024;
                logfile.Growth = 0;
                logfile.GrowthType = smo::FileGrowthType.None;
            }
            else
            {
                logfile.Size = 16 * 1024;
                logfile.Growth = 10;
                logfile.GrowthType = smo.FileGrowthType.Percent;
            }

            db.LogFiles.Add(logfile);
            return logfile;
        }

        /// <summary>
        /// Creates a new database on the server and with the name specified in the app.config file.
        /// </summary>
        public void CreateDatabase()
        {
            var dbname = connectionString.InitialCatalog;
            var s = GetSmoServer();
            var db = new smo::Database(s, dbname);

            db.RecoveryModel = simpleRecovery ? smo::RecoveryModel.Simple : smo::RecoveryModel.Full;
            db.Collation = "SQL_Latin1_General_CP1_CI_AS";

            var fg = AddFileGroup(db, "PRIMARY");
            var datafile = AddDataFile(fg);
            var logfile = AddLogFile(db);
            
            db.Create();
        }

        /// <summary>
        /// Drops the database on the server specified in the app.config file with the name passed as a parameter.
        /// </summary>
        /// <remarks>
        /// This function is used for deleting test databases too, not just the cluster schema database.
        /// </remarks>
        public void DropDatabase(bool checkExistence)
        {
            var s = GetSmoServer();
            var exists = s.Databases[connectionString.InitialCatalog] != null;

            if (checkExistence && !exists)
            {
                // *** TODO
                throw new Exception("Database does not exist.");
            }

            if (exists)
            {
                s.KillDatabase(connectionString.InitialCatalog);
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
        public virtual void CreateSchema()
        {
        }

        public void AddUser(string username, string role)
        {
            var s = GetSmoServer();

            if (s.Logins[username] == null)
            {
                var l = new smo::Login(s, username);
                l.LoginType = smo.LoginType.WindowsGroup;
                l.Create();
            }

            var db = GetSmoDatabase();
            var u = new smo::User(db, username)
            {
                Login = username,
            };
            u.Create();
            u.AddToRole(role);
        }

        protected void ExecuteSqlFile(string filename)
        {
            var sql = File.ReadAllText(filename);
            ExecuteSqlScript(sql);
        }

        protected void ExecuteSqlScript(string sql)
        {
            using (var cn = new SqlConnection(connectionString.ConnectionString))
            {
                cn.Open();

                string[] scripts = SplitSqlScript(sql);

                foreach (string q in scripts)
                {
                    if (!String.IsNullOrWhiteSpace(q))
                    {
                        using (SqlCommand cmd = new SqlCommand(q, cn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        protected string GetFileAsHex(string filename)
        {
            // Load enum assembly as a binary and convert to hex
            var buffer = File.ReadAllBytes(filename);
            var sb = new StringBuilder();

            sb.Append("0x");

            for (int i = 0; i < buffer.Length; i++)
            {
                sb.AppendFormat("{0:X2}", buffer[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Splits SQL script into chunks base on the GO statements.
        /// </summary>
        /// <param name="script">The script to chunk up.</param>
        /// <returns>The script chunks.</returns>
        protected string[] SplitSqlScript(string script)
        {
            // Look for rows starting with GO
            return script.Split(new string[] { "\r\nGO", "\nGO", "\r\ngo", "\ngo" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
