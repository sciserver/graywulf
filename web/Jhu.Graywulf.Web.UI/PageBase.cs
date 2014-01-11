using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Web.UI
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        private GraywulfSchemaManager schemaManager;
        private DatasetBase myDBDataset;

        public FileFormatFactory FileFormatFactory
        {
            get { return FileFormatFactory.Create(this.Federation.FileFormatFactory); }
        }

        public StreamFactory StreamFactory
        {
            get { return StreamFactory.Create(Federation.StreamFactory); }
        }

        /// <summary>
        /// Get a schema manager that provides access to the databases
        /// of the federation plus the user's mydb
        /// </summary>
        public GraywulfSchemaManager SchemaManager
        {
            get
            {
                if (schemaManager == null)
                {
                    schemaManager = new GraywulfSchemaManager(RegistryContext, Jhu.Graywulf.Registry.AppSettings.FederationName);

                    // Load datasets from the federation
                    if (schemaManager.Datasets.IsEmpty)
                    {
                        schemaManager.Datasets.LoadAll();
                    }

                    // Add custom datasets (MYDB)
                    var mydb = MyDBDatabaseInstance;

                    var mydbds = new SqlServerDataset()
                    {
                        ConnectionString = mydb.GetConnectionString().ConnectionString,
                        Name = mydb.DatabaseDefinition.Name,
                        DefaultSchemaName = "dbo",    // **** TODO?
                        IsCacheable = false,
                        IsMutable = true,
                    };

                    schemaManager.Datasets[mydbds.Name] = mydbds;
                }

                return schemaManager;
            }
        }

        public SqlServerDataset MyDBDataset
        {
            get
            {
                if (myDBDataset == null)
                {
                    myDBDataset = SchemaManager.Datasets[MyDBDatabaseDefinition.Name];
                }

                return (SqlServerDataset)myDBDataset;
            }
        }

        // ---

        protected bool HasQueryInSession()
        {
            return Util.QueryEditorUtil.HasQueryInSession(this);
        }

        protected void SetQueryInSession(string query, int[] selection, bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.SetQueryInSession(this, query, selection, executeSelectedOnly);
        }

        protected void GetQueryFromSession(out string query, out int[] selection, out bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.GetQueryFromSession(this, out query, out selection, out executeSelectedOnly);
        }

        protected Guid LastQueryJobGuid
        {
            get { return (Guid)Session[Jhu.Graywulf.Web.UI.Constants.SessionLastQueryJobGuid]; }
            set { Session[Jhu.Graywulf.Web.UI.Constants.SessionLastQueryJobGuid] = value; }
        }

        public string SelectedSchemaObject
        {
            get { return (string)Session[Jhu.Graywulf.Web.UI.Constants.SessionSelectedSchemaObject]; }
            set { Session[Jhu.Graywulf.Web.UI.Constants.SessionSelectedSchemaObject] = value; }
        }

        // ---

        protected string GetExportUrl(JobDescription exportJob)
        {
            return String.Format(
                "~/Download/{0}",
                System.IO.Path.GetFileName(exportJob.Path));
        }

        // ---

        protected override void OnUserSignedIn()
        {
            base.OnUserSignedIn();

            if (MyDBDatabaseInstance == null)
            {
                var udii = new UserDatabaseInstanceInstaller(RegistryUser);
                var udi = udii.GenerateUserDatabaseInstance(MyDBDatabaseVersion);

                var mydb = udi.DatabaseInstance;
                mydb.Deploy();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.DataBind();

            base.OnPreRender(e);
        }
    }
}