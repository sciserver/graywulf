using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.SqlCodeGen.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Jobs.Query;

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
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.DirtyRead))
            {
                jobInstance = new JobInstance(context);
                jobInstance.Guid = JobGuid;
                jobInstance.Load();
            }
        }


        protected void Download_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(fileFormat.SelectedValue))
            {
                var compression = DataFileCompression.None;
                var file = FederationContext.FileFormatFactory.CreateFileFromMimeType(fileFormat.SelectedValue);
                var query = (SqlQuery)jobInstance.Parameters["Query"].Value;
                var table = query.Output;

                var uri = new Uri(table.ObjectName + file.Description.Extension, UriKind.RelativeOrAbsolute);
                uri = FederationContext.StreamFactory.AppendCompressionExtension(uri, compression);
                file.Uri = uri;
                file.Compression = compression;

                var task = new ExportTable()
                {
                    BatchName = table.ObjectName,
                    Source = SourceTableQuery.Create(table),
                    Destination = file,
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                };

                // Set response headers
                Response.BufferOutput = false;

                if (compression != DataFileCompression.None)
                {
                    Response.ContentType = Jhu.Graywulf.IO.Constants.CompressionMimeTypes[compression];
                }
                else
                {
                    Response.ContentType = file.Description.MimeType;
                }

                Response.AppendHeader("Content-Disposition", "attachment; filename=" + uri.ToString());

                // Run export
                var sf = FederationContext.StreamFactory;
                using (var stream = sf.Open(Response.OutputStream, DataFileMode.Write, compression, DataFileArchival.None))
                {
                    file.Open(stream, DataFileMode.Write);
                    task.Execute();
                    stream.Flush();
                }

                Response.End();
            }
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
            var q = (SqlQuery)jobInstance.Parameters["Query"].Value;
            var codegen = new SqlServerCodeGenerator();
            string sql = codegen.GenerateSelectStarQuery(q.Output, 100);

            using (var cn = FederationContext.MyDBDataset.OpenConnection())
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