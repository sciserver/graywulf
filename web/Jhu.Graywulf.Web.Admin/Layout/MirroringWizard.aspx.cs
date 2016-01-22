using System;
using System.Collections.Generic;
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

namespace Jhu.Graywulf.Web.Admin.Layout
{
    public partial class MirroringWizard : PageBase, IEntityForm
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Layout/MirroringWizard.aspx?guid={0}", guid);
        }

        protected DatabaseDefinition item;

        public Entity Item
        {
            get { return item; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            item = new DatabaseDefinition(RegistryContext);
            item.Guid = new Guid(Request.QueryString["guid"]);
            item.Load();

            if (!IsPostBack)
            {
                UpdateForm();
            }

            SourceDatabases.ParentEntity = item;
        }

        protected void UpdateForm()
        {
            Name.Text = item.Name;
            Cascade.Checked = true;
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            /*double sizefactor = double.Parse(SizeFactor.Text);
            string postfix = String.Empty;

            DatabaseVersion databaseVersion = new DatabaseVersion(RegistryContext);
            databaseVersion.Guid = new Guid(databaseVersionList.SelectedValue);
            databaseVersion.Load();

            int q = 0;
            for (int sli = 0; sli < slices.Count; sli++)
            {
                for (int ssi = 0; ssi < serverInstances.Count; ssi++)
                {
                    CheckBox cb = (CheckBox)FindControlRecursive(string.Format("cb_{0}_{1}", sli, ssi));
                    if (cb.Checked)
                    {
                        switch (item.LayoutType)
                        {
                            case DatabaseLayoutType.Sliced:
                                break;
                            default:
                                postfix = String.Format("{0}", q.ToString("00"));
                                break;
                        }

                        var dii = new DatabaseInstanceInstaller(item);

                        dii.GenerateDatabaseInstance(
                            serverInstances[ssi], 
                            slices[sli], 
                            databaseVersion, 
                            NamePattern.Text.Replace("[$Number]", postfix), 
                            DatabaseNamePattern.Text.Replace("[$Number]", postfix), 
                            sizefactor, 
                            GenerateFileGroups.Checked);

                        q++;
                    }
                }
            }

            Response.Redirect(item.GetDetailsUrl(EntityGroup.Layout), false);*/
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(item.GetDetailsUrl(EntityGroup.Layout), false);
        }


        #region IEntityForm Members


        public void OnButtonCommand(object sender, CommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}