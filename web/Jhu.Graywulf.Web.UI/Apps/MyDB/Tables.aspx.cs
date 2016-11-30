using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Tables : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string datasetName)
        {
            var url = "~/Apps/MyDb/Tables.aspx";

            if (datasetName != null)
            {
                url += "?dataset=" + HttpContext.Current.Server.UrlEncode(datasetName);
            }

            return url;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                toolbar.RefreshDatasetList();
            }

            var userdb = toolbar.Dataset;
            userdb.Tables.LoadAll(true);

            // TODO: change this to support arbitrary sorting
            var tables = userdb.Tables.Values.OrderBy(t => t.UniqueKey).ToArray();
            
            TableList.DataSource = tables;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            Page.DataBind();
        }

        protected void Toolbar_SelectedDatasetChanged(object sender, EventArgs e)
        {
            TableList.SelectedDataKeys.Clear();
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
                        Response.Redirect(MyDB.Rename.GetUrl(objid), false);
                        break;
                    case "PrimaryKey":
                        Response.Redirect(MyDB.PrimaryKey.GetUrl(objid), false);
                        break;
                    case "Drop":
                        Response.Redirect(MyDB.Drop.GetUrl(objids), false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        
    }
}