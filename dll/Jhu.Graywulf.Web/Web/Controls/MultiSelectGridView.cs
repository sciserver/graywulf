using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("Jhu.Graywulf.Web.Controls.MultiSelectGridView.js", "text/javascript", PerformSubstitution = true)]

namespace Jhu.Graywulf.Web.Controls
{
    public class MultiSelectGridView : GridView, IScriptControl
    {
        protected const string ViewStateSelectedDataKeys = "SelectedDataKeys";

        private HashSet<string> selectedDataKeys;

        public ListSelectionMode SelectionMode
        {
            get { return (ListSelectionMode)(ViewState["SelectionMode"] ?? ListSelectionMode.Multiple); }
            set { ViewState["SelectionMode"] = value; }
        }

        public HashSet<string> SelectedDataKeys
        {
            get { return selectedDataKeys; }
        }

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

        protected override System.Collections.ICollection CreateColumns(PagedDataSource dataSource, bool useDataSource)
        {
            var columns = base.CreateColumns(dataSource, useDataSource);

            // Look for duplicate selection fields
            var found = false;

            foreach (DataControlField col in columns)
            {
                if (col is SelectionField)
                {
                    if (found)
                    {
                        throw new InvalidOperationException("MultiSelectGridView can contain one selection column only.");
                    }
                    found = true;
                }
            }

            return columns;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.PageIndexChanging += new GridViewPageEventHandler(MultiSelectGridView_PageIndexChanging);

            if (!Page.IsPostBack)
            {
                selectedDataKeys = new HashSet<string>();
            }
            else     // TODO: only when visible
            {
                switch (SelectionMode)
                {
                    case ListSelectionMode.Single:
                        selectedDataKeys = new HashSet<string>();
                        break;
                    case ListSelectionMode.Multiple:
                        selectedDataKeys = (HashSet<string>)(ViewState[ViewStateSelectedDataKeys] ?? new HashSet<string>());
                        break;
                }

                // Save selection
                foreach (GridViewRow row in Rows)
                {
                    var key = GetKey(DataKeys[row.RowIndex]);
                    var cb = (CheckBox)row.FindControl(SelectionField.DefaultSelectionCheckBoxID);

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

        void MultiSelectGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.PageIndex = e.NewPageIndex;
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
                    throw new InvalidOperationException("You must have a ScriptManager on the Page.");
                }
            }

            if (DataKeyNames == null)
            {
                throw new InvalidOperationException("DataKeyNames must be set");
            }

            base.OnPreRender(e);

            // TODO: only when selectedDataKeys != null 
            foreach (GridViewRow row in Rows)
            {
                var key = GetKey(DataKeys[row.RowIndex]);
                var selected = selectedDataKeys.Contains(key);

                var cb = row.FindControl(SelectionField.DefaultSelectionCheckBoxID) as CheckBox;

                if (cb != null)
                {
                    cb.Checked = selected;
                }
            }

            base.OnPreRender(e);
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (Rows.Count > 0)
            {
                var descriptor = new ScriptControlDescriptor("Graywulf.MultiSelectGridView", this.ClientID);
                descriptor.AddProperty("SelectionMode", this.SelectionMode);
                descriptor.AddProperty("SelectionCheckboxID", SelectionField.DefaultSelectionCheckBoxID);
                descriptor.AddProperty("CssClass", this.CssClass);
                descriptor.AddProperty("SelectedRowCssClass", this.SelectedRowStyle.CssClass);

                yield return descriptor;
            }
            else
            {
                yield break;
            }
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (Rows.Count > 0)
            {
                yield return new ScriptReference("Jhu.Graywulf.Web.Controls.MultiSelectGridView.js", "Jhu.Graywulf.Web");
            }
            else
            {
                yield break;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this.DesignMode)
            {
                ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);
            }

            base.Render(writer);
        }
    }
}