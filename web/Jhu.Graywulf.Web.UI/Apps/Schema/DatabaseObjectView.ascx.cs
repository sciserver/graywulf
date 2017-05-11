using System;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DatabaseObjectView : SchemaItemView<DatabaseObject>
    {
        public override void UpdateView()
        {
            if (Item != null)
            {
                fullyQualifiedNameLabel.Text = Jhu.Graywulf.Schema.Constants.DatabaseObjectsName_Singular[Item.ObjectType];

                datasetNameLabel.Text = Item.DatasetName;
                schemaNameLabel.Text = Item.SchemaName;
                objectNameLabel.Text = Item.ObjectName;

                summaryLabel.Text = Item.Metadata.Summary;
                summaryPanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Summary);
                remarksLabel.Text = Item.Metadata.Remarks;
                remarksPanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Remarks);
                exampleLabel.Text = Util.DocumentationFormatter.FormatExample(Item.Metadata.Example);
                examplePanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Example);
            }
        }
    }
}