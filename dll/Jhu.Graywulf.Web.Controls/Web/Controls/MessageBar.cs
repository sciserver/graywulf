using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.MessageBar.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class MessageBar : UserControl, IScriptControl
    {
        private Panel panel;
        private Label button;
        private Label label;

        public string Text
        {
            get { return label.Text; }
            set { label.Text = value; }
        }

        public bool CloseButton
        {
            get { return button.Visible; }
            set { button.Visible = value; }
        }

        public Color ForeColor
        {
            get { return label.ForeColor; }
            set { label.ForeColor = value; }
        }

        public Color BackColor
        {
            get { return panel.BackColor; }
            set { panel.BackColor = value; }
        }

        public MessageBar()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.panel = new Panel();
            this.label = new Label();
            this.button = new Label();

            panel.CssClass = "gw-messagebar";
            button.CssClass = "gw-messagebar-button";
            button.Text = "\u00D7";

            panel.Controls.Add(button);
            panel.Controls.Add(label);
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

        protected override void Render(HtmlTextWriter writer)
        {
            panel.RenderControl(writer);
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            yield break;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.MessageBar.js", "Jhu.Graywulf.Web.Controls");
        }
    }
}
