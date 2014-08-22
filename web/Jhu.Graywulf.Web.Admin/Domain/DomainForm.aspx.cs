using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Web.Admin.Domain
{
    public partial class DomainForm : EntityFormPageBase<Registry.Domain>
    {
        protected Registry.Cluster cluster;

        protected override void OnItemLoaded(bool newentity)
        {
            base.OnItemLoaded(newentity);

            if (!IsPostBack && newentity)
            {
                var di = new Jhu.Graywulf.Install.DomainInstaller(Item);
                di.GenerateDefaultSettings();
            }
        }

        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();

            IdentityProvider.Text = Item.IdentityProvider;
            AuthenticatorFactory.Text = Item.AuthenticatorFactory;
            ShortTitle.Text = Item.ShortTitle;
            LongTitle.Text = Item.LongTitle;
            Email.Text = Item.Email;
            Copyright.Text = Item.Copyright;
            Disclaimer.Text = Item.Disclaimer;
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            Item.IdentityProvider = IdentityProvider.Text;
            Item.AuthenticatorFactory = AuthenticatorFactory.Text;
            Item.ShortTitle = ShortTitle.Text;
            Item.LongTitle = LongTitle.Text;
            Item.Email = Email.Text;
            Item.Copyright = Copyright.Text;
            Item.Disclaimer = Disclaimer.Text;
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (newentity)
            {
                var i = new DomainInstaller(Item);
                i.GenerateDefaultChildren();
            }
        }
    }
}