using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class ListViewDataPager : DataPager
    {
        [Themeable(true)]
        public string CssClass
        {
            get { return (string)(ViewState["CssClass"]); }
            set { ViewState["CssClass"] = value; }
        }

        public ListViewDataPager()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var nppf = new NextPreviousPagerField();
            nppf.ShowNextPageButton = false;
            nppf.ShowLastPageButton = false;
            nppf.ShowFirstPageButton = false;
            nppf.ShowPreviousPageButton = true;
            Fields.Add(nppf);

            var npf = new NumericPagerField();
            npf.ButtonCount = 10;
            npf.ButtonType = ButtonType.Link;
            Fields.Add(npf);

            nppf = new NextPreviousPagerField();
            nppf.ShowNextPageButton = true;
            nppf.ShowLastPageButton = false;
            nppf.ShowFirstPageButton = false;
            nppf.ShowPreviousPageButton = false;
            Fields.Add(nppf);
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            writer.AddAttribute("class", CssClass);
            writer.RenderBeginTag("div");

            base.Render(writer);

            writer.RenderEndTag();
        }
    }
}
