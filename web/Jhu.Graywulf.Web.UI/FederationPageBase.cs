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
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI
{
    public class FederationPageBase : Jhu.Graywulf.Web.UI.PageBase
    {
        private const string SessionSelectedSchemaObject = "Jhu.Graywulf.Web.UI.SelectedSchemaObject";
        private const string SessionLastQueryJobGuid = "Jhu.Graywulf.Web.UI.LastQueryJobGuid";

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

        public void SetQueryInSession(string query, int[] selection, bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.SetQueryInSession(this, query, selection, executeSelectedOnly);
        }

        protected void GetQueryFromSession(out string query, out int[] selection, out bool executeSelectedOnly)
        {
            Util.QueryEditorUtil.GetQueryFromSession(this, out query, out selection, out executeSelectedOnly);
        }

        protected Guid LastQueryJobGuid
        {
            get { return (Guid)Session[SessionLastQueryJobGuid]; }
            set { Session[SessionLastQueryJobGuid] = value; }
        }

        public string SelectedSchemaObject
        {
            get { return (string)Session[SessionSelectedSchemaObject]; }
            set { Session[SessionSelectedSchemaObject] = value; }
        }
    }
}