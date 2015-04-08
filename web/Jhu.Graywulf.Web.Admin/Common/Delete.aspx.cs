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

namespace Jhu.Graywulf.Web.Admin.Common
{
    public partial class Delete : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return GetUrl(new[] { guid });
        }

        public static string GetUrl(Guid[] guids)
        {
            var gs = Util.UrlFormatter.ArrayToUrlList(guids);
            return String.Format("~/Common/Delete.aspx?guid={0}", gs);
        }

        protected Entity item;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadItem();
                UpdateForm();
                ViewState["ConcurrencyVersion"] = item.ConcurrencyVersion;
            }
        }

        protected void LoadItem()
        {
            var ef = new EntityFactory(RegistryContext);
            item = ef.LoadEntity(new Guid(Request.QueryString["guid"]));
        }

        protected virtual void UpdateForm()
        {
            Name.Text = item.Name;

            if (!item.CanDelete(true, true))
            {
                Message.Text = Resources.Messages.CannotDeleteEntity;
                Message.Visible = true;
                //Ok.Enabled = false;
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Validate();

            if (IsValid)
            {
                LoadItem();
                item.ConcurrencyVersion = (long)ViewState["ConcurrencyVersion"];

                try
                {
                    if (ViewState["ForceOverwrite"] != null)
                    {
                        item.Delete(true);
                    }
                    else
                    {
                        item.Delete();
                    }
                }
                catch (InvalidConcurrencyVersionException)
                {
                    Message.Text = Resources.Messages.InvalidConcurrencyVersion;
                    Message.Visible = true;
                    ViewState["ForceOverwrite"] = true;

                    return;
                }

                if (item.IsExisting)
                {
                    Response.Redirect(item.GetDetailsUrl(), false);
                }
                else
                {
                    Response.Redirect(item.GetParentDetailsUrl(), false);
                }
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["redirect"] != null)
            {
                Response.Redirect(Request.QueryString["redirect"], false);
            }
            else
            {
                LoadItem();
                Response.Redirect(item.GetDetailsUrl(), false);
            }
        }
    }
}