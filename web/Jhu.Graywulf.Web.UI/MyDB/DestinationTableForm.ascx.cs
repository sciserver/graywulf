using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class DestinationTableForm : CustomUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetTableName()
        {
            if (this.Visible)
            {
                string destination = "";

                if (!String.IsNullOrWhiteSpace(schemaName.Text))
                {
                    destination += String.Format("[{0}]", schemaName.Text);
                }


                if (destination != String.Empty)
                {
                    destination += ".";
                }

                destination += String.Format("[{0}]", tableName.Text);

                return destination;
            }
            else
            {
                return null;
            }
        }

        public DestinationTable GetDestinationTable()
        {
            return ImportJob.GetDestinationTable(FederationContext, schemaName.Text, tableName.Text);
        }
    }
}