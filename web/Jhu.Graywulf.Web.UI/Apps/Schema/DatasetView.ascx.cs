using System;
using System.IO;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DatasetView : SchemaItemView<DatasetBase>
    {
        public override void UpdateView()
        {
            datasetName.Text = Item.Name;

            urlPar.Visible = !String.IsNullOrWhiteSpace(Item.Metadata.Url);
            urlLink.NavigateUrl = url.Text = Item.Metadata.Url;

            summaryPanel.Visible = !String.IsNullOrWhiteSpace(Item.Metadata.Summary);
            summary.Text = Item.Metadata.Summary;

            remarksPanel.Visible = !String.IsNullOrWhiteSpace(Item.Metadata.Remarks);
            remarks.Text = Item.Metadata.Remarks;

            docPage.Visible = !String.IsNullOrWhiteSpace(Item.Metadata.DocPage);
            docPage.Src = GetDocPageUrl(Item.Metadata);
        }
    }
}