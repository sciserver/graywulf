using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public partial class ExportTablesToUriForm : UserControl, IExportTablesForm
    {
        public static string GetUrl()
        {
            return "~/Jobs/ExportTables/ExportTablesToUriForm.ascx";
        }

        public Uri Uri
        {
            get { return new Uri(uri.Text, UriKind.RelativeOrAbsolute); }
            set { uri.Text = value.OriginalString; }
        }

        public Credentials Credentials
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(username.Text) ||
                    !String.IsNullOrWhiteSpace(password.Text))
                {
                    return new Credentials()
                    {
                        UserName = username.Text,
                        Password = password.Text,
                    };
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null)
                {
                    username.Text = value.UserName;
                    password.Text = value.Password;
                }
                else
                {
                    username.Text = String.Empty;
                    password.Text = String.Empty;
                }
            }
        }
    }
}