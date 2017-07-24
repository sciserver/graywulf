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
    public class EntityDetailsPageBase<T> : PageBase, IEntityForm
        where T : Entity, new()
    {
        private T item;

        public T Item
        {
            get { return item; }
        }

        Entity IEntityForm.Item
        {
            get { return item; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CreateItem();
            LoadItem();

            if (!IsPostBack)
            {
                UpdateForm();
            }

            InitLists();
        }

        protected virtual void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        private void CreateItem()
        {
            item = new T();
            item.Guid = new Guid(Request.QueryString["guid"]);
        }

        private void LoadItem()
        {
            item.RegistryContext = RegistryContext;
            item.Load();
        }

        protected virtual void UpdateForm()
        {
        }

        protected virtual void InitLists()
        {
        }

        public virtual void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Edit":
                    EditItem();
                    break;
                case "Delete":
                    DeleteItem();
                    break;
                case "ToggleShowHide":
                    if (item.Hidden)
                    {
                        ShowItem();
                    }
                    else
                    {
                        HideItem();
                    }
                    break;
                case "ToggleDeploymentState":
                    if (item.DeploymentState == DeploymentState.Deployed)
                    {
                        UndeployItem();
                    }
                    else
                    {
                        DeployItem();
                    }
                    break;
                case "ToggleRunningState":
                    if (item.RunningState == RunningState.Running)
                    {
                        StopItem();
                    }
                    else
                    {
                        StartItem();
                    }
                    break;
                case "Discover":
                    DiscoverItem();
                    break;
                case "Serialize":
                    SerializeItem();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            OnButtonCommand(sender, e);
        }

        protected void EditItem()
        {
            Response.Redirect(item.GetFormUrl(), false);
        }

        protected void DeleteItem()
        {
            Response.Redirect(Jhu.Graywulf.Web.Admin.Common.Delete.GetUrl(item.Guid), false);
        }

        protected void ShowItem()
        {
            item.Show();
            UpdateForm();
        }

        protected void HideItem()
        {
            item.Hide();
            UpdateForm();
        }

        protected void SerializeItem()
        {
            Response.Redirect(Jhu.Graywulf.Web.Admin.Common.Serialize.GetUrl(item.Guid), false);
        }

        protected void DiscoverItem()
        {
            Response.Redirect(Common.Discover.GetUrl(item.Guid), false);
        }

        protected void DeployItem()
        {
            item.Deploy();
            UpdateForm();
        }

        protected void UndeployItem()
        {
            item.Undeploy();
            UpdateForm();
        }

        protected void StartItem()
        {
            item.Start();
            UpdateForm();
        }

        protected void StopItem()
        {
            item.Stop();
            UpdateForm();
        }
    }
}