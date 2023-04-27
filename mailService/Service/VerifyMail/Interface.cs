using mailService.Model.dto;

namespace mailService.Service.VerifyMail
{
    public interface IMailVerifyService
    {
        bool SendVerifyMsg(AuthVerifyDto verifyInfo);
    }
}
