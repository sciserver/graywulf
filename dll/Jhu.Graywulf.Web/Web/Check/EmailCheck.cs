using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Check;

namespace Jhu.Graywulf.Web.Check
{
    public class EmailCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Email; }
        }

        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }

        public EmailCheck(string fromName, string fromEmail, string toEmail)
        {
            this.FromName = fromName;
            this.FromEmail = fromEmail;
            this.ToEmail = toEmail;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            var smtpclient = new SmtpClient();

            yield return ReportInfo("Sending e-mail message to {0}", ToEmail);
            yield return ReportInfo("Delivery method: {0}", smtpclient.DeliveryMethod);
            yield return ReportInfo("Server: {0}:{1}", smtpclient.Host, smtpclient.Port);

            var subject = String.Format("{0} test message from {1}", FromName, Environment.MachineName);
            var body = "Test message, please ignore.";

            var msg = Util.EmailTemplateUtility.CreateMessage(
                FromEmail, FromName,
                ToEmail, ToEmail,
                subject, body);

            Util.EmailTemplateUtility.SendMessage(msg);

            yield return ReportSuccess("E-mail message sent.");
        }
    }
}
