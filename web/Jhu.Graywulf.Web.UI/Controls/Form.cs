using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Controls
{
    [ParseChildren(true), Themeable(true)]
    public class Form : WebControl
    {
        private Label formTitle;
        private Image formIcon;

        private ITemplate formTemplate;
        private Control formContainer;
        private ITemplate buttonsTemplate;
        private Control buttonsContainer;

        public bool IsModal
        {
            get { return (bool)(ViewState["IsModal"] ?? false); }
            set { ViewState["IsModal"] = value; }
        }

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
            var table = new Table();
            var titlerow = new TableRow();
            var titlecell = new TableCell();
            var formrow = new TableRow();
            var iconcell = new TableCell();
            var formcell = new TableCell();
            var buttonrow = new TableRow();
            var buttoncell = new TableCell();

            this.formContainer = new Control();
            this.buttonsContainer = new Control();

            this.Controls.Add(table);
            table.Rows.AddRange(new[] { titlerow, formrow, buttonrow });
            titlerow.Cells.AddRange(new[] { titlecell });
            formrow.Cells.AddRange(new[] { iconcell, formcell });
            buttonrow.Cells.AddRange(new[] { buttoncell });

            titlecell.Controls.Add(formTitle);
            iconcell.Controls.Add(formIcon);
            formcell.Controls.Add(formContainer);

            titlecell.CssClass = "gw-form-title";
            titlecell.ColumnSpan = 2;
            iconcell.CssClass = "gw-form-icon";
            formcell.CssClass = "gw-form";

            buttoncell.CssClass = "gw-form-buttons";
            buttoncell.ColumnSpan = 2;

            buttoncell.Controls.Add(buttonsContainer);
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
            var css = "";

            if (IsModal)
            {
                css += "modal ";
                writer.AddAttribute("role", "dialog");
            }

            css += "dock-center dock-scroll gw-form";

            if (!String.IsNullOrEmpty(CssClass))
            {
                css += " " + CssClass;
            }

            writer.AddAttribute("class", css);
            writer.AddAttribute("id", this.ClientID);

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.RenderContents(writer);

            writer.RenderEndTag();
        }
    }
}
