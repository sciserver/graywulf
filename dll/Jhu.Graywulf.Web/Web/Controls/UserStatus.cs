using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Web.Security;

namespace Jhu.Graywulf.Web.Controls
{
    [Themeable(true)]
    public class UserStatus : UserControlBase
    {
        private Image icon;
        private Label usernameLabel;
        private Label username;
        private HyperLink account;
        private HyperLink signOut;
        private HyperLink signIn;
        private HyperLink register;

        [Bindable(false), Themeable(true), UrlProperty]
        public string ImageUrl
        {
            get { return icon.ImageUrl; }
            set { icon.ImageUrl = value; }
        }

        [Bindable(true), Themeable(true)]
        public string UsernameLabelText
        {
            get { return usernameLabel.Text; }
            set { usernameLabel.Text = value; }
        }

        [Bindable(false), Themeable(true)]
        public string CssClass
        {
            get { return (string)ViewState["CssClass"] ?? ""; }
            set { ViewState["CssClass"] = value; }
        }

        public UserStatus()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.icon = new Image();

            this.usernameLabel = new Label();
            this.usernameLabel.Text = Labels.Username;

            this.username = new Label();

            this.account = new HyperLink();
            this.account.Text = Labels.Account;

            this.signOut = new HyperLink();
            this.signOut.Text = Labels.SignOut;

            this.signIn = new HyperLink();
            this.signIn.Text = Labels.SignIn;

            this.register = new HyperLink();
            this.register.Text = Labels.Register;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Controls.Add(usernameLabel);
            Controls.Add(username);
            Controls.Add(account);
            Controls.Add(signOut);
            Controls.Add(signIn);
            Controls.Add(register);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            username.Text = Page.User.Identity.Name;

            icon.CssClass = CssClass;
            usernameLabel.CssClass = CssClass;
            username.CssClass = CssClass;
            account.CssClass = CssClass;
            signOut.CssClass = CssClass;
            signIn.CssClass = CssClass;

            var returl = Server.UrlEncode(Request.RawUrl);

            var wam = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];

            account.NavigateUrl = wam.GetUserAccountUrl();
            signOut.NavigateUrl = wam.GetSignOutUrl();
            signIn.NavigateUrl = wam.GetSignInUrl();
            register.NavigateUrl = wam.GetUserAccountUrl();
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (DesignMode)
            {
                writer.Write(Labels.UserStatusDesigner);
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(icon.ImageUrl))
                {
                    icon.RenderControl(writer);
                }

                if (Page.Request.IsAuthenticated)
                {
                    usernameLabel.RenderControl(writer);
                    writer.Write(": ");

                    username.Text = Page.User.Identity.Name;
                    username.RenderControl(writer);
                    
                    writer.Write(" | ");
                    account.RenderControl(writer);
                    writer.Write(" | ");
                    signOut.RenderControl(writer);
                }
                else
                {
                    signIn.RenderControl(writer);
                    writer.Write(" | ");
                    register.RenderControl(writer);
                }
            }
        }
    }
}
