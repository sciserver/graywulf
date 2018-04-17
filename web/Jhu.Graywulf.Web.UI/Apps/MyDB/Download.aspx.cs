using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.UI;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Download : FederationPageBase
    {
        private Stream stream;
        private ExportTable task;
        private DataFileBase file;

        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Apps/MyDB/Download.aspx?guid={0}", guid.ToString("D"));
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeExportTask();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(WriteToResponseAsync));
        }

        private void InitializeExportTask()
        {
            var guid = Guid.Parse(Request["guid"]);
            task = (ExportTable)PopSessionItem(guid);
            file = task.Destination;

            // Set response headers
            Response.BufferOutput = false;

            if (file.Compression != DataFileCompression.None)
            {
                Response.ContentType = Jhu.Graywulf.IO.Constants.CompressionMimeTypes[file.Compression];
            }
            else
            {
                Response.ContentType = file.Description.MimeType;
            }

            Response.AppendHeader("Content-Disposition", "attachment; filename=" + file.Uri.ToString());

            // Run export
            var sf = FederationContext.StreamFactory;
            stream = sf.Open(Response.OutputStream, DataFileMode.Write, file.Compression, DataFileArchival.None);
            file.Open(stream, DataFileMode.Write);
        }
            
        private async Task WriteToResponseAsync()
        {
            await task.ExecuteAsync();
            stream.Flush();
            stream.Dispose();
            Response.End();
        }
    }
}