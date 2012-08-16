using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

namespace TrainingRequisition.ClassLibrary.Utilities
{
    public class UtilityEmail
    {

        public static void Send(string emailAddress,
           string ccEmailAddress,
            string subject,
            string emailTemplateFile,
            Dictionary<string, string> replacements)
        {
            // open the file
            string content = "";
            using (System.IO.TextReader reader = System.IO.File.OpenText(emailTemplateFile))
            {
                content = reader.ReadToEnd();
                foreach (string key in replacements.Keys)
                    content = content.Replace("#" + key + "#", replacements[key]);
            }

            Send(emailAddress, ccEmailAddress, subject, content);
        }

        public static void Send(string emailAddress, string ccEmailAddress,
            string subject, string content)
        {
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();
            MailMessage mail = new MailMessage();
            if (!string.IsNullOrEmpty(ccEmailAddress))
                mail.CC.Add(ccEmailAddress);
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            mail.From = new MailAddress(adminEmail);
            mail.Subject = subject;
            mail.Body = content;
            mail.To.Add(new MailAddress(emailAddress));
            mail.IsBodyHtml = true;
            try
            {
                SmtpClient mailClient = new SmtpClient();
                mailClient.Send(mail);
                //mailClient.Send(adminEmail, emailAddress, subject, content);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }
    }
}
