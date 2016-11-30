using System;
using System.Globalization;
using System.Web;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class Default : FederationPageBase
    {
        public static string GetUrl()
        {
            return GetUrl(null);
        }

        public static string GetUrl(string datasetName)
        {
            var url = "~/Apps/MyDb/Default.aspx";

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

            datasetName.Text = userdb.Name;
            datasetUsageLabel.Text = userdb.Name;

            UpdateUsage(userdb);
        }

        private void UpdateUsage(DatasetBase userdb)
        {
            
            var used = (double)userdb.Statistics.UsedSpace / userdb.Statistics.DataSpace;
            var free = 1 - used;

            DataSpace.Text = Util.ByteSizeFormatter.Format(userdb.Statistics.DataSpace);
            UsedSpace.Text = Util.ByteSizeFormatter.Format(userdb.Statistics.UsedSpace);
            LogSpace.Text = Util.ByteSizeFormatter.Format(userdb.Statistics.LogSpace);

            if (used == 0)
            {
                usageUsed.Visible = false;
            }
            else
            {
                usageUsed.Width = String.Format(CultureInfo.InvariantCulture, "{0}%", used * 100.0);
            }

            if (free == 0)
            {
                usageFree.Visible = false;
            }
            else
            {
                usageFree.Width = String.Format(CultureInfo.InvariantCulture, "{0}%", free * 100.0);
            }

            ProgressUsedLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:p}", free);
            RequestSpaceLink.NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Common.Feedback.GetSpaceRequestUrl();
        }
    }
}