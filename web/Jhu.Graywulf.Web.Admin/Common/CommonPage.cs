using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Common
{
    public abstract class CommonPage : PageBase
    {
        private Entity[] entities;
        private bool checkConcurrency;
        protected CustomValidator concurrencyValidator;

        protected Guid[] Guids
        {
            get
            {
                return (Guid[])FormCacheLoad(new Guid(Request.QueryString["key"]));
            }
        }

        protected Entity[] Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        protected bool CheckConcurrency
        {
            get { return checkConcurrency; }
            set { checkConcurrency = value; }
        }

        protected long[] ConcurrencyVersions
        {
            get { return (long[])ViewState["ConcurrencyVersions"]; }
            set { ViewState["ConcurrencyVersions"] = value; }
        }

        protected CommonPage()
        {
        }

        private void InitializeMembers()
        {
            this.entities = null;
            this.checkConcurrency = false;
        }

        protected void LoadEntities()
        {
            if (entities == null)
            {
                var ef = new EntityFactory(RegistryContext);
                entities = ef.LoadEntities(Guids).ToArray();

                if (checkConcurrency)
                {
                    var cv = new long[entities.Length];

                    for (int i = 0; i < entities.Length; i++)
                    {
                        cv[i] = entities[i].ConcurrencyVersion;
                    }

                    ConcurrencyVersions = cv;
                }
            }
        }

        protected abstract void UpdateForm();

        protected abstract void ProcessForm();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEntities();
                UpdateForm();
            }
        }

        protected void ConcurrencyValidator_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (checkConcurrency)
            {
                LoadEntities();

                var cv = ConcurrencyVersions;

                for (int i = 0; i < entities.Length; i++)
                {
                    e.IsValid &= (entities[i].ConcurrencyVersion == cv[i]);
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            Validate();

            if (IsValid)
            {
                ProcessForm();
            }

            /*
            if (item.IsExisting)
            {
                Response.Redirect(item.GetDetailsUrl(), false);
            }
            else
            {
                Response.Redirect(item.GetParentDetailsUrl(), false);
            }*/
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["redirect"] != null)
            {
                Response.Redirect(Request.QueryString["redirect"], false);
            }
            else
            {
                Response.Redirect(OriginalReferer);
            }
        }
    }
}