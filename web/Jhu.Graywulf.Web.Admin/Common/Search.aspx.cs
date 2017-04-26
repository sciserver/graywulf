using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Common
{
    public partial class Search : PageBase
    {

        public static string GetUrl()
        {
            return "~/Common/Search.aspx";
        }

        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshTypeList();
            }
            else
            {
                SearchResultList.Search = GetSearchObject();
            }
        }

        private void RefreshTypeList()
        {
            var types = Constants.EntityTypeMap
                .Select(i => i.Key)
                .OrderBy(i => Constants.EntityNames_Singular[i])
                .ToArray();

            type.Items.Clear();

            type.Items.Add(new ListItem("(any)", ""));

            foreach (var t in types)
            {
                var li = new ListItem(Constants.EntityNames_Singular[t], t.ToString());
                type.Items.Add(li);
            }
        }

        private EntitySearch GetSearchObject()
        {
            var s = new EntitySearch(RegistryContext);
            s.Name = name.Text;

            if (!String.IsNullOrWhiteSpace(type.SelectedValue))
            {
                s.EntityType = (EntityType)Enum.Parse(typeof(EntityType), type.SelectedValue);
            }

            return s;
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            SearchResultList.Sort("name", SortDirection.Ascending);
            SearchResultList.Visible = true;
        }
    }
}