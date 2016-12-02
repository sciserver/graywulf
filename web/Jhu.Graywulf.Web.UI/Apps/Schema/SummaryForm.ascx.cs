using System;
using schema = Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public partial class SummaryForm : System.Web.UI.UserControl
    {
        protected schema::DatabaseObject databaseObject;

        public schema::DatabaseObject DatabaseObject
        {
            get { return databaseObject; }
            set
            {
                databaseObject = value;
                RefreshForm();
            }
        }

        private void RefreshForm()
        {
            if (databaseObject != null)
            {
                fullyQualifiedNameLabel.Text = Jhu.Graywulf.Schema.Constants.DatabaseObjectsName_Singular[databaseObject.ObjectType];

                datasetNameLabel.Text = databaseObject.DatasetName;
                schemaNameLabel.Text = databaseObject.SchemaName;
                objectNameLabel.Text = databaseObject.ObjectName;

                summaryLabel.Text = databaseObject.Metadata.Summary;
                remarksLabel.Text = databaseObject.Metadata.Remarks;
                remarksPanel.Visible = !String.IsNullOrEmpty(databaseObject.Metadata.Remarks);
                exampleLabel.Text = Util.DocumentationFormatter.FormatExample(databaseObject.Metadata.Example);
                examplePanel.Visible = !String.IsNullOrEmpty(databaseObject.Metadata.Example);
            }
        }
    }
}