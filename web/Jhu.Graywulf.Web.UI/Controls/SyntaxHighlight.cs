using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    [DefaultProperty("Text"), ParseChildren(false), PersistChildren(true), ToolboxData("<{0}:CodeView runat=server></{0}:CodeView>"), ControlValueProperty("Text")]
    public class SyntaxHighlight : System.Web.UI.WebControls.WebControl, ITextControl
    {
        private bool textSetByAddParsedSubObject;

        [Bindable(true), Localizable(true), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public string Text
        {
            get { return (string)ViewState["Text"] ?? ""; }
            set { ViewState["Text"] = value; }
        }

        public string Brush
        {
            get { return (string)ViewState["Brush"] ?? ""; }
            set { ViewState["Brush"] = value; }
        }

        public SyntaxHighlight()
        {
            base.Load += Page_Load;
            base.PreRender += Page_PreRender;
        }

        protected virtual void Page_Load(object sender, EventArgs e)
        {

        }

        protected virtual void Page_PreRender(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "core", VirtualPathUtility.ToAbsolute("~/Scripts/SyntaxHighlighter/scripts/shCore.js"));
            ScriptManager.RegisterClientScriptInclude(this, this.GetType(), "autoloader", VirtualPathUtility.ToAbsolute("~/Scripts/SyntaxHighlighter/scripts/shAutoLoader.js"));

            ScriptManager.RegisterClientScriptInclude(this, this.GetType(),
                String.Format("brush{0}", Brush.ToLowerInvariant()),
                VirtualPathUtility.ToAbsolute(
                    String.Format("~/Scripts/SyntaxHighlighter/scripts/shBrush{0}.js", Brush)));

            ScriptManager.RegisterStartupScript(this, this.GetType(), "all", "SyntaxHighlighter.all();", true);
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

        public override void RenderControl(HtmlTextWriter writer)
        {
            var text = new StringBuilder(Text);
            string url = Page.Request.Url.Authority + VirtualPathUtility.ToAbsolute("~");

            text.Replace("[$ApplicationUrl]", url);

            writer.Write(String.Format("<script type=\"syntaxhighlighter\" class=\"brush: {0};\"><![CDATA[\r\n", Brush.ToLowerInvariant()));
            writer.Write(text.ToString());
            writer.Write("]]></script>");

            base.RenderControl(writer);
        }
    }
}