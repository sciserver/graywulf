using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.UI
{
    public static class Constants
    {
        public const string ReturnUrl = "ReturnUrl";
        public const string OriginalReferer = "OriginalReferer";

        public const string ApplicationDomainName = "Jhu.Graywulf.Web.DomainName";
        public const string ApplicationShortTitle = "Jhu.Graywulf.Web.ShortTitle";
        public const string ApplicationLongTitle = "Jhu.Graywulf.Web.LongTitle";
        public const string ApplicationCopyright = "Jhu.Graywulf.Web.Copyright";

        public const string SessionRegistryDatabase = "Jhu.Graywulf.Registry.Database";
        public const string SessionPrincipal = "Jhu.Graywulf.Security.Principal";
        public const string SessionTempPrincipal = "Jhu.Graywulf.Security.TempUser";
        public const string SessionCaptcha = "Jhu.Graywulf.Web.Captcha";
        public const string SessionAuthenticator = "Jhu.Graywulf.Security.Authenticator";

        public const string SessionClusterGuid = "Jhu.Graywulf.Web.Cluster";
        public const string SessionDomainGuid = "Jhu.Graywulf.Web.Domain";
        public const string SessionFederationGuid = "Jhu.Graywulf.Web.Federation";
        
        public const string SessionException = "Jhu.Graywulf.Web.Exception";
        public const string SessionExceptionEventID = "Jhu.Graywulf.Web.ExceptionEventID";

        public const string CaptchaVirtualPath = "~/Captcha.aspx";
        public const string CaptchaMimeType = "image/jpeg";
    }
}
