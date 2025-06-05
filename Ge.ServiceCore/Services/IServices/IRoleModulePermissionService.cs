using Ge.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore.Services.IServices
{
    public interface IRoleModulePermissionService : IBaseService<RoleModulePermission>
    {
        Task<List<RoleModulePermission>> GetRoleModule();
    }
}
