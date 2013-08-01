using System;

namespace Jhu.Graywulf.Web.Auth.Controls
{
    public partial class Menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Home.NavigateUrl = "/";
            Back.NavigateUrl = ((PageBase)Page).ReturnUrl;
        }
    }
}