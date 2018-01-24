using System;
using System.Linq;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class ColumnsView : SchemaItemView<IColumns>
    {
        public static string GetUrl(string objid)
        {
            return Default.GetUrl(Default.SchemaView.Columns, objid);
        }

        public override void UpdateView()
        {
            if (Item != null)
            {
                listView.DataSource = Item.Columns.Values.OrderBy(i => i.ID);
                listView.DataBind();
            }
        }
    }
}