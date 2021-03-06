﻿using System;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class DatabaseObjectView : SchemaItemView<DatabaseObject>
    {
        public override void UpdateView()
        {
            if (Item != null)
            {
                objectTypeLabel.Text = Jhu.Graywulf.Sql.Schema.Constants.DatabaseObjectsName_Singular[Item.ObjectType];

                datasetNameLabel.Text = Item.DatasetName;
                schemaNameLabel.Text = Item.SchemaName;
                objectNameLabel.Text = Item.ObjectName;

                summaryLabel.Text = Item.Metadata.Summary;
                summaryPanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Summary);
                remarksLabel.Text = Item.Metadata.Remarks;
                remarksPanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Remarks);
                exampleLabel.Text = Util.DocumentationFormatter.FormatExample(Item.Metadata.Example);
                examplePanel.Visible = !String.IsNullOrEmpty(Item.Metadata.Example);

                docPage.Visible = !String.IsNullOrWhiteSpace(Item.Metadata.DocPage);
                docPage.Src = GetDocPageUrl(Item.Metadata);
            }
        }
    }
}