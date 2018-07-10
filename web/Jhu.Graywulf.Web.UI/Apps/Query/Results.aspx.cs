using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.QueryGeneration.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Sql.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.Apps.Query
{
    public partial class Results : FederationPageBase
    {
        public static string GetUrl(Guid jobGuid)
        {
            return String.Format("~/Apps/Query/Results.aspx?guid={0}", jobGuid);
        }

        private JobInstance jobInstance;

        protected Guid JobGuid
        {
            get { return Guid.Parse(Request["guid"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!IsPostBack)
            {
                RefreshFormatList();
            }

            // Use lower isolation level for polling
            using (var context = ContextManager.Instance.CreateContext(Registry.TransactionMode.DirtyRead))
            {
                var ef = new EntityFactory(context);
                jobInstance = ef.LoadEntity<JobInstance>(JobGuid);
            }
        }


        protected void Download_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();

            /*
            if (!String.IsNullOrWhiteSpace(fileFormat.SelectedValue))
            {
                var query = (SqlQuery)jobInstance.Parameters["Query"].Value;

                // TODO:
                // var table = query.Output;
                var compression = DataFileCompression.None;
                var file = FederationContext.FileFormatFactory.CreateFileFromMimeType(fileFormat.SelectedValue);
                var uri = StreamFactory.CombineFileExtensions("", table.ObjectName, file.Description.Extension, DataFileArchival.None, compression);
                file.Uri = new Uri(uri, UriKind.Relative);
                file.Compression = compression;

                var task = new ExportTable()
                {
                    BatchName = table.ObjectName,
                    Source = SourceTableQuery.Create(table),
                    Destination = file,
                    StreamFactoryType = FederationContext.Federation.StreamFactory,
                    FileFormatFactoryType = FederationContext.Federation.FileFormatFactory,
                };

                var guid = PushSessionItem(task);
                Response.Redirect(Apps.MyDB.Download.GetUrl(guid), false);
            }*/
        }

        protected void RefreshFormatList()
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            fileFormat.Items.Clear();

            foreach (var df in dfs)
            {
                var li = new ListItem(df.DisplayName, df.MimeType);
                fileFormat.Items.Add(li);
            }
        }

        protected void RenderResults()
        {
            throw new NotImplementedException();

            /*
            var q = (SqlQuery)jobInstance.Parameters["Query"].Value;
            var codegen = new SqlServerCodeGenerator();
            string sql = codegen.GenerateSelectStarQuery(q.Output, 100);

            using (var cn = q.Destination.Dataset.OpenConnection())
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                    {
                        RenderTable(dr);
                    }
                }
            }
            */
        }

        private void RenderTable(IDataReader dr)
        {
            // TODO: merge this with peek.aspx
            var output = Response.Output;

            output.WriteLine("<table border=\"1\" cellspacing=\"0\" style=\"border-collapse:collapse\">");

            // header
            output.WriteLine("<tr>");

            for (int i = 0; i < dr.FieldCount; i++)
            {
                output.WriteLine("<td class=\"header\" nowrap>{0}<br />{1}</td>", dr.GetName(i), dr.GetDataTypeName(i));
            }

            output.WriteLine("</tr>");

            while (dr.Read())
            {
                output.WriteLine("<tr>");

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    output.WriteLine("<td nowrap>{0}</td>", dr.GetValue(i).ToString());
                }

                output.WriteLine("</tr>");
            }


            output.WriteLine("</table>");
        }
    }
}