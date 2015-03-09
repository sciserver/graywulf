using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class UriForm : CustomUserControlBase
    {
        public Uri Uri
        {
            get { return new Uri(uri.Text, UriKind.RelativeOrAbsolute); }
            set { uri.Text = value.OriginalString; }
        }
    }
}