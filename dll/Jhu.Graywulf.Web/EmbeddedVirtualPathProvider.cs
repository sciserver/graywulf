using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace Jhu.Graywulf.Web
{
    public class EmbeddedVirtualPathProvider : VirtualPathProvider
    {
        private static readonly StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;

        private Dictionary<string, string> virtualFiles = new Dictionary<string, string>(comparer)
        {
            { "~/Captcha.aspx", "Jhu.Graywulf.Web.Captcha.aspx"},
            { "~/Feedback.aspx", "Jhu.Graywulf.Web.Feedback.aspx"},
            { "~/Error.aspx", "Jhu.Graywulf.Web.Error.aspx"},
        };

        public override bool FileExists(string virtualPath)
        {
            var apprelpath = VirtualPathUtility.ToAppRelative(virtualPath);

            if (virtualFiles.ContainsKey(apprelpath))
            {
                return true;
            }

            return base.FileExists(virtualPath);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var apprelpath = VirtualPathUtility.ToAppRelative(virtualPath);

            if (virtualFiles.ContainsKey(apprelpath))
            {
                return new EmbeddedVirtualFile(virtualPath, virtualFiles[apprelpath]);
            }

            return base.GetFile(virtualPath);
        }
    }
}
