using System;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class IndexesView : SchemaItemView<IIndexes>
    {
        public override void UpdateView()
        {
            if (Item != null)
            {
                listView.DataSource = Item.Indexes.Values;
                listView.DataBind();
            }
        }
    }
}