using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
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

        public Uri CustomizableUri
        {
            get { return Uri; }
            set { Uri = value; }
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

        protected void Page_Init(object sender, EventArgs e)
        {
            uriFormatValidator.ValidationExpression = Jhu.Graywulf.IO.Constants.UrlPattern;
        }

        public void GenerateDefaultUri(string filename)
        {
            uri.Text = "ftp://myftp.edu/upload/" + filename;
        }
    }
}