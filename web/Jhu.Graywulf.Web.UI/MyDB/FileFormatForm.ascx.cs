using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class FileFormatForm : CustomUserControlBase
    {
        public DataFileMode FileMode
        {
            get { return (DataFileMode)(ViewState["FileMode"] ?? DataFileMode.Read); }
            set { ViewState["FileMode"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshFileFormatLists();
            }
        }

        protected void FileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void RefreshFileFormatLists()
        {
            if (fileFormatList != null)
            {
                fileFormatList.Items.Clear();
                fileFormatList.Items.Add(new ListItem("(detect automatically)", ""));
            }

            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (FileMode == DataFileMode.Read && df.CanRead || FileMode == DataFileMode.Write && df.CanWrite)
                {
                    var li = new ListItem(df.DisplayName, df.MimeType);

                    if (fileFormatList != null)
                    {
                        fileFormatList.Items.Add(li);
                    }
                }
            }
        }

        public FileFormat GetFormat()
        {
            string mimeType = null;

            if (!String.IsNullOrWhiteSpace(fileFormatList.SelectedValue))
            {
                mimeType = fileFormatList.SelectedValue;
            }

            var format = new FileFormat()
            {
                MimeType = mimeType,
                GenerateIdentity = generateIdentity.Checked,
            };

            return format;
        }

        public DataFileBase GetDataFile(Uri uri)
        {
            var format = GetFormat();
            var file = format.GetDataFile(FederationContext, uri);

            return file;
        }
    }
}