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
    }
}