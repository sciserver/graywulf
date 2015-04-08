using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Tables : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Tables.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            FederationContext.MyDBDataset.Tables.LoadAll();

            // TODO: change this to support arbitrary sorting
            var tables = FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey).ToArray();
            TableList.DataSource = tables;
        }

        protected void TableSelected_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TableList.SelectedDataKeys.Count > 0;
        }

        protected void SingleTableSelectedValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = TableList.SelectedDataKeys.Count == 1;
        }

        protected void TableList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            TableList.PageIndex = e.NewPageIndex;
            TableList.DataBind();
        }

        protected void Button_Command(object sender, CommandEventArgs e)
        {
            if (IsValid)
            {
                var objid = TableList.SelectedDataKeys.First();
                var objids = TableList.SelectedDataKeys.ToArray();

                switch (e.CommandName)
                {
                    case "View":
                        Response.Redirect(Schema.Default.GetUrl(objid), false);
                        break;
                    //case "Edit":
                    //    break;
                    case "Peek":
                        Response.Redirect(Schema.Peek.GetUrl(objid), false);
                        break;
                    case "Export":
                        Response.Redirect(MyDB.Export.GetUrl(objid), false);
                        break;
                    case "Rename":
                        Response.Redirect(MyDB.RenameObject.GetUrl(objid), false);
                        break;
                    case "Drop":
                        Response.Redirect(MyDB.DropObject.GetUrl(objids), false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        
    }
}