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
using System.IO;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Federation
{
    public partial class DeploymentPackageForm : EntityFormPageBase<DeploymentPackage>
    {
        protected override void OnUpdateForm()
        {
            base.OnUpdateForm();
        }

        protected override void OnSaveForm()
        {
            base.OnSaveForm();

            if (Data.PostedFile != null)
            {
                item.Filename = System.IO.Path.GetFileName(Data.PostedFile.FileName);
            }
        }

        protected override void OnSaveFormCompleted(bool newentity)
        {
            if (Data.PostedFile != null)
            {
                byte[] buffer = new byte[Data.PostedFile.InputStream.Length];
                Data.PostedFile.InputStream.Read(buffer, 0, buffer.Length);
                item.SetData(buffer);
            }
        }
    }
}