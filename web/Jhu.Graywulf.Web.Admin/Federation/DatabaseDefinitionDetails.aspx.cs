using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class DatabaseDefinitionDetails : EntityDetailsPageBase<DatabaseDefinition>
    {
        protected override void UpdateForm()
        {
            base.UpdateForm();

            SchemaSourceServerInstance.EntityReference.Value = item.SchemaSourceServerInstance;
            SchemaSourceDatabaseName.Text = item.SchemaSourceDatabaseName;
            LayoutType.Text = item.LayoutType.ToString();
            DatabaseInstanceNamePattern.Text = item.DatabaseInstanceNamePattern;
            DatabaseNamePattern.Text = item.DatabaseNamePattern;
            SliceCount.Text = item.SliceCount.ToString();
            PartitionCount.Text = item.PartitionCount.ToString();
            PartitionRangeType.Text = item.PartitionRangeType.ToString();
            PartitionFunction.Text = item.PartitionFunction;
        }

        protected override void InitLists()
        {
            FileGroupList.ParentEntity = item;
            SliceList.ParentEntity = item;
            DatabaseVersionList.ParentEntity = item;
            DeploymentPackageList.ParentEntity = item;

            base.InitLists();
        }

        private void ActivateDeactivateItem()
        {
            item.DeploymentState = DeploymentState.Deployed;
            item.RunningState = RunningState.Running;

            item.Save();

            UpdateForm();
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Slice":
                    Response.Redirect(SlicingWizard.GetUrl(item.Guid));
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }
        }
    }
}