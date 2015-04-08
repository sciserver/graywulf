using System;
using System.Collections.Generic;
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
    public partial class Discover : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Common/Discover.aspx?guid={0}", guid);
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
            var f = new EntityFactory(RegistryContext);
            item = f.LoadEntity(new Guid(Request.QueryString["guid"]));
        }

        protected virtual void UpdateForm()
        {
            Name.Text = item.Name;

            var update = new List<Entity>();
            var delete = new List<Entity>();
            var create = new List<Entity>();
            item.Discover(update, delete, create);

            EntityList.Items.Clear();

            foreach (var entity in update)
            {
                EntityList.Items.Add(String.Format("{0} - {1} ({2})", entity.Name, entity.EntityType, "to be updated"));
            }
            foreach (var entity in delete)
            {
                EntityList.Items.Add(String.Format("{0} - {1} ({2})", entity.Name, entity.EntityType, "to be deleted"));
            }
            foreach (var entity in create)
            {
                EntityList.Items.Add(String.Format("{0} - {1} ({2})", entity.Name, entity.EntityType, "to be created"));
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                LoadItem();
                item.ConcurrencyVersion = (long)ViewState["ConcurrencyVersion"];       // TODO

                // Discover again
                var update = new List<Entity>();
                var delete = new List<Entity>();
                var create = new List<Entity>();
                item.Discover(update, delete, create);

                var ef = new EntityFactory(RegistryContext);
                ef.ApplyChanges(update, delete, create);

                Response.Redirect(item.GetDetailsUrl(), false);
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