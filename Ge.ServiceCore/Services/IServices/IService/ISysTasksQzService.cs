using Ge.Model;
using Ge.Model.System;
using Ge.Model.System.Dto;

namespace Ge.ServiceCore.Services
{
    public interface ISysTasksQzService : IBaseService<SysTasks>
    {
        PagedInfo<SysTasks> SelectTaskList(TasksQueryDto parm);
        //SysTasksQz GetId(object id);
        int AddTasks(SysTasks parm);
        int UpdateTasks(SysTasks parm);
    }
}
