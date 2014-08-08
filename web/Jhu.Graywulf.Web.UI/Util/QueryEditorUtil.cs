using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Jhu.Graywulf.Util
{
    public static class QueryEditorUtil
    {
        public static bool HasQueryInSession(Page page)
        {
            return page.Session["Query"] != null;
        }

        public static void SetQueryInSession(Page page, string query, int[] selection, bool executeSelectedOnly)
        {
            if (selection == null)
            {
                selection = new[] { 0, 0, 0, 0 };
            }

            page.Session["Query"] = query;
            page.Session["QuerySelection"] = selection;
            page.Session["QuerySelectedOnly"] = executeSelectedOnly;
        }

        public static void GetQueryFromSession(Page page, out string query, out int[] selection, out bool executeSelectedOnly)
        {
            query = (string)page.Session["Query"];
            selection = (int[])page.Session["QuerySelection"];
            executeSelectedOnly = (bool)page.Session["QuerySelectedOnly"];
        }
    }
}