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
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin
{
    /// <summary>
    /// Summary description for EntityFormBase
    /// </summary>
    public class EntityFormPageBase<T> : PageBase, IEntityForm
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
        
        public Controls.EntityForm EntityForm
        {
            get { return ((EntityForm)Page.Master).EntityFormControl; }
        }

        public Label Message
        {
            get { return (Label)FindControlRecursive("Message"); }
        }
        
        protected virtual void Page_Load(object sender, EventArgs e)
        {
            LoadItem();

            if (!IsPostBack)
            {
                OnUpdateForm();
                ViewState["ConcurrencyVersion"] = item.ConcurrencyVersion;
            }
        }

        private void LoadItem()
        {
            if (Request.QueryString["guid"] != null)
            {
                var ef = new EntityFactory(RegistryContext);
                item = ef.LoadEntity<T>(new Guid(Request.QueryString["guid"]));
            }
            else
            {
                item = new T();
                item.RegistryContext = RegistryContext;
                item.ParentReference.Guid = new Guid(Request.QueryString["parentGuid"]);
            }

            OnItemLoaded(!item.IsExisting && !IsPostBack);
        }

        protected virtual void OnUpdateForm()
        {
        }

        protected virtual void OnSaveForm()
        {
        }

        protected virtual void OnItemLoaded(bool newentity)
        {
        }

        protected virtual void OnSaveFormCompleted(bool newentity)
        {
        }

        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Ok":
                    SaveForm();
                    break;
                case "Cancel":
                    CancelForm();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void SaveForm()
        {
            if (IsValid)
            {
                OnSaveForm();

                item.ConcurrencyVersion = (long)ViewState["ConcurrencyVersion"];
                item.RegistryContext = RegistryContext;

                try
                {
                    bool newentity = !item.IsExisting;

                    if (ViewState["ForceOverwrite"] != null)
                    {
                        item.Save(true);
                    }
                    else
                    {
                        item.Save();
                    }

                    OnSaveFormCompleted(newentity);
                }
                catch (Registry.DuplicateNameException)
                {
                    Message.Text = Resources.Messages.DuplicateName;
                    Message.Visible = true;
                    ViewState["ForceOverwrite"] = false;

                    return;
                }
                catch (InvalidConcurrencyVersionException)
                {
                    Message.Text = Resources.Messages.InvalidConcurrencyVersion;
                    Message.Visible = true;
                    ViewState["ForceOverwrite"] = true;

                    return;
                }
                catch (LockingCollisionException)
                {
                    Message.Text = Resources.Messages.LockingCollision;
                    Message.Visible = true;
                    ViewState["ForceOverwrite"] = true;

                    return;
                }

                Response.Redirect(item.GetDetailsUrl(), false);
            }
        }

        protected void CancelForm()
        {
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
}