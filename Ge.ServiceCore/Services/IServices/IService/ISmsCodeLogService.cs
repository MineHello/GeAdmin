using Ge.Model;
using Ge.Model.Dto;
using Ge.Model.Models;

namespace Ge.ServiceCore.Services
{
    /// <summary>
    /// 短信验证码记录service接口
    /// </summary>
    public interface ISmsCodeLogService : IBaseService<SmsCodeLog>
    {
        PagedInfo<SmsCodeLogDto> GetList(SmscodeLogQueryDto parm);

        SmsCodeLog GetInfo(long Id);

        SmsCodeLog AddSmscodeLog(SmsCodeLog parm);

    }
}
