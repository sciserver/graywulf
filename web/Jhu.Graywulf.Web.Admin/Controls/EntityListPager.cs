using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public class EntityListPager : DataPager
    {
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        /*
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (HasControls())
            {
                foreach (Control child in Controls)
                {
                    var item = child as DataPagerFieldItem;

                    if (item == null || !item.HasControls())
                    {
                        child.RenderControl(writer);
                    }
                    else
                    {
                        foreach (Control button in item.Controls)
                        {
                            var space = button as LiteralControl;

                            if (space != null && space.Text == "&nbsp;")
                            {
                                continue;
                            }

                            writer.RenderBeginTag(HtmlTextWriterTag.Li);
                            button.RenderControl(writer);
                            writer.RenderEndTag();
                        }
                    }
                }
            }
        }*/
    }
}