using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    public partial class CancelJob : PageBase
    {
        public static string GetUrl(Guid guid)
        {
            return GetUrl(new Guid[] { guid });
        }

        public static string GetUrl(Guid[] guids)
        {
            string gs = Web.Util.UrlFormatter.ArrayToUrlList(guids);
            return String.Format("~/Jobs/CancelJob.aspx?guid={0}", gs);
        }

        private Guid[] guids;
        private List<JobInstance> jobs;

        private void LoadJobs()
        {
            jobs = new List<JobInstance>();

            foreach (var guid in guids)
            {
                var job = new JobInstance(RegistryContext);
                job.Guid = guid;
                job.Load();

                if (IsAuthenticatedUser(job.UserGuidOwner))
                {
                    throw new System.Security.SecurityException("Access denied.");  // TODO
                }

                if (job.CanCancel)
                {
                    jobs.Add(job);
                }
            }
        }

        private void UpdateForm()
        {
            JobList.Items.Clear();

            foreach (var job in jobs)
            {
                JobList.Items.Add(new ListItem(job.Name));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            guids = Request.QueryString["guid"].Split(',').Select(g => Guid.Parse(g)).ToArray();

            if (!IsPostBack)
            {
                LoadJobs();

                if (jobs.Count == 0)
                {
                    Response.Redirect(OriginalReferer);
                }

                UpdateForm();
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                LoadJobs();

                foreach (var job in jobs)
                {
                    if (job.CanCancel)
                    {
                        job.Cancel();
                    }
                }

                Response.Redirect(Jobs.Default.GetUrl());
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }
    }
}