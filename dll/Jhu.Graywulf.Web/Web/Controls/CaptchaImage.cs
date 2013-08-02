using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class CaptchaImage : Image
    {
        public int Digits
        {
            get { return (int)(ViewState["Digits"] ?? 8); }
            set { ViewState["Digits"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            ImageUrl = 
                String.Format("{0}?width={1}&height={2}&digits={3}",
                    VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, Constants.CaptchaVirtualPath),
                    (int)Width.Value,
                    (int)Height.Value,
                    Digits);
        }
    }
}
