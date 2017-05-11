using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.CodeMirror.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class CodeMirror : WebControl, INamingContainer, IScriptControl, ITextControl
    {
        private static readonly Dictionary<string, string> Modes = new Dictionary<string, string>()
        {
            {"text/x-sql", "sql"},
            {"text/x-mysql", "mysql"},
            {"text/x-plsql", "plsql"},
            {"text/xml", "xml"},
        };

        private TextBox text;
        private HiddenField selectedText;
        private HiddenField selectionCoords;

        public override string CssClass
        {
            get { return text.CssClass; }
            set { text.CssClass = value; }
        }

        public string Text
        {
            get { return text.Text; }
            set { text.Text = value; }
        }

        public string SelectedText
        {
            get { return selectedText.Value; }
            set { selectedText.Value = value; }
        }

        [TypeConverter(typeof(Int32ArrayConverter))]
        public int[] SelectionCoords
        {
            get { return (int[])new Int32ArrayConverter().ConvertFromString(selectionCoords.Value); }
            set { selectionCoords.Value = new Int32ArrayConverter().ConvertToString(value); }
        }

        public int IndentUnit
        {
            get { return (int)(ViewState["IndentUnit"] ?? 4); }
            set { ViewState["IndentUnit"] = value; }
        }

        public bool LineNumbers
        {
            get { return (bool)(ViewState["LineNumbers"] ?? true); }
            set { ViewState["LineNumbers"] = value; }
        }

        public bool MatchBrackets
        {
            get { return (bool)(ViewState["MatchBrackets"] ?? true); }
            set { ViewState["MatchBrackets"] = value; }
        }

        public string Mode
        {
            get { return (string)(ViewState["Mode"] ?? ""); }
            set { ViewState["Mode"] = value; }
        }

        public string Theme
        {
            get { return (string)(ViewState["Theme"] ?? ""); }
            set { ViewState["Theme"] = value; }
        }

        public CodeMirror()
        {
            this.text = new TextBox();
            this.text.ID = "text";
            this.text.TextMode = TextBoxMode.MultiLine;

            this.selectedText = new HiddenField();
            this.selectedText.ID = "selectedText";

            this.selectionCoords = new HiddenField();
            this.selectionCoords.ID = "selectionCoords";

            this.Controls.Add(text);
            this.Controls.Add(selectedText);
            this.Controls.Add(selectionCoords);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var link = new HtmlLink();
            link.Href = "~/Scripts/CodeMirror/lib/codemirror.css";
            link.Attributes["rel"] = "stylesheet";
            Page.Header.Controls.AddAt(0, link);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "",
                    String.Format(
@"
$find('{0}${1}').onBeforeSubmit.call($find('{0}${1}'));
",
                        this.ClientID, this.GetType().Name));

                var scriptManager = ScriptManager.GetCurrent(this.Page);
                if (scriptManager != null)
                {
                    Scripts.ScriptLibrary.Register(scriptManager, new Scripts.JQuery());
                    scriptManager.RegisterScriptControl(this);
                }
                else
                {
                    throw new ApplicationException("You must have a ScriptManager on the Page.");
                }
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }

            writer.AddAttribute("id", ClientID);
            writer.AddAttribute("class", CssClass);
            writer.RenderBeginTag("div");

            text.RenderControl(writer);
            selectedText.RenderControl(writer);
            selectionCoords.RenderControl(writer);

            writer.RenderEndTag();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("Graywulf.CodeMirror", this.ClientID);
            descriptor.AddProperty("IndentUnit", this.IndentUnit);
            descriptor.AddProperty("LineNumbers", this.LineNumbers);
            descriptor.AddProperty("MatchBrackets", this.MatchBrackets);
            descriptor.AddProperty("Mode", this.Mode);
            descriptor.AddProperty("Theme", this.Theme);
            descriptor.AddProperty("Width", this.Width.ToString());
            descriptor.AddProperty("Height", this.Height.ToString());

            yield return descriptor;
        }

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.CodeMirror.js", "Jhu.Graywulf.Web.Controls");
            yield return new ScriptReference("~/Scripts/CodeMirror/lib/codemirror.js");

            if (!String.IsNullOrWhiteSpace(Mode))
            {
                yield return new ScriptReference(String.Format("~/Scripts/CodeMirror/mode/{0}/{0}.js", Modes[Mode]));
            }
        }
    }
}