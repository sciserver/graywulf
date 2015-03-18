using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Jobs;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class DestinationTableForm : CustomUserControlBase
    {
        public string TableName
        {
            get
            {
                if (this.Visible && !String.IsNullOrWhiteSpace(tableName.Text))
                {
                    return tableName.Text;
                }
                else
                {
                    return null;
                }
            }
        }

        protected void TableNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string sn, tn;
                Jhu.Graywulf.Web.Api.Util.SqlParser.ParseTableName(FederationContext, tableName.Text, out sn, out tn);
                args.IsValid = true;
            }
            catch (Exception)
            {
                args.IsValid = false;
            }
        }
    }
}