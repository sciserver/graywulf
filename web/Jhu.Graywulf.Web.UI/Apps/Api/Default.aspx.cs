using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.UI.Apps.Api
{
    public partial class Default : PageBase
    {
        public static string GetUrl()
        {
            return "~/Apps/Api/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            foreach (var service in application.Services)
            {
                var snattr = (ServiceNameAttribute)service.GetCustomAttributes(typeof(ServiceNameAttribute), false)[0];
                var dsattr = (DescriptionAttribute)service.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                
                var li = new HtmlGenericControl("li");
                var a = new HyperLink();
                var d = new Literal();

                a.Text = snattr.Name;
                a.NavigateUrl = String.Format("~/Api/{0}/{1}.svc/help", snattr.Version, snattr.Name);
                d.Text = ": " + dsattr.Description;

                li.Controls.Add(a);
                li.Controls.Add(d);
                serviceList.Controls.Add(li);
            }
        }
    }
}