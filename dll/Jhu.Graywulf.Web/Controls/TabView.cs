using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class TabView : View
    {
        [Bindable(true), DefaultValue(""), Localizable(true)]
        public string Text
        {
            get { return (string)ViewState["Text"] ?? String.Empty; }
            set { ViewState["Text"] = value; }
        }

        [Bindable(true), DefaultValue(""), Localizable(true)]
        public bool Enabled
        {
            get { return (bool)(ViewState["Enabled"] ?? false); }
            set { ViewState["Enabled"] = value; }
        }

        [Bindable(true), DefaultValue(""), Localizable(true)]
        public bool Hidden
        {
            get { return (bool)(ViewState["Hidden"] ?? false); }
            set { ViewState["Hidden"] = value; }
        }

        public TabView()
        {
        }
    }
}
