using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class UsageBar : WebControl
    {
        private List<double> values;

        public List<double> Values
        {
            get { return values; }
        }

        public UsageBar()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.values = new List<double>();
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            double sum = 0;
            
            for (int i = 0; i < values.Count; i++)
            {
                sum += values[i];
            }

            AddAttributesToRender(writer);
            writer.RenderBeginTag("table");
            writer.RenderBeginTag("tr");

            for (int i = 0; i < values.Count; i++)
            {
                writer.AddStyleAttribute("width", String.Format("{0}%", (int)Math.Round(values[i] / sum * 100)));

                writer.RenderBeginTag("td");
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
            writer.RenderEndTag();

        }
    }
}
