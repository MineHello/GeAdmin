
using Ge.Model;
using Ge.Model.Dto;
using Ge.Model.Models;

namespace Ge.ServiceCore.Services
{
    /// <summary>
    /// 邮件发送记录service接口
    /// </summary>
    public interface IEmailLogService : IBaseService<EmailLog>
    {
        PagedInfo<EmailLogDto> GetList(EmailLogQueryDto parm);

        EmailLog GetInfo(long Id);


        int UpdateEmailLog(EmailLog parm);
    }
}
