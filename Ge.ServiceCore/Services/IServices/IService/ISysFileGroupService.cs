using Ge.Model;
using Ge.Model.System.Model;
using Ge.Model.System.Model.Dto;

namespace Ge.ServiceCore.Services
{
    /// <summary>
    /// 文件分组service接口
    /// </summary>
    public interface ISysFileGroupService : IBaseService<SysFileGroup>
    {
        PagedInfo<SysFileGroupDto> GetList(SysFileGroupQueryDto parm);

        SysFileGroup GetInfo(int GroupId);

        List<SysFileGroup> GetTreeList(SysFileGroupQueryDto parm);

        SysFileGroup AddSysFileGroup(SysFileGroup parm);
        int UpdateSysFileGroup(SysFileGroup parm);
    }
}
