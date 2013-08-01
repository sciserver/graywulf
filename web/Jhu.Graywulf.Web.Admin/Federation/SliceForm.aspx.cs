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
    public partial class SliceForm : EntityFormPageBase<Slice>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            From.Text = item.From.ToString();
            To.Text = item.To.ToString();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            item.From = long.Parse(From.Text);
            item.To = long.Parse(To.Text);
        }
    }
}