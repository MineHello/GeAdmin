using Ge.Infrastructure.AppExtensions;
using Ge.Infrastructure.Extensions;
using Ge.Model;
using Ge.Model.System;
using Ge.Model.System.Dto;
using Ge.Repository;
using Microsoft.AspNetCore.Server.IIS;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ge.Infrastructure.Constant;

namespace Ge.ServiceCore.Services.IServices
{
    internal class SysUserService : BaseService<SysUser>, ISysUserService
    {
        public int ChangePhoneNum(long userid, string phoneNum)
        {
            throw new NotImplementedException();
        }

        public int ChangeUser(SysUser user)
        {
            throw new NotImplementedException();
        }

        public int ChangeUserStatus(SysUser user)
        {
            throw new NotImplementedException();
        }

        public List<long> CheckPhoneBind(string phoneNum)
        {
            throw new NotImplementedException();
        }

        public void CheckUserAllowed(SysUser user)
        {
            throw new NotImplementedException();
        }

        public void CheckUserDataScope(long userid, long loginUserId)
        {
            throw new NotImplementedException();
        }

        public string CheckUserNameUnique(string userName)
        {
            throw new NotImplementedException();
        }

        public int DeleteUser(long userid)
        {
            throw new NotImplementedException();
        }

        public (string, object, object) ImportUsers(List<SysUser> users)
        {
            throw new NotImplementedException();
        }

        public SysUser InsertUser(SysUser sysUser)
        {
            throw new NotImplementedException();
        }

        public SysUser Login(LoginBodyDto user)
        {
            throw new NotImplementedException();
        }

        public SysUser Register(RegisterDto dto)
        {
            throw new NotImplementedException();
        }

        public int ResetPwd(long userid, string password)
        {
            throw new NotImplementedException();
        }

        public SysUserDto SelectUserById(long userId)
        {
            throw new NotImplementedException();
        }

        public PagedInfo<SysUserDto> SelectUserList(SysUserQueryDto user, PagerInfo pager)
        {
            var exp = Expressionable.Create<SysUser>();
            exp.AndIF(!string.IsNullOrEmpty(user.UserName), u => u.UserName.Contains(user.UserName));
            exp.AndIF(user.UserId > 0, u => u.UserId == user.UserId);
            exp.AndIF(user.Status != -1, u => u.Status == user.Status);
            exp.AndIF(user.BeginTime != DateTime.MinValue && user.BeginTime != null, u => u.Create_time >= user.BeginTime);
            exp.AndIF(user.EndTime != DateTime.MinValue && user.EndTime != null, u => u.Create_time <= user.EndTime);
            exp.AndIF(!user.Phonenumber.IsEmpty(), u => u.Phonenumber == user.Phonenumber);
            exp.And(u => u.DelFlag == 0);

            if (user.DeptId != 0)
            {
                var allChildDepts = Context.Queryable<SysDept>().ToChildList(it => it.ParentId, user.DeptId);

                exp.And(u => allChildDepts.Select(f => f.DeptId).ToList().Contains(u.DeptId));
            }
            var query = Queryable()
                .LeftJoin<SysDept>((u, dept) => u.DeptId == dept.DeptId)
                .Where(exp.ToExpression())
                .Select((u, dept) => new SysUserDto
                {
                    UserId = u.UserId.SelectAll(),
                    DeptName = dept.DeptName,
                });
            var list = query.ToPage(pager);
            //foreach (var item in list.Result)
            //{
            //    if (!HttpContextExtension.HasSensitivePerm(App.HttpContext, SensitivePerms.ViewRealPhone))
            //    {
            //        item.Phonenumber = MaskUtil.MaskPhone(item.Phonenumber);
            //    }
            //    if (!HttpContextExtension.HasSensitivePerm(App.HttpContext, SensitivePerms.ViewEmail))
            //    {
            //        item.Email = MaskUtil.MaskPhone(item.Email);
            //    }
            //}

            return list;
        }

        public void SelectUserList()
        {
            throw new NotImplementedException();
        }

        public void UpdateLoginInfo(string userIP, long userId)
        {
            throw new NotImplementedException();
        }

        public int UpdatePhoto(SysUser user)
        {
            throw new NotImplementedException();
        }

        public int UpdateUser(SysUserDto user)
        {
            throw new NotImplementedException();
        }
    }
}
