using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class CaptchaImage : Image
    {
        protected override void OnLoad(EventArgs e)
        {
            var url = Apps.Common.Captcha.GetUrl((int)Width.Value, (int)Height.Value);
            ImageUrl = VirtualPathUtility.MakeRelative(Page.AppRelativeVirtualPath, url);
        }
    }
}
