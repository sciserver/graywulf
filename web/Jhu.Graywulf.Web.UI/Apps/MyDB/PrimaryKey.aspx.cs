using System;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Jobs.SqlScript;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class PrimaryKey : FederationPageBase
    {
        public static string GetUrl(string objid)
        {
            return String.Format("~/Apps/MyDb/PrimaryKey.aspx?objid={0}", objid);
        }

        protected Jhu.Graywulf.Schema.Table table;

        protected void Page_Load(object sender, EventArgs e)
        {
            string objid = Request.QueryString["objid"];
            table = (Jhu.Graywulf.Schema.Table)FederationContext.SchemaManager.GetDatabaseObjectByKey(objid);

            if (!table.Dataset.IsMutable)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (!IsPostBack)
            {
                schemaName.Text = table.SchemaName;
                objectName.Text = table.ObjectName;

                if (table.PrimaryKey != null)
                {
                    primaryKeyName.Text = table.PrimaryKey.DisplayName;
                    primaryKeyColumns.Text = table.PrimaryKey.ColumnListDisplayString;
                    primaryKeyPanel.Visible = true;
                    dropKey.Visible = true;
                }
                else
                {
                    createKeyPanel.Visible = true;
                    ok.Visible = true;
                    RefreshColumnList();
                }
            }
        }

        private void RefreshColumnList()
        {
            columnList.Items.Clear();

            columnList.Items.Add(new ListItem("(select column", ""));

            foreach (var column in table.Columns.Values)
            {
                var li = new ListItem(column.Name, column.Name);
                columnList.Items.Add(li);
            }
        }

        private void ScheduleScriptJob(string sql, string comments)
        {
            FederationContext.Federation.ControllerMachineRole.LoadQueueInstances(false);
            var jf = SqlScriptJobFactory.Create(RegistryContext);
            var parameters = jf.CreateParameters(new[] { table.Dataset }, sql);
            var queue = FederationContext.Federation.ControllerMachineRole.QueueInstances[Registry.Constants.LongQueueName];
            var ji = jf.ScheduleAsJob(parameters, queue.GetFullyQualifiedName(), new TimeSpan(), comments);

            ji.Save();
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var cg = new SqlServerCodeGenerator();
                string sql = "";
                string columnname = null;
                var comments = String.Format("Create primary key on {0}", table.DisplayName);

                switch (primaryKeyType.SelectedValue)
                {
                    case "autogen":
                        // Add identity column
                        columnname = "__ID";

                        var column = new Column(table)
                        {
                            ColumnName = columnname,
                            DataType = DataTypes.SqlBigInt,
                            IsIdentity = true,
                        };
                        column.DataType.IsNullable = false;
                        table.Columns.TryAdd(column.Name, column);

                        sql = cg.GenerateAddColumnScript(table, new[] { column });
                        sql += Environment.NewLine + "GO" + Environment.NewLine;

                        break;
                    case "column":
                        // Select column
                        columnname = columnList.SelectedValue;
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var pk = new Index(table)
                {
                    IsPrimaryKey = true,
                    IsClustered = true,
                    IsUnique = true,
                    IsCompressed = true,
                };

                pk.GenerateIndexName(null);

                var pkc = new IndexColumn(table.Columns[columnname])
                {
                    KeyOrdinal = 0,
                };

                pk.Columns.TryAdd(pkc.Name, pkc);
                table.Indexes.TryAdd(pk.IndexName, pk);

                sql += cg.GenerateCreatePrimaryKeyScript(table);

                ScheduleScriptJob(sql, comments);

                primaryKeyForm.Visible = false;
                jobResultsForm.Visible = true;
            }
        }
        
        protected void DropKey_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                var cg = new SqlServerCodeGenerator();
                var sql = cg.GenerateDropPrimaryKeyScript(table);
                var comments = String.Format("Drop primary key on {0}", table.DisplayName);

                ScheduleScriptJob(sql, comments);

                primaryKeyForm.Visible = false;
                jobResultsForm.Visible = true;
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer, false);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Jobs.Default.GetUrl(), false);
        }

        protected void PrimaryKeyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (primaryKeyType.SelectedValue == "autogen")
            {
                columnListTable.Visible = false;
            }
            else
            {
                columnListTable.Visible = true;
            }
        }
    }
}