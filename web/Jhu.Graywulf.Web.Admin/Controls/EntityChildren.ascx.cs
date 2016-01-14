using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    [ParseChildren(true, DefaultProperty="Lists")]
    public partial class EntityChildren : System.Web.UI.UserControl
    {
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<EntityList> Lists
        {
            get { return new List<EntityList>(); }
            set
            {
                // This is a hack here but should work
                foreach (var l in value)
                {
                    var v = new TabView();
                    if (String.IsNullOrWhiteSpace(l.Text))
                    {
                        v.Text = Jhu.Graywulf.Registry.Constants.EntityNames_Plural[l.ChildrenType];
                    }
                    else
                    {
                        v.Text = l.Text;
                    }
                    v.Controls.Add(l);
                    MultiViewTabs.Views.Add(v);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}