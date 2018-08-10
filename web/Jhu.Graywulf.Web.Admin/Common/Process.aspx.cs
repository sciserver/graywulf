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
    public partial class Process : CommonPage
    {
        private List<Entity> update;
        private List<Entity> error;

        public static string GetUrl(Operation op, Guid key)
        {
            return String.Format("~/Common/Process.aspx?op={0}&key={1}", op, key);
        }

        protected Operation Operation
        {
            get { return (Operation)Enum.Parse(typeof(Operation), Request.QueryString["op"]); }
        }

        protected override RegistryContext OnCreateRegistryContext()
        {
            var application = (Jhu.Graywulf.Web.UI.UIApplicationBase)HttpContext.Current.ApplicationInstance;
            return application.CreateRegistryContext(Registry.TransactionMode.ReadWrite);
        }

        protected override void UpdateForm()
        {
            var op = Operation;
            LoadEntities();

            update = new List<Entity>();
            error = new List<Entity>();

            foreach (var e in Entities)
            {
                if (e.IsOperationSupported(op))
                {
                    try
                    {
                        e.RunOperation(op);
                        update.Add(e);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            System.Diagnostics.Debugger.Break();
                        }
#endif
                        error.Add(e);
                    }
                }
            }

            if (error.Count > 0)
            {
                errorListDiv.Visible = true;
                errorList.Items.Clear();

                foreach (var entity in error)
                {
                    errorList.Items.Add(String.Format("{0} - {1} ({2})", entity.Name, entity.EntityType, "error"));
                }
            }

            updateList.Items.Clear();

            foreach (var entity in update)
            {
                updateList.Items.Add(String.Format("{0} - {1} ({2})", entity.Name, entity.EntityType, "processed"));
            }
        }

        protected override void ProcessForm()
        {
            Response.Redirect(OriginalReferer);
        }
    }
}