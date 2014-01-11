using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class PartitionDetails : EntityDetailsPageBase<Partition>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            From.Text = Item.From.ToString();
            To.Text = Item.To.ToString();
        }
    }
}