public class EmailClient
{
    public static async Task SendMail(string toEmail, string fromEmail, string subject, string body,
        string attachmentFilePathCsv = "", bool isBodyHtml = true)
    {
        SmtpClient client = new SmtpClient(AppSettings.GetValue(AppSettingsKeys.EMAIL_HOST),
            Convert.ToInt32(AppSettings.GetValue(AppSettingsKeys.EMAIL_PORT)))
        {
            Credentials = new NetworkCredential(AppSettings.GetValue(AppSettingsKeys.EMAIL_USERNAME),
                AppSettings.GetValue(AppSettingsKeys.EMAIL_PASSWORD)),
            EnableSsl = true
        };
        MailAddress from = new MailAddress(fromEmail);
        MailAddress to = new MailAddress(toEmail);
        MailMessage message = new MailMessage(from, to)
        {
            Body = body,
            Subject = subject,
            IsBodyHtml = isBodyHtml
        };

        if (!string.IsNullOrEmpty(attachmentFilePathCsv))
        {
            var attachments = attachmentFilePathCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            attachments.ToList().ForEach(attachment => AddAttachment(message, attachment));
        }

        await client.SendMailAsync(message);
        
        message.Dispose();
    }

    private static void AddAttachment(MailMessage message, string attachmentFilePath)
    {
        Attachment data = new Attachment(attachmentFilePath, MediaTypeNames.Application.Octet);
        ContentDisposition disposition = data.ContentDisposition;
        disposition.CreationDate = System.IO.File.GetCreationTime(attachmentFilePath);
        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentFilePath);
        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentFilePath);
        message.Attachments.Add(data);
    }
}