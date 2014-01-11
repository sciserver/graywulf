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
    public partial class PartitionForm : EntityFormPageBase<Partition>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            From.Text = Item.From.ToString();
            To.Text = Item.To.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.From = long.Parse(From.Text);
            Item.To = long.Parse(To.Text);
        }
    }
}