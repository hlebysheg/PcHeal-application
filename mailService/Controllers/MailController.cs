using MailKit.Net.Smtp;
using mailService.Model.dto;
using mailService.Service.VerifyMail;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System.Collections.Specialized;

namespace mailService.Controllers
{
    [ApiController]
    [Route("api/mail/verify")]
    public class MailController : ControllerBase
    {

        private readonly ILogger<MailController> _logger;
        private readonly IMailVerifyService _mailService;

        public MailController(ILogger<MailController> logger, IConfiguration conf, IMailVerifyService mailService)
        {
            _logger = logger;
            _mailService = mailService;
        }

        [HttpPost]
        public IActionResult SendEmail(AuthVerifyDto verifyInfo)
        {

            if (_mailService.SendVerifyMsg(verifyInfo))
            {
                return Ok("mail send");
            }
            return BadRequest("Send mail error");
        }
    }
}