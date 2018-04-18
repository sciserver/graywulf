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
    public partial class ImportOptionsForm : FederationUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void SetFileOptions(FileFormat format)
        {
            format.GenerateIdentityColumn = generateIdentityColumn.Checked;
        }

        public void SetFileOptions(DataFileBase dataFile)
        {
            dataFile.GenerateIdentityColumn = generateIdentityColumn.Checked;
        }
    }
}