using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class CommentsForm : System.Web.UI.UserControl
    {
        public string Comments
        {
            get 
            {
                if (Visible)
                {
                    return comments.Text;
                }
                else
                {
                    return String.Empty;
                }
            }
            set { comments.Text = value; }
        }
    }
}