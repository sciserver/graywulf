using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class CompressionForm : System.Web.UI.UserControl
    {
        public event EventHandler SelectionChanged;

        public DataFileCompression LastCompression
        {
            get { return (DataFileCompression)(ViewState["LastCompression"] ?? DataFileCompression.None); }
            set { ViewState["LastCompression"] = value; }
        }

        public DataFileCompression Compression
        {
            get
            {
                if (Visible)
                {
                    return (DataFileCompression)Enum.Parse(typeof(DataFileCompression), compressionList.SelectedValue);
                }
                else
                {
                    return DataFileCompression.None;
                }
            }
            set
            {
                compressionList.SelectedValue = value.ToString();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            LastCompression = Compression;
        }

        protected void CompressionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }
    }
}