using System;
using System.Linq;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class ParametersView : SchemaItemView<IParameters>
    {
        public override void UpdateView()
        {
            if (Item != null)
            {
                listView.DataSource = Item.Parameters.Values.OrderBy(i => i.ID);
                listView.DataBind();
            }
        }
    }
}