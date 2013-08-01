using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Jhu.Graywulf.Registry;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.MultiSelectListView.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    /// <summary>
    /// Summary description for MultiselectListView
    /// </summary>
    public class MultiSelectListView : ListView, IScriptControl
    {
        protected const string DefaultSelectionCheckboxID = "selectionCheckbox";
        protected const string DefaultSelectionElementID = "selectionElement";
        protected const string ViewStateSelectedDataKeys = "SelectedDataKeys";

        private HashSet<string> selectedDataKeys;

        public string SelectionCheckboxID
        {
            get { return (string)(ViewState["SelectionCheckboxID"] ?? DefaultSelectionCheckboxID); }
            set { ViewState["SelectionCheckboxID"] = value; }
        }

        public ListSelectionMode SelectionMode
        {
            get { return (ListSelectionMode)(ViewState["SelectionMode"] ?? ListSelectionMode.Multiple); }
            set { ViewState["SelectionMode"] = value; }
        }

        public string SelectionElementID
        {
            get { return (string)(ViewState["SelectionElementID"] ?? DefaultSelectionElementID); }
            set { ViewState["SelectionElementID"] = value; }
        }

        [Themeable(true)]
        public string CssClassSelected
        {
            get { return (string)(ViewState["CssClassSelected"]); }
            set { ViewState["CssClassSelected"] = value; }
        }

        public HashSet<string> SelectedDataKeys
        {
            get { return selectedDataKeys; }
        }

        // --


        // --

        private string GetKey(DataKey key)
        {
            string res = "";

            for (int i = 0; i < key.Values.Count; i++)
            {
                if (i > 0)
                {
                    res += "|";
                }

                res += key.Values[i].ToString();
            }

            return res;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                selectedDataKeys = new HashSet<string>();
            }
            else
            {
                switch (SelectionMode)
                {
                    case ListSelectionMode.Single:
                        selectedDataKeys = new HashSet<string>();
                        break;
                    case ListSelectionMode.Multiple:
                        selectedDataKeys = (HashSet<string>)ViewState[ViewStateSelectedDataKeys];
                        break;
                }

                // Save selection
                foreach (ListViewItem item in Items)
                {
                    var key = GetKey(DataKeys[item.DisplayIndex]);
                    var cb = (CheckBox)item.FindControl(SelectionCheckboxID);

                    if (cb != null)
                    {
                        if (cb.Checked && !selectedDataKeys.Contains(key))
                        {
                            selectedDataKeys.Add(key);

                            if (SelectionMode == ListSelectionMode.Single)
                            {
                                break;
                            }
                        }

                        if (!cb.Checked && selectedDataKeys.Contains(key))
                        {
                            selectedDataKeys.Remove(key);
                        }
                    }
                }
            }

            ViewState[ViewStateSelectedDataKeys] = selectedDataKeys;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!this.DesignMode)
            {
                var scriptManager = ScriptManager.GetCurrent(this.Page);
                if (scriptManager != null)
                {
                    scriptManager.RegisterScriptControl(this);
                    Util.JQuery.Register(scriptManager);
                }
                else
                {
                    throw new ApplicationException("You must have a ScriptManager on the Page.");
                }
            }

            base.OnPreRender(e);

            foreach (ListViewItem item in Items)
            {
                var key = GetKey(DataKeys[item.DisplayIndex]);
                var selected = selectedDataKeys.Contains(key);

                var cb = item.FindControl(SelectionCheckboxID) as CheckBox;

                if (cb != null)
                {
                    cb.Checked = selected;
                }

                var se = item.FindControl(SelectionElementID) as HtmlControl;
                
                if (se != null && selected)
                {
                    se.Attributes["class"] = CssClassSelected;
                }
            }

            base.OnPreRender(e);
        }

        //--

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            var descriptor = new ScriptControlDescriptor("Graywulf.MultiSelectListView", this.UniqueID);
            descriptor.AddProperty("SelectionMode", this.SelectionMode);
            descriptor.AddProperty("SelectionCheckboxID", this.SelectionCheckboxID);
            descriptor.AddProperty("SelectionElementID", this.SelectionElementID);
            descriptor.AddProperty("CssClassSelected", this.CssClassSelected);

            yield return descriptor;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("Jhu.Graywulf.Web.Controls.MultiSelectListView.js", "Jhu.Graywulf.Web");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }

            writer.AddAttribute("id", this.UniqueID);
            writer.RenderBeginTag("div");

            base.Render(writer);

            writer.RenderEndTag();
        }
    }
}