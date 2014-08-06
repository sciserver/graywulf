using System;
using System.IO;
using System.Text;
using System.Web.Mail;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.UI
{
    public partial class Feedback : PageBase
    {
        public static string GetUrl()
        {
            return "~/Feedback.aspx";
        }

        public static string GetSpaceRequestUrl()
        {
            return String.Format("~/Feedback.aspx?mode={0}", Mode.SpaceRequest);
        }

        public static string GetErrorReportUrl()
        {
            return String.Format("~/Feedback.aspx?mode={0}", Mode.Error);
        }

        public static string GetJobErrorUrl(Guid jobGuid)
        {
            return String.Format("~/Feedback.aspx?mode={0}&guid={1}", Mode.JobError, jobGuid);
        }

        private enum Mode
        {
            Unknown,
            Error,
            JobError,
            SpaceRequest
        }

        private Guid jobGuid;
        private Mode mode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["mode"] != null)
            {
                mode = (Mode)Enum.Parse(typeof(Mode), (string)Request.QueryString["mode"]);
            }
            else
            {
                mode = Mode.Unknown;
            }

            if (Request.QueryString["guid"] != null)
            {
                jobGuid = new Guid(Request.QueryString["guid"]);
            }
            else
            {
                jobGuid = Guid.Empty;
            }

            if (!IsPostBack)
            {
                if (User != null && User.Identity != null && User.Identity is GraywulfIdentity)
                {
                    var user = ((GraywulfIdentity)User.Identity).User;

                    // Load user if available
                    if (user != null)
                    {
                        Name.Text = String.Format("{0} {1}", user.FirstName, user.LastName);
                        NameRow.Visible = false;

                        Email.Text = user.Email;
                        EmailRow.Visible = false;
                    }
                }

                switch (mode)
                {
                    case Mode.SpaceRequest:
                        SpecifySpace.Visible = true;

                        Subject.Items.Add(new ListItem("Request more MyDB space", "SpaceRequest"));
                        Subject.SelectedValue = "SpaceRequest";
                        break;
                    case Mode.Error:
                    case Mode.JobError:
                        ErrorsIncluded.Visible = true;
                        Subject.SelectedValue = "Error";
                        break;
                    default:
                        break;
                }
            }
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            string templatefile;
            Exception ex = null;
            long eventid = 0;

            if (mode == Mode.Error && Session[Constants.SessionException] != null)
            {
                ex = (Exception)Session[Constants.SessionException];
                eventid = (long)Session[Constants.SessionExceptionEventID];

                templatefile = "~/Templates/ErrorFeedbackEmail.xml";
            }
            else if (mode == Mode.JobError)
            {


                templatefile = "~/Templates/ErrorFeedbackEmail.xml";
            }
            else
            {
                templatefile = "~/Templates/FeedbackEmail.xml";
            }

            var template = File.ReadAllText(MapPath(templatefile));
            string subject;
            string body;

            Util.EmailTemplateUtility.LoadEmailTemplate(template, out subject, out body);

            Util.EmailTemplateUtility.ReplaceEmailToken(ref subject, "[$ShortTitle]", RegistryContext.Federation.ShortTitle);
            Util.EmailTemplateUtility.ReplaceEmailToken(ref subject, "[$Subject]", Subject.Text);

            Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$ShortTitle]", RegistryContext.Federation.ShortTitle);
            Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$Name]", Name.Text);
            Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$Email]", Email.Text);
            Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$Subject]", Subject.Text);
            Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$Comments]", Comments.Text);

            if (mode == Mode.Error && Session[Constants.SessionException] != null)
            {
                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$ErrorMessage]", ex.Message);
                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$JobGUID]", "-");
                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$EventID]", eventid.ToString());
            }
            else if (mode == Mode.JobError)
            {
                var job = new JobInstance(RegistryContext);
                job.Guid = jobGuid;
                job.Load();

                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$ErrorMessage]", job.ExceptionMessage);
                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$JobGUID]", job.Guid.ToString());
                Util.EmailTemplateUtility.ReplaceEmailToken(ref body, "[$EventID]", "-");
            }

            var msg = Util.EmailTemplateUtility.CreateMessage(
                RegistryContext.Federation.Email, RegistryContext.Federation.ShortTitle,
                RegistryContext.Federation.Email, RegistryContext.Federation.ShortTitle,
                subject, body);

            // Append stack trace
            if (mode == Mode.Error && Session[Constants.SessionException] != null)
            {
                System.Net.Mail.Attachment att;

                using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(new HttpUnhandledException(ex.Message, ex).GetHtmlErrorMessage())))
                {
                    att = new System.Net.Mail.Attachment(mem, "error.html");

                    msg.Attachments.Add(att);
                    Util.EmailTemplateUtility.SendMessage(msg);
                }
            }
            else
            {
                Util.EmailTemplateUtility.SendMessage(msg);
            }

            FeedbackForm.Visible = false;
            SuccessForm.Visible = true;
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }

        protected void Back_Click(object sender, EventArgs e)
        {
            if (mode == Mode.Error)
            {
                Response.Redirect("~/");
            }
            else
            {
                Response.Redirect(OriginalReferer);
            }
        }
    }
}