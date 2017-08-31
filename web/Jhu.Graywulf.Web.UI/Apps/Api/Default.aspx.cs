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

        public static string GetUrl(string serviceUrl)
        {
            return GetUrl() + "?serviceUrl=" + HttpUtility.UrlEncode(serviceUrl);
        }

        public string ServiceUrl
        {
            get { return Request["serviceUrl"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;

            if (!IsPostBack)
            {
                serviceList1.Items.Clear();
                serviceList1.Items.Add(new ListItem("(select service)", ""));
            }
            
            foreach (var service in application.Services)
            {
                var snattr = (ServiceNameAttribute)service.GetCustomAttributes(typeof(ServiceNameAttribute), false)[0];
                var dsattr = (DescriptionAttribute)service.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                var url = String.Format("~/Api/{0}/{1}.svc/help", snattr.Version, snattr.Name);

                if (!IsPostBack)
                {
                    var item = new ListItem()
                    {
                        Text = snattr.Name,
                        Value = url
                    };
                    serviceList1.Items.Add(item);
                }

                var li = new HtmlGenericControl("li");
                var a = new HyperLink();
                var d = new Literal();

                a.Text = snattr.Name;
                a.NavigateUrl = GetUrl(url);
                d.Text = ": " + dsattr.Description;

                li.Controls.Add(a);
                li.Controls.Add(d);
                serviceList2.Controls.Add(li);
            }

            if (!String.IsNullOrWhiteSpace(ServiceUrl))
            {
                serviceList1.SelectedValue = ServiceUrl;
                iframe.Attributes.Add("src", ServiceUrl);
                servicesForm.Visible = false;
                iframePanel.Visible = true;
            }
            else
            {
                servicesForm.Visible = true;
                iframePanel.Visible = false;
            }
        }

        protected void ServiceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(serviceList1.SelectedValue))
            {
                Response.Redirect(GetUrl(serviceList1.SelectedValue), false);
            }
        }
    }
}