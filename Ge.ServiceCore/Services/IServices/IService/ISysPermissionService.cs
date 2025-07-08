using Ge.Model.System;
using Ge.Model.System.Dto;

namespace Ge.ServiceCore.Services
{
    public interface ISysPermissionService
    {
        public List<string> GetRolePermission(SysUserDto user);
        public List<string> GetMenuPermission(SysUserDto user);
    }
}
