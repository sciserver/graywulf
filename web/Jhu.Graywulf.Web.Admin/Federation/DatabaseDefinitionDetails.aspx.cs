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

            LayoutType.Text = Item.LayoutType.ToString();
            DatabaseInstanceNamePattern.Text = Item.DatabaseInstanceNamePattern;
            DatabaseNamePattern.Text = Item.DatabaseNamePattern;
            SliceCount.Text = Item.SliceCount.ToString();
            PartitionCount.Text = Item.PartitionCount.ToString();
            PartitionRangeType.Text = Item.PartitionRangeType.ToString();
            PartitionFunction.Text = Item.PartitionFunction;
        }

        protected override void InitLists()
        {
            FileGroupList.ParentEntity = Item;
            SliceList.ParentEntity = Item;
            DatabaseVersionList.ParentEntity = Item;
            DeploymentPackageList.ParentEntity = Item;

            base.InitLists();
        }

        private void ActivateDeactivateItem()
        {
            Item.DeploymentState = DeploymentState.Deployed;
            Item.RunningState = RunningState.Running;

            Item.Save();

            UpdateForm();
        }

        public override void OnButtonCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Slice":
                    Response.Redirect(SlicingWizard.GetUrl(Item.Guid), false);
                    break;
                default:
                    base.OnButtonCommand(sender, e);
                    break;
            }
        }
    }
}