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

namespace Jhu.Graywulf.Web.UI.Controls
{
    [Themeable(true)]
    public class UserStatus : UserControlBase
    {
        private Image icon;
        private Label usernameLabel;
        private Label username;
        private LinkButton account;
        private LinkButton signOut;
        private LinkButton signIn;
        private LinkButton register;

        private WebAuthenticationModule authenticationModule;

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

            this.account = new LinkButton();
            this.account.CausesValidation = false;
            this.account.Click += Account_Click;

            this.signOut = new LinkButton();
            this.signOut.CausesValidation = false;
            this.signOut.Click += SignOut_Click;

            this.signIn = new LinkButton();
            this.signIn.CausesValidation = false;
            this.signIn.Click += SignIn_Click;

            this.register = new LinkButton();
            this.register.CausesValidation = false;
            this.register.Click += Register_Click;
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.account.Text = String.Format(Labels.Account, Page.Application[Web.UI.Constants.ApplicationDomainName]);
            this.signOut.Text = String.Format(Labels.SignOut, Page.Application[Web.UI.Constants.ApplicationDomainName]);
            this.signIn.Text = String.Format(Labels.SignIn, Page.Application[Web.UI.Constants.ApplicationDomainName]);
            this.register.Text = String.Format(Labels.Register, Page.Application[Web.UI.Constants.ApplicationDomainName]);


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

        #region Event handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            Controls.Add(usernameLabel);
            Controls.Add(username);
            Controls.Add(account);
            Controls.Add(signOut);
            Controls.Add(signIn);
            Controls.Add(register);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            username.Text = Page.User.Identity.Name;

            icon.CssClass = CssClass;
            usernameLabel.CssClass = CssClass;
            username.CssClass = CssClass;
            account.CssClass = CssClass;
            signOut.CssClass = CssClass;
            signIn.CssClass = CssClass;

            authenticationModule = (WebAuthenticationModule)HttpContext.Current.ApplicationInstance.Modules["WebAuthenticationModule"];
        }

        void Account_Click(object sender, EventArgs e)
        {
            var url = authenticationModule.GetUserAccountUrl();
            Response.Redirect(url, false);
        }

        void Register_Click(object sender, EventArgs e)
        {
            var url = authenticationModule.GetRegisternUrl();
            Response.Redirect(url, false);
        }

        void SignIn_Click(object sender, EventArgs e)
        {
            var url = authenticationModule.GetSignInUrl();
            Response.Redirect(url, false);
        }

        void SignOut_Click(object sender, EventArgs e)
        {
            // In case the sing-out page cannot remove cookies
            // we can do it here

            var url = authenticationModule.GetSignOutUrl();

            authenticationModule.DeleteAuthResponseHeaders();
            Session.Abandon();
            Response.Redirect(url, false);
        }

        #endregion
    }
}
