using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
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
