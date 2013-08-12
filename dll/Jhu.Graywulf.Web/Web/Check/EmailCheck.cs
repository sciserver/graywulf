using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using Jhu.Graywulf.Web;
using Jhu.Graywulf.Web.Util;

namespace Jhu.Graywulf.Web.Check
{
    public class EmailCheck : CheckRoutineBase
    {
        public string EmailAddress { get; set; }

        public EmailCheck(string emailAddress)
        {
            this.EmailAddress = emailAddress;
        }

        public override void Execute(PageBase page)
        {
            var smtpclient = new SmtpClient();

            page.Response.Output.WriteLine(
                "Sending e-mail message to {0}",
                EmailAddress);

            page.Response.Output.WriteLine("Delivery method: {0}", smtpclient.DeliveryMethod);
            page.Response.Output.WriteLine("Server: {0}:{1}", smtpclient.Host, smtpclient.Port);

            var subject = String.Format("{0} test message from {1}", page.Domain.ShortTitle, Environment.MachineName);
            var body = "Test message, please ignore.";

            var msg = EmailTemplateUtility.CreateMessage(
                page.Domain.Email, page.Domain.ShortTitle,
                EmailAddress, EmailAddress,
                subject, body);

            EmailTemplateUtility.SendMessage(msg);

            page.Response.Output.WriteLine("E-mail message sent.");
        }
    }
}
