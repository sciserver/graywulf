using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class SciDriveForm : System.Web.UI.UserControl
    {
        public Uri Uri
        {
            get { return new Uri(uri.Text, UriKind.RelativeOrAbsolute); }
            set { uri.Text = value.OriginalString; }
        }
    }
}