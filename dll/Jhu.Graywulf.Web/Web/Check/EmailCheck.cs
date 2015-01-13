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
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }

        public EmailCheck(string fromName, string fromEmail, string toEmail)
        {
            this.FromName = fromName;
            this.FromEmail = fromEmail;
            this.ToEmail = toEmail;
        }

        public override void Execute(TextWriter output)
        {
            var smtpclient = new SmtpClient();

            output.WriteLine(
                "Sending e-mail message to {0}",
                ToEmail);

            output.WriteLine("Delivery method: {0}", smtpclient.DeliveryMethod);
            output.WriteLine("Server: {0}:{1}", smtpclient.Host, smtpclient.Port);

            var subject = String.Format("{0} test message from {1}", FromName, Environment.MachineName);
            var body = "Test message, please ignore.";

            var msg = Util.EmailTemplateUtility.CreateMessage(
                FromEmail, FromName,
                ToEmail, ToEmail,
                subject, body);

            Util.EmailTemplateUtility.SendMessage(msg);

            output.WriteLine("E-mail message sent.");
        }
    }
}
