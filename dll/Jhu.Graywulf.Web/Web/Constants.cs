using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web
{
    public static class Constants
    {
        public const string ReturnUrl = "ReturnUrl";
        public const string OriginalReferer = "OriginalReferer";

        public const string ApplicationShortTitle = "Jhu.Graywulf.Web.ShortTitle";
        public const string ApplicationLongTitle = "Jhu.Graywulf.Web.LongTitle";
        public const string ApplicationCopyright = "Jhu.Graywulf.Web.Copyright";

        public const string SessionRegistryDatabase = "Jhu.Graywulf.Registry.Database";
        public const string SessionUsername = "Jhu.Graywulf.Web.Username";
        public const string SessionUserGuid = "Jhu.Graywulf.Web.UserGuid";
        public const string SessionContextGuid = "Jhu.Graywulf.Web.ContextGuid";
        public const string SessionCaptcha = "Jhu.Graywulf.Web.Captcha";
        
        public const string SessionException = "Jhu.Graywulf.Web.Exception";
        public const string SessionExceptionEventID = "Jhu.Graywulf.Web.ExceptionEventID";

        public const string CaptchaVirtualPath = "~/Captcha.aspx";
        public const string CaptchaMimeType = "image/jpeg";
    }
}
