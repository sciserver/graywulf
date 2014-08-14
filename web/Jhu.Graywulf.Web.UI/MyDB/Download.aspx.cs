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
    public partial class Download : CopyTablePage
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
                    RefreshFileFormatLists(false, true);
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

        private ExportTableArchive GetTableExporter()
        {
            // Check if uploaded file is an archive
            var batchName = "download";

            var tableKeys = Request.QueryString["tables"].Split(',');
            var format = Request.QueryString["format"];

            var task = new ExportTableArchive()
            {
                BatchName = batchName,
                BypassExceptions = true,
                Sources = GetSourceTables(tableKeys),
                Destinations = GetDestinationFiles(tableKeys, format),
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