using MailKit.Net.Smtp;
using mailService.Controllers;
using mailService.Model.dto;
using MimeKit;
using System.Collections.Specialized;

namespace mailService.Service.VerifyMail
{
    public class MailVerifyService : IMailVerifyService
    {
        private readonly ILogger<MailVerifyService> _logger;
        private readonly IConfiguration _conf;

        public MailVerifyService(ILogger<MailVerifyService> logger, IConfiguration conf)
        {
            _logger = logger;
            _conf = conf;
        }

        public bool SendVerifyMsg(AuthVerifyDto verifyInfo)
        {
            var email = new MimeMessage();

            string author = _conf.GetValue<string>("Mail:MainMail");
            string connection = _conf.GetValue<string>("Mail:Connection");
            string password = _conf.GetValue<string>("Mail:Password");
            int port = _conf.GetValue<int>("Mail:Port");

            email.From.Add(MailboxAddress.Parse(author));
            email.To.Add(MailboxAddress.Parse(verifyInfo.Email));
            email.Subject = "verify";


            using var smtp = new SmtpClient();
            smtp.Connect(connection, port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(author, password);


            var pathToTemplate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailTemplate", "mailVerification.html");
            string htmlBody = "";
            using (StreamReader streamReader = System.IO.File.OpenText(pathToTemplate))
            {
                htmlBody = streamReader.ReadToEnd();
            }
            ListDictionary replacements = new ListDictionary();
            var body = htmlBody.Replace("{CODE}", verifyInfo.Code);
            //{CODE}
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            try
            {
                smtp.Send(email);
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                smtp.Disconnect(true);
            }
            return true;
        }
    }
}
