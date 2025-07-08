using Ge.Model;
using Ge.Model.System;
using Ge.Model.System.Dto;

namespace Ge.ServiceCore.Services
{
    /// <summary>
    /// 数据差异日志service接口
    /// </summary>
    public interface ISqlDiffLogService : IBaseService<SqlDiffLog>
    {
        PagedInfo<SqlDiffLogDto> GetList(SqlDiffLogQueryDto parm);

        SqlDiffLog GetInfo(long PId);

        SqlDiffLog AddSqlDiffLog(SqlDiffLog parm);

        int UpdateSqlDiffLog(SqlDiffLog parm);

    }
}
