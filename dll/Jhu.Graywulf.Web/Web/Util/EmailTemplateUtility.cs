using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net.Mail;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Util
{
    public static class EmailTemplateUtility
    {
        // TODO: change function name to more meaningful
        public static void LoadEmailTemplate(string message, out string subject, out string body)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(message);

            subject = xml.SelectNodes("//messageTemplate/subject")[0].InnerText;
            body = xml.SelectNodes("//messageTemplate/body")[0].InnerText;
        }

        public static void ReplaceEmailTokens(ref string template, Entity entity)
        {
            // TODO: this fails if there are still tokens in the template that refer
            // to invalid properties of the entity
            template = Jhu.Graywulf.Registry.Util.ResolveExpression(entity, template);
        }

        public static void ReplaceEmailToken(ref string template, string token, string value)
        {
            template = template.Replace(token, value);
        }

        public static MailMessage CreateMessage(string fromEmail, string fromName, string toEmail, string toName, string subject, string body)
        {
            var msg = new MailMessage();
            msg.From = new MailAddress(fromEmail, fromName);
            msg.To.Add(new MailAddress(toEmail, toName));
            msg.Subject = subject;
            msg.Body = body;
            msg.BodyEncoding = global::System.Text.Encoding.UTF8;
            msg.HeadersEncoding = global::System.Text.Encoding.UTF8;
            msg.SubjectEncoding = global::System.Text.Encoding.UTF8;

            return msg;
        }

        public static void SendMessage(string fromEmail, string fromName, string toEmail, string toName, string subject, string body)
        {
            SendMessage(CreateMessage(fromEmail, fromName, toEmail, toName, subject, body));
        }

        public static void SendMessage(MailMessage msg)
        {
            var smtpclient = new SmtpClient();
            smtpclient.Send(msg);
        }
    }
}
