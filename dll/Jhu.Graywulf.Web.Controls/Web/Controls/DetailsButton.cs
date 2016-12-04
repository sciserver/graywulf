using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.DetailsButton.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class DetailsButton : UserControl, IScriptControl
    {
        private const string ClosedCssClass = "glyphicon-chevron-down";
        private const string OpenCssClass = "glyphicon-chevron-up";
        private Label label;

        public string CssClass
        {
            get { return (string)(ViewState["CssClass"] ?? "gw-details-button glyphicon"); }
            set { ViewState["CssClass"] = value; }
        }

        public string Style
        {
            get { return label.Style.Value; }
            set { label.Style.Value = value; }
        }

        public DetailsButton()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.label = new Label();
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
            label.CssClass = CssClass + ' ' + ClosedCssClass;
            label.RenderControl(writer);
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
