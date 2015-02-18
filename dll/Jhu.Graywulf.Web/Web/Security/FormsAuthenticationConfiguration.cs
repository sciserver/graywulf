using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Security
{
    public class FormsAuthenticationConfiguration : ConfigurationSection
    {
        #region Static declarations

        private static ConfigurationPropertyCollection properties;

        private static readonly ConfigurationProperty propBaseUri = new ConfigurationProperty(
            "baseUri", typeof(Uri), new Uri("/auth/", UriKind.Relative), ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propSignInUri = new ConfigurationProperty(
            "signInUri", typeof(Uri), new Uri("signIn.aspx?ReturnUrl=[$ReturnUrl]", UriKind.Relative), ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propSignOutUri = new ConfigurationProperty(
            "signOutUri", typeof(Uri), new Uri("signOut.aspx?ReturnUrl=[$ReturnUrl]", UriKind.Relative), ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propRegisterUri = new ConfigurationProperty(
            "registerUri", typeof(Uri), new Uri("user.aspx?ReturnUrl=[$ReturnUrl]", UriKind.Relative), ConfigurationPropertyOptions.None);

        private static readonly ConfigurationProperty propAccountUri = new ConfigurationProperty(
            "accountUri", typeof(Uri), new Uri("user.aspx?ReturnUrl=[$ReturnUrl]", UriKind.Relative), ConfigurationPropertyOptions.None);

        static FormsAuthenticationConfiguration()
        {
            properties = new ConfigurationPropertyCollection();

            properties.Add(propBaseUri);
            properties.Add(propSignInUri);
            properties.Add(propSignOutUri);
            properties.Add(propRegisterUri);
            properties.Add(propAccountUri);
        }

        #endregion
        #region Properties

        [ConfigurationProperty("baseUri")]
        public Uri BaseUri
        {
            get { return (Uri)base[propBaseUri]; }
            set { base[propBaseUri] = value; }
        }

        [ConfigurationProperty("signInUri")]
        public Uri SignInUri
        {
            get { return (Uri)base[propSignInUri]; }
            set { base[propSignInUri] = value; }
        }

        [ConfigurationProperty("signOutUri")]
        public Uri SignOutUri
        {
            get { return (Uri)base[propSignOutUri]; }
            set { base[propSignOutUri] = value; }
        }

        [ConfigurationProperty("registerUri")]
        public Uri RegisterUri
        {
            get { return (Uri)base[propRegisterUri]; }
            set { base[propRegisterUri] = value; }
        }

        [ConfigurationProperty("accountUri")]
        public Uri AccountUri
        {
            get { return (Uri)base[propAccountUri]; }
            set { base[propAccountUri] = value; }
        }

        #endregion
    }
}
