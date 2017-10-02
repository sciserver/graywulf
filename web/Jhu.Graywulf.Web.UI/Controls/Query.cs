using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


namespace Jhu.Graywulf.Web.UI.Controls
{
    /// <summary>
    /// Implements functions to display a query using syntax highligher
    /// in the documentation
    /// </summary>
    [DefaultProperty("Text"), ParseChildren(false), PersistChildren(true), ToolboxData("<{0}:CodeView runat=server></{0}:CodeView>"), ControlValueProperty("Text")]
    public class Query : SyntaxHighlight
    {
        private Button edit;

        public Query()
        {
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            edit = new Button();
            edit.CssClass = "FormButton";
            edit.Text = "Try this query";
            edit.Click += new EventHandler(edit_Click);

            var p = new HtmlGenericControl("p");
            p.Style["text-align"] = "right";
            p.Controls.Add(edit);

            this.Controls.Add(p);
        }

        protected override void Page_PreRender(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Brush)) Brush = "Sql";

            base.Page_PreRender(sender, e);
        }

        void edit_Click(object sender, EventArgs e)
        {
            Util.QueryEditorUtil.SetQueryInSession(this.Page, Text, null, true);
            Page.Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Query.Default.GetUrl(), false);
        }
    }
}
