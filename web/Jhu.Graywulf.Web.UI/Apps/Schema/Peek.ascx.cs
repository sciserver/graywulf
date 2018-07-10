using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using Jhu.Graywulf.Sql.Schema;
using schema = Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.QueryGeneration;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Data;


namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class Peek : SchemaItemView<TableOrView>
    {
        private DbConnection databaseConnection;
        private DbTransaction databaseTransaction;
        private SmartCommand databaseCommand;
        private ISmartDataReader dataReader;
        
        public static string GetUrl(string objid)
        {
            return Default.GetUrl(Default.SchemaView.Peek, objid);
        }

        public override void UpdateView()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            databaseConnection = null;
            databaseTransaction = null;
            databaseCommand = null;
            dataReader = null;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (Visible)
            {
                Page.RegisterAsyncTask(new PageAsyncTask(ExecuteQuery));
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (dataReader != null)
            {
                dataReader.Close();
                dataReader.Dispose();
            }

            if (databaseCommand != null)
            {
                databaseCommand.Dispose();
            }

            if (databaseTransaction != null)
            {
                databaseTransaction.Commit();
                databaseTransaction.Dispose();
            }

            if (databaseConnection != null)
            {
                databaseConnection.Close();
                databaseConnection.Dispose();
            }
        }

        private async Task ExecuteQuery(CancellationToken cancellationToken)
        {
            var tableOrView = Item;
            var codegen = QueryGeneratorFactory.CreateCodeGenerator(tableOrView.Dataset);
            var sql = codegen.GenerateSelectStarQuery(tableOrView, 100);

            // To peek into data, pick a server
            // Do not do this for mutable databasets such as MyDB
            if (tableOrView.Dataset is GraywulfDataset && !tableOrView.Dataset.IsMutable)
            {
                var ds = (GraywulfDataset)tableOrView.Dataset;

                if (!ds.DatabaseDefinitionReference.IsEmpty)
                {
                    var dd = ds.DatabaseDefinitionReference.Value;
                    dd.RegistryContext = RegistryContext;

                    var di = dd.GetRandomDatabaseInstance(Registry.Constants.ProdDatabaseVersionName);
                    var cstr = di.GetConnectionString();

                    databaseConnection = new SqlConnection(cstr.ConnectionString);
                    await databaseConnection.OpenAsync(cancellationToken);
                }
            }

            if (databaseConnection == null)
            {
                databaseConnection = await tableOrView.Dataset.OpenConnectionAsync(cancellationToken);
            }

            databaseTransaction = databaseConnection.BeginTransaction(IsolationLevel.ReadUncommitted);

            databaseCommand = new SmartCommand(tableOrView.Dataset, databaseConnection.CreateCommand())
            {
                Connection = databaseConnection,
                Transaction = databaseTransaction,
                CommandText = sql,
                CommandType = CommandType.Text
            };

            dataReader = await databaseCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderTable(writer, dataReader);
        }

        private void RenderTable(TextWriter writer, ISmartDataReader dr)
        {
            writer.WriteLine("<div class=\"dock-fill dock-scroll\">");
            writer.WriteLine("<table border=\"1\" cellspacing=\"0\" style=\"border-collapse:collapse\">");

            // header
            writer.WriteLine("<tr>");

            var columns = dr.Columns;

            for (int i = 0; i < columns.Count; i++)
            {
                writer.WriteLine(
                    "<td class=\"header\" nowrap>{0}<br />{1}</td>",
                    columns[i].Name,
                    columns[i].DataType.TypeNameWithLength);
            }

            writer.WriteLine("</tr>");

            // Rows
            while (dr.Read())
            {
                writer.WriteLine("<tr>");

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    writer.WriteLine("<td nowrap>{0}</td>", dr.GetValue(i).ToString());
                }

                writer.WriteLine("</tr>");
            }

            // Footer
            writer.WriteLine("</table>");
            writer.WriteLine("</div>");
        }
    }
}