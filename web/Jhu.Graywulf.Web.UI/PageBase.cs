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

namespace Jhu.Graywulf.Web.UI
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        private GraywulfSchemaManager schemaManager;
        private DatasetBase myDBDataset;

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
                    schemaManager = new GraywulfSchemaManager(RegistryContext, Jhu.Graywulf.Registry.Federation.AppSettings.FederationName);

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

        public QueryFactory QueryFactory
        {
            get
            {
                var ft = Type.GetType(Federation.QueryFactoryTypeName);
                return (QueryFactory)Activator.CreateInstance(ft, RegistryContext);
            }
        }

        public FileFormatFactory FileFormatFactory
        {
            get
            {
                var ft = Type.GetType(Federation.FileFormatFactoryTypeName);
                return (FileFormatFactory)Activator.CreateInstance(ft);
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
            get { return (Guid)Session["LastQueryJobGuid"]; }
            set { Session["LastQueryJobGuid"] = value; }
        }

        protected string SelectedSchemaObject
        {
            get { return (string)Session["SchemaSelectedObject"]; }
            set { Session["SchemaSelectedObject"] = value; }
        }

        // ---

        protected string GetExportUrl(JobDescription exportJob)
        {
            return String.Format(
                "~/Download/{0}",
                System.IO.Path.GetFileName(exportJob.Path));
        }

        // ---

        protected override void OnUserLoggedIn()
        {
            base.OnUserLoggedIn();

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