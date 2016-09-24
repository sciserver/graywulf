using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class CaptchaValidator : CustomValidator
    {
        protected override bool OnServerValidate(string value)
        {
            // Make sure captcha and text match
            return value == (string)Page.Session[Constants.SessionCaptcha];
        }
    }
}
