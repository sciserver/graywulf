using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class FederationApplicationBase : DomainApplicationBase
    {
        protected override void OnApplicationStart()
        {
            base.OnApplicationStart();

            // Load settings from federation
            using (var context = CreateRegistryContext())
            {
                var federation = context.Federation;

                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationShortTitle] = federation.ShortTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationLongTitle] = federation.LongTitle;
                Application[Jhu.Graywulf.Web.UI.Constants.ApplicationCopyright] = federation.Copyright;
            }
        }

        protected override void RegisterButtons()
        {
            base.RegisterButtons();

            var buttons = ConfigurationManager.GetSection("jhu.graywulf/webui/dropdownButtons") as NameValueCollection;

            if (buttons != null)
            {
                foreach (string key in buttons.Keys)
                {
                    var parts = buttons[key].Split('|');

                    RegisterDropdownButton(new MenuButton()
                    {
                        Text = parts[0],
                        NavigateUrl = parts[1]
                    });
                }
            }
        }
    }
}
