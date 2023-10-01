#nullable enable

using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

using SqlSensei.Core.Logging.Email;

namespace SqlSensei.Core.EmailLogger
{
    public class GmailEmailService : IEmailService
    {
        private readonly string username;
        private readonly string password;
        private readonly string emailFrom;
        private readonly string emailTo;

        public GmailEmailService(string username, string password, string emailFrom, string emailTo)
        {
            this.username = username;
            this.password = password;
            this.emailFrom = emailFrom;
            this.emailTo = emailTo;
        }

        public async Task SendEmailAsync(string subject, string content, string? fileName, byte[]? attachmentBytes, string? attachmentContentType)
        {
            using var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var from = new MailAddress(emailFrom, "SqlSensei Notification");
            var to = new MailAddress(emailTo);
            var msg = new MailMessage(from, to)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            };

            if (attachmentBytes != null && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(attachmentContentType))
            {
                var attachment = new Attachment(new MemoryStream(attachmentBytes), new ContentType(attachmentContentType))
                {
                    Name = fileName
                };

                msg.Attachments.Add(attachment);
            }

            await client.SendMailAsync(msg);
        }
    }
}
