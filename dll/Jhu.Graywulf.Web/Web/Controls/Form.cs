using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    [ParseChildren(true), Themeable(true)]
    public class Form : WebControl
    {
        private Panel formDiv;
        private Panel buttonsDiv;
        private Label formTitle;
        private Image formIcon;

        private ITemplate formTemplate;
        private Control formContainer;
        private ITemplate buttonsTemplate;
        private Control buttonsContainer;

        [Themeable(true)]
        public string Text
        {
            get { return formTitle.Text; }
            set { formTitle.Text = value; }
        }

        [Themeable(true), UrlProperty]
        public string ImageUrl
        {
            get { return formIcon.ImageUrl; }
            set
            {
                formIcon.ImageUrl = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateInstance(TemplateInstance.Single)]
        public ITemplate FormTemplate
        {
            get { return formTemplate; }
            set { formTemplate = value; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateInstance(TemplateInstance.Single)]
        public ITemplate ButtonsTemplate
        {
            get { return buttonsTemplate; }
            set { buttonsTemplate = value; }
        }

        public Form()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.formTitle = new Label();
            this.formIcon = new Image();

            this.formTemplate = null;
            this.buttonsTemplate = null;
        }

        private void CreateLayout()
        {
            formDiv = new Panel();
            var table = new Table();
            var titlerow = new TableRow();
            var titlecell = new TableCell();
            var formrow = new TableRow();
            var iconcell = new TableCell();
            var formcell = new TableCell();

            this.formContainer = new Control();
            this.buttonsContainer = new Control();

            this.Controls.Add(formDiv);
            formDiv.Controls.Add(table);
            table.Rows.AddRange(new[] { titlerow, formrow });
            titlerow.Cells.AddRange(new[] { new TableCell(), titlecell });
            formrow.Cells.AddRange(new[] { iconcell, formcell });

            titlecell.Controls.Add(formTitle);
            iconcell.Controls.Add(formIcon);
            formcell.Controls.Add(formContainer);

            formDiv.CssClass = "FormFrame dock-top";
            table.CssClass = "Form";
            titlecell.CssClass = "FormHeadline";
            iconcell.CssClass = "FormIcon";
            formcell.CssClass = "Form";

            // ---

            buttonsDiv = new Panel();

            buttonsDiv.Controls.Add(new LiteralControl("<p class=\"FormMessage\"></p>"));

            buttonsDiv.Controls.Add(new LiteralControl("<p class=\"FormButtons\">"));
            buttonsDiv.Controls.Add(buttonsContainer);
            buttonsDiv.Controls.Add(new LiteralControl("</p>"));

            buttonsDiv.CssClass = "dock-bottom";

            // ---
            Controls.Add(formDiv);
            Controls.Add(buttonsDiv);
        }

        private void ClearContent()
        {
            formContainer.Controls.Clear();
            buttonsContainer.Controls.Clear();
        }

        private void CreateContents()
        {
            if (base.DesignMode)
            {
                ClearContent();
            }

            if (FormTemplate != null)
            {
                FormTemplate.InstantiateIn(formContainer);
            }

            if (ButtonsTemplate != null)
            {
                ButtonsTemplate.InstantiateIn(buttonsContainer);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            CreateLayout();

            base.OnInit(e);

            CreateContents();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            formDiv.RenderControl(writer);
            buttonsDiv.RenderControl(writer);
        }
    }
}
