using Ge.Model;
using Ge.ServiceCore.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ge.ServiceCore.Services
{
    public class RoleModulePermissionService:BaseService<RoleModulePermission>,IRoleModulePermissionService
    {
        private readonly IModulesService moduleRepository;
        private readonly IRoleService roleRepository;


        // 将多个仓储接口注入
        public RoleModulePermissionService(
            IModulesService moduleRepository, 
            IRoleService roleRepository)
        {
            this.moduleRepository = moduleRepository;
            this.roleRepository = roleRepository;
        }

        /// <summary>
        /// 获取全部 角色接口(按钮)关系数据 注意我使用咱们之前的AOP缓存，很好的应用上了
        /// </summary>
        /// <returns></returns>
        public async Task<List<RoleModulePermission>> GetRoleModule()
        {
            var roleModulePermissions = await base.GetListAsync(a => a.IsDeleted == false);
            if (roleModulePermissions.Count > 0)
            {
                foreach (var item in roleModulePermissions)
                {
                    item.Role = await roleRepository.GetByIdAsync(item.RoleId);
                    item.Module = await moduleRepository.GetByIdAsync(item.ModuleId);
                }

            }
            return roleModulePermissions;
        }
    }
}
