using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Jobs.Query;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI
{
    public class FederationPageBase : Jhu.Graywulf.Web.UI.PageBase
    {
        private const string SessionLastQueryJobGuidKey = "Jhu.Graywulf.Web.UI.LastQueryJobGuid";

        // ---

        protected FederationPageBase()
        {
        }

        protected FederationPageBase(bool readOnly)
            : base(readOnly)
        {
        }

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

    }
}