using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin
{
    /// <summary>
    /// Summary description for EntityDetailsBase
    /// </summary>
    public class MappingDetailsPageBase<T> : PageBase
        where T : Entity, new()
    {
        protected System.Web.UI.WebControls.Label name;
        protected System.Web.UI.WebControls.Label message;
        protected T item;

        protected void Page_Load(object sender, EventArgs e)
        {
            name = FindControlRecursive("Name") as Label;
            message = FindControlRecursive("Message") as Label;

            if (!IsPostBack)
            {
                CreateItem();
                LoadItem();
                UpdateForm();
            }
        }

        protected void CreateItem()
        {
            item = new T();
            item.Guid = new Guid(Request.QueryString["guid"]);
        }

        protected void LoadItem()
        {
            item.RegistryContext = RegistryContext;
            item.Load();
        }

        protected virtual void UpdateForm()
        {
            name.Text = item.Name;
        }
    }
}