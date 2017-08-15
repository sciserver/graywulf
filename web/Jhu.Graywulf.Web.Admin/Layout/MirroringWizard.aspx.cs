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
            var ef = new EntityFactory(RegistryContext);
            item = ef.LoadEntity<DatabaseDefinition>(new Guid(Request.QueryString["guid"]));

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
            var ef = new EntityFactory(RegistryContext);
            var jf = MirrorDatabaseJobFactory.Create(RegistryContext);
            var dis = new List<DatabaseInstance>();

            foreach (var guid in SourceDatabases.SelectedDataKeys)
            {
                var di = ef.LoadEntity<DatabaseInstance>(new Guid(guid));
                dis.Add(di);
            }

            var par = jf.CreateParameters(dis);

            if (par.SourceDatabaseInstanceGuids.Length == 0 ||
                par.DestinationDatabaseInstanceGuids.Length == 0)
            {
                throw new Exception("No source or destination databases found");    // *** TODO
            }

            par.CascadedCopy = CascadedCopy.Checked;
            par.SkipExistingFiles = SkipExistingFiles.Checked;
            par.AttachAsReadOnly = AttachAsReadOnly.Checked;
            par.RunCheckDb = RunCheckDb.Checked;

            var q = ef.LoadEntity<QueueInstance>(new Guid(QueueInstance.SelectedValue));
            var job = jf.ScheduleAsJob(par, q.GetFullyQualifiedName(), TimeSpan.Zero, "");
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

            var mr = ((DatabaseDefinition)Item).Federation.ControllerMachineRole;
            mr.LoadQueueInstances(false);

            foreach (var q in mr.QueueInstances.Values)
            {
                QueueInstance.Items.Add(new ListItem(mr.Name + "\\" + q.Name, q.Guid.ToString()));
            }
        }
    }
}