using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI
{
    public class PageBase : Jhu.Graywulf.Web.PageBase
    {
        private FederationContext federationContext;

        public FederationContext FederationContext
        {
            get
            {
                if (federationContext == null)
                {
                    federationContext = new FederationContext(RegistryContext, RegistryUser);
                }

                return federationContext;
            }
        }

        // ---

        protected bool HasQueryInSession()
        {
            return Util.QueryEditorUtil.HasQueryInSession(this);
        }

        protected void SetQueryInSession(string query, int[] selection, bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.SetQueryInSession(this, query, selection, executeSelectedOnly);
        }

        protected void GetQueryFromSession(out string query, out int[] selection, out bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.GetQueryFromSession(this, out query, out selection, out executeSelectedOnly);
        }

        protected Guid LastQueryJobGuid
        {
            get { return (Guid)Session[Jhu.Graywulf.Web.UI.Constants.SessionLastQueryJobGuid]; }
            set { Session[Jhu.Graywulf.Web.UI.Constants.SessionLastQueryJobGuid] = value; }
        }

        public string SelectedSchemaObject
        {
            get { return (string)Session[Jhu.Graywulf.Web.UI.Constants.SessionSelectedSchemaObject]; }
            set { Session[Jhu.Graywulf.Web.UI.Constants.SessionSelectedSchemaObject] = value; }
        }

        // ---

        protected string GetExportUrl(ExportJob job)
        {
            return String.Format(
                "~/Download/{0}",
                System.IO.Path.GetFileName(Jhu.Graywulf.Util.UriConverter.ToFileName(job.Uri)));
        }

        protected override void OnPreRender(EventArgs e)
        {
            Page.DataBind();

            base.OnPreRender(e);
        }
    }
}