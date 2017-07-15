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
    public class Query : System.Web.UI.WebControls.WebControl, ITextControl
    {
        private LinkButton edit;
        private bool textSetByAddParsedSubObject;

        [Bindable(true), Localizable(true), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get { return (string)ViewState["Text"] ?? ""; }
            set { ViewState["Text"] = value; }
        }

        protected override void AddParsedSubObject(object obj)
        {
            if (this.HasControls())
            {
                base.AddParsedSubObject(obj);
            }
            else if (obj is LiteralControl)
            {
                if (this.textSetByAddParsedSubObject)
                {
                    this.Text = this.Text + ((LiteralControl)obj).Text;
                }
                else
                {
                    this.Text = ((LiteralControl)obj).Text;
                }
                this.textSetByAddParsedSubObject = true;
            }
            else
            {
                string text = this.Text;
                if (text.Length != 0)
                {
                    this.Text = string.Empty;
                    base.AddParsedSubObject(new LiteralControl(text));
                }
                base.AddParsedSubObject(obj);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            edit = new LinkButton();
            edit.Text = "Try this query";
            edit.Click += new EventHandler(edit_Click);

            this.Controls.Add(edit);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "core", VirtualPathUtility.ToAbsolute("~/Scripts/SyntaxHighlighter/scripts/shCore.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "autoloader", VirtualPathUtility.ToAbsolute("~/Scripts/SyntaxHighlighter/scripts/shAutoLoader.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "brushsql", VirtualPathUtility.ToAbsolute("~/Scripts/SyntaxHighlighter/scripts/shBrushSql.js"));

            ScriptManager.RegisterStartupScript(this, this.GetType(), "all", "SyntaxHighlighter.all();", true);
        }

        void edit_Click(object sender, EventArgs e)
        {
            Util.QueryEditorUtil.SetQueryInSession(this.Page, Text, null, true);
            Page.Response.Redirect(Jhu.Graywulf.Web.UI.Apps.Query.Default.GetUrl(), false);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            writer.Write(String.Format("<script type=\"syntaxhighlighter\" class=\"brush: {0};\"><![CDATA[\r\n", "sql"));
            writer.Write(Text);
            writer.Write("]]></script>");

            edit.RenderControl(writer);
        }
    }
}
