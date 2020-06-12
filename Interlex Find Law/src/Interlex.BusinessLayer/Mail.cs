using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Interlex.BusinessLayer
{
    public class Mail
    {
        public static void SendMail(string fromEMail, string fromName, string[] toEMail, string subject, string body)
        {
            // TODO REMOVE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //fromEMail = "support@apis.bg";

            SmtpClient client = new SmtpClient();
            
            MailAddress from = new MailAddress(fromEMail, fromName, System.Text.Encoding.UTF8);

            foreach (string to_email in toEMail)
            {
                MailAddress to = new MailAddress(to_email);
                MailMessage message = new MailMessage(from, to);
                message.IsBodyHtml = true;
                message.Body = body;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.Subject = subject;
                message.SubjectEncoding = Encoding.GetEncoding(1251);// System.Text.Encoding.UTF8;

                client.Send(message);

                message.Dispose();
            }
        }
    }
}
