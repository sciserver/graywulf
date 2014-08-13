using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Download : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Download.aspx";
        }

        public static string GetUrl(string objid)
        {
            return String.Format("~/MyDb/Download.aspx?objid={0}", objid);
        }

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if file is requested
            if (Request.QueryString["download"] != null)
            {
                var filename = String.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0}_download_{1:yyMMddHHmmss}.zip",
                    User.Identity.Name,
                    DateTime.Now);

                Response.Expires = -1;
                Response.ContentType = IO.Constants.MimeTypeZip;
                Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", filename));

                var exporter = GetTableExporter();
                exporter.Execute();

                Response.OutputStream.Flush();
                Response.End();
            }
            else
            {
                if (!IsPostBack)
                {
                    RefreshFileFormatLists();
                    RefreshTableList();
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                DownloadLink.HRef = String.Format(
                    "Download.aspx?download=true&tables={0}&format={1}",
                    Request.Form[TableList.UniqueID],
                    Request.Form[FileFormatList.UniqueID]);
                
                DownloadForm.Visible = false;
                ResultsForm.Visible = true;
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            Response.Redirect(Jhu.Graywulf.Web.UI.MyDB.Tables.GetUrl());
        }

        #endregion

        private void RefreshFileFormatLists()
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.CanWrite)
                {
                    var li = new ListItem(df.DisplayName, df.Extension);
                    FileFormatList.Items.Add(li);
                }
            }
        }

        private void RefreshTableList()
        {
            FederationContext.MyDBDataset.Tables.LoadAll();

            foreach (var table in FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                TableList.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }
        }


        private SourceTableQuery[] GetSources()
        {
            var tableKeys = Request.QueryString["tables"].Split(',');
            var sources = new SourceTableQuery[tableKeys.Length];

            for (int i = 0; i < tableKeys.Length; i++)
            {
                var table = FederationContext.MyDBDataset.Tables[tableKeys[i]];

                // TODO: maybe set a row maximum here
                sources[i] = SourceTableQuery.Create(table);
            }

            return sources;
        }

        private DataFileBase[] GetDestinations()
        {
            var tableKeys = Request.QueryString["tables"].Split(',');
            var format = Request.QueryString["format"];
            var destinations = new DataFileBase[tableKeys.Length];

            for (int i = 0; i < tableKeys.Length; i++)
            {
                var table = FederationContext.MyDBDataset.Tables[tableKeys[i]];

                destinations[i] = FederationContext.FileFormatFactory.CreateFileFromExtension(format);

                // TODO: maybe change file naming logic here?
                destinations[i].Uri = Util.UriConverter.FromFilePath(table.TableName + destinations[i].Description.Extension);
            }

            return destinations;
        }

        private ExportTableArchive GetTableExporter()
        {
            // Check if uploaded file is an archive
            var batchName = "download";

            var task = new ExportTableArchive()
            {
                BatchName = batchName,
                BypassExceptions = true,
                Sources = GetSources(),
                Destinations = GetDestinations(),
                StreamFactoryType = RegistryContext.Federation.StreamFactory,
                FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
            };

            // Wrap response stream into a zip file
            var stream = FederationContext.StreamFactory.Open(
                Response.OutputStream,
                DataFileMode.Write,
                DataFileCompression.None,
                DataFileArchival.Zip);

            task.Open(stream);

            return task;
        }
    }
}