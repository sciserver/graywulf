using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Util
{
    public static class EmailSender
    {

        public static void Send(User user, string bodyTemplate, string baseUrl)
        {
            var d = user.Domain;
            string subject;
            string body;
            EmailTemplateUtility.LoadEmailTemplate(bodyTemplate, out subject, out body);

            // Send out email
            EmailTemplateUtility.ReplaceEmailToken(ref subject, "[$BaseUrl]", baseUrl);
            EmailTemplateUtility.ReplaceEmailTokens(ref subject, user);

            EmailTemplateUtility.ReplaceEmailToken(ref body, "[$BaseUrl]", baseUrl);
            EmailTemplateUtility.ReplaceEmailTokens(ref body, user);

            EmailTemplateUtility.SendMessage(
                d.Email,
                d.ShortTitle,
                user.Email,
                String.Format("{0} {1}", user.FirstName, user.LastName),
                subject,
                body);
        }
    }
}
