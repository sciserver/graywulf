using System;
using System.Globalization;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Default : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Default.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataSpace.Text = Util.ByteSizeFormatter.Format(FederationContext.MyDBDataset.Statistics.DataSpace);
                UsedSpace.Text = Util.ByteSizeFormatter.Format(FederationContext.MyDBDataset.Statistics.UsedSpace);
                LogSpace.Text = Util.ByteSizeFormatter.Format(FederationContext.MyDBDataset.Statistics.LogSpace);

                var used = (double)FederationContext.MyDBDataset.Statistics.UsedSpace / FederationContext.MyDBDataset.Statistics.DataSpace;
                var free = 1 - used;

                if (used == 0)
                {
                    ProgressUsed.Visible = false;
                }
                else
                {
                    ProgressUsed.Width = String.Format(CultureInfo.InvariantCulture, "{0}%", used * 100.0);
                }

                if (free == 0)
                {
                    ProgressFree.Visible = false;
                }
                else
                {
                    ProgressFree.Width = String.Format(CultureInfo.InvariantCulture, "{0}%", free * 100.0);
                }

                ProgressUsedLabel.Text = String.Format(CultureInfo.InvariantCulture, "{0:p}", free);

                RequestSpaceLink.NavigateUrl = Jhu.Graywulf.Web.UI.Feedback.GetSpaceRequestUrl();
            }
        }
    }
}