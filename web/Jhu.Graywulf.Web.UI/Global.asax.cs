using System;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI
{
    public class Global : FederationApplicationBase
    {
        protected override void OnRegisterApps()
        {
            base.OnRegisterApps();

            RegisterApp(typeof(Apps.Common.App));
            RegisterApp(typeof(Apps.Schema.App));
            RegisterApp(typeof(Apps.Query.App));
            RegisterApp(typeof(Apps.Jobs.App));
            RegisterApp(typeof(Apps.MyDB.App));
            RegisterApp(typeof(Apps.Api.App));
            RegisterApp(typeof(Apps.Docs.App));
        }

        protected override void OnRegisterServices()
        {
            base.OnRegisterServices();

            RegisterService(typeof(IAuthService));
            RegisterService(typeof(IManageService));
            RegisterService(typeof(ISchemaService));
            RegisterService(typeof(IJobsService));
            RegisterService(typeof(IDataService));
            RegisterService(typeof(ITestService));
        }

        protected override void OnRegisterButtons()
        {
            base.OnRegisterButtons();

            RegisterFooterButton(new Graywulf.Web.UI.MenuButton()
            {
                Text = "status",
                NavigateUrl = Jhu.Graywulf.Web.UI.Apps.Common.Status.GetUrl()
            });
        }
    }
}