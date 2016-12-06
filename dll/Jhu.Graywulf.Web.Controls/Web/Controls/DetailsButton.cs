using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.DetailsButton.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class DetailsButton : UserControl, IScriptControl
    {
        private const string ClosedCssClass = "glyphicon glyphicon-chevron-down";
        private const string OpenCssClass = "glyphicon glyphicon-chevron-up";
        private Label label;
        private Label glyph;

        public string Text
        {
            get { return (string)ViewState["Text"]; }
            set { ViewState["Text"] = value; }
        }

        public string CssClass
        {
            get { return (string)(ViewState["CssClass"] ?? "gw-details-button"); }
            set { ViewState["CssClass"] = value; }
        }

        public string Style
        {
            get { return glyph.Style.Value; }
            set { glyph.Style.Value = value; }
        }

        public DetailsButton()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.label = new Label();
            this.glyph = new Label();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                var scriptManager = ScriptManager.GetCurrent(this.Page);

                if (scriptManager != null)
                {
                    Scripts.ScriptLibrary.RegisterReferences(scriptManager, new Scripts.JQuery());
                    scriptManager.RegisterScriptControl(this);
                }
                else
                {
                    throw new InvalidOperationException("You must have a ScriptManager on the Page.");
                }
            }

            base.OnPreRender(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Text != null)
            {
                label.CssClass = CssClass;
                label.Text = Text;
                label.RenderControl(writer);
            }

            glyph.CssClass = CssClass + ' ' + ClosedCssClass;
            glyph.RenderControl(writer);
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            yield break;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.DetailsButton.js", "Jhu.Graywulf.Web.Controls");
        }
    }
}
