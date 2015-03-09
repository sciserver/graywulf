using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class CommentsForm : System.Web.UI.UserControl
    {
        public string Comments
        {
            get { return comments.Text; }
            set { comments.Text = value; }
        }
    }
}