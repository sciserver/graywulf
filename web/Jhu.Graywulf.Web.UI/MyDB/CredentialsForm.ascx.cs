using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class CredentialsForm : CustomUserControlBase
    {
        public Credentials GetCredentials()
        {
            if (this.Visible && 
                (!String.IsNullOrWhiteSpace(username.Text) || !String.IsNullOrWhiteSpace(password.Text)))
            {

                return new Credentials()
                {
                    Username = username.Text,
                    Password = password.Text,
                };
            }
            else
            {
                return null;
            }
        }
    }
}