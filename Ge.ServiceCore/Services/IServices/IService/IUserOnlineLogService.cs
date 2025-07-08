using Ge.Model;
using Ge.Model.Dto;
using Ge.Model.Models;


namespace Ge.ServiceCore.Monitor.IMonitorService
{
    /// <summary>
    /// 用户在线时长service接口
    /// </summary>
    public interface IUserOnlineLogService : IBaseService<UserOnlineLog>
    {
        PagedInfo<UserOnlineLogDto> GetList(UserOnlineLogQueryDto parm);


        PagedInfo<UserOnlineLogDto> ExportList(UserOnlineLogQueryDto parm);
    }
}
