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
using Jhu.Graywulf.Jobs.MirrorDatabase;

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
            RefreshQueueInstanceList();

            Name.Text = item.Name;
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            var jf = MirrorDatabaseJobFactory.Create(RegistryContext);
            var dis = new List<DatabaseInstance>();

            foreach (var guid in SourceDatabases.SelectedDataKeys)
            {
                var di = new DatabaseInstance(RegistryContext);
                di.Guid = new Guid(guid);
                di.Load();
                dis.Add(di);
            }

            var par = jf.CreateParameters(dis);
            par.CascadedCopy = CascadedCopy.Checked;
            par.SkipExistingFiles = SkipExistingFiles.Checked;

            var q = new QueueInstance(RegistryContext);
            q.Guid = new Guid(QueueInstance.SelectedValue);
            q.Load();

            var job = jf.ScheduleAsJob(par, q.GetFullyQualifiedName(), "");
            job.Save();

            Response.Redirect(OriginalReferer);
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

        protected void RefreshQueueInstanceList()
        {
            QueueInstance.Items.Add(new ListItem("(not set)", Guid.Empty.ToString()));

            var m = ((DatabaseDefinition)Item).Federation.ControllerMachine;
            m.LoadQueueInstances(false);

            foreach (var q in m.QueueInstances.Values)
            {
                QueueInstance.Items.Add(new ListItem(m.Name + "\\" + q.Name, q.Guid.ToString()));
            }
        }
    }
}