using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Download : FederationPageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Apps/MyDB/Download.aspx?guid={0}", guid.ToString("D"));
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            WriteToResponse();
        }

        private void WriteToResponse()
        {
            var guid = Guid.Parse(Request["guid"]);
            var task = (ExportTable)PopSessionItem(guid);
            var file = task.Destination;

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
            using (var stream = sf.Open(Response.OutputStream, DataFileMode.Write, file.Compression, DataFileArchival.None))
            {
                file.Open(stream, DataFileMode.Write);

                // TODO: make it async
                Util.TaskHelper.Wait(task.ExecuteAsync());

                stream.Flush();
            }

            Response.End();
        }
    }
}