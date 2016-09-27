using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class FileFormatForm : FederationUserControlBase
    {
        public DataFileMode FileMode
        {
            get { return (DataFileMode)(ViewState["FileMode"] ?? DataFileMode.Read); }
            set { ViewState["FileMode"] = value; }
        }

        public bool Required
        {
            get { return (bool)(ViewState["Required"] ?? false); }
            set { ViewState["Required"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshFileFormatLists();
            }
        }

        protected void RefreshFileFormatLists()
        {
            fileFormatList.Items.Clear();

            if (Required)
            {
                fileFormatList.Items.Add(new ListItem("(select file format)", ""));
            }
            else
            {
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

            fileFormatListRequiredValidator.Enabled = Required;
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
            };

            return format;
        }

        public DataFileBase GetDataFile(Uri uri)
        {
            // This can figure out the file type based on a URI

            var format = GetFormat();
            var file = format.GetDataFile(FederationContext, uri);
            return file;
        }

        public DataFileBase GetDataFile()
        {
            // This is used when the user doesn't need to specify a URI but
            // the file format is set directly

            var file = FederationContext.FileFormatFactory.CreateFileFromMimeType(fileFormatList.SelectedValue);
            return file;
        }

    }
}