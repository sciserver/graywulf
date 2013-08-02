using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [ParseChildren(DefaultProperty="Elements")]
    public class Toolbar : WebControl, INamingContainer
    {
        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassDisabled
        {
            get { return (string)(ViewState["CssClassDisabled"] ?? String.Empty); }
            set { ViewState["CssClassDisabled"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassHover
        {
            get { return (string)(ViewState["CssClassHover"] ?? String.Empty); }
            set { ViewState["CssClassHover"] = value; }
        }

        public Toolbar()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override void Render(HtmlTextWriter writer)
        {
            // render style attribute
            ControlStyle.AddAttributesToRender(writer);

            writer.AddAttribute("class", CssClass);
            writer.RenderBeginTag("table");

            writer.AddAttribute("class", CssClass);
            writer.RenderBeginTag("tr");
            

            foreach (var c in Controls.Cast<Control>())
            {
                c.RenderControl(writer);
            }

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
    }
}
