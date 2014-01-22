using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    public class OpenIDAuthenticator : InteractiveAuthenticatorBase
    {
        private string discoveryUrl;

        public override string Protocol
        {
            get { return Constants.ProtocolNameOpenID; }
        }

        public string DiscoveryUrl
        {
            get { return discoveryUrl; }
            set { discoveryUrl = value; }
        }

        public OpenIDAuthenticator()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.discoveryUrl = null;
        }

        public override GraywulfPrincipal Authenticate()
        {
            // Get OpenID provider's response from the http context
            using (var openid = new OpenIdRelyingParty())
            {
                var response = openid.GetResponse();
                
                // TODO: figure out which OpenID provider sent the response
                // and associate with the right authenticator

                if (response != null)
                {
                    switch (response.Status)
                    {
                        case AuthenticationStatus.Authenticated:
                            return CreatePrincipal(response);
                        case AuthenticationStatus.Canceled:
                        case AuthenticationStatus.Failed:
                            throw new System.Security.Authentication.AuthenticationException("OpenID authentication failed.", response.Exception);
                        case AuthenticationStatus.ExtensionsOnly:
                        case AuthenticationStatus.SetupRequired:
                            throw new InvalidOperationException();
                        default:
                            throw new NotImplementedException();
                    }
                }

                return null;
            }
        }

        public override void RedirectToLoginPage()
        {
            using (var openid = new OpenIdRelyingParty())
            {
                // TODO: do this only once when OpenID is initialized
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Ssl3;

                IAuthenticationRequest request = openid.CreateRequest(discoveryUrl);
                request.AddExtension(CreateFetchRequest());
                request.RedirectToProvider();
            }
        }

        private FetchRequest CreateFetchRequest()
        {
            var fetch = new FetchRequest();

            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Contact.Email, true));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.First, false));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.Last, false));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.Middle, false));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.Prefix, false));
            fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Name.FullName, false));

            // TODO: add more

            return fetch;
        }

        private GraywulfPrincipal CreatePrincipal(IAuthenticationResponse response)
        {
            var identity = new GraywulfIdentity()
            {
                Protocol = this.Protocol,
                Authority = response.Provider.Uri.ToString(),
                Identifier = response.ClaimedIdentifier,
                IsAuthenticated = false,
                User = new User()
            };

            var fetch = response.GetExtension<FetchResponse>();
            
            identity.User.Email = fetch.Attributes[WellKnownAttributes.Contact.Email].Values[0];
            identity.User.FirstName = fetch.Attributes[WellKnownAttributes.Name.First].Values[0];
            identity.User.LastName = fetch.Attributes[WellKnownAttributes.Name.Last].Values[0];

            // TODO: add more

            return new GraywulfPrincipal(identity);
        }
    }
}
