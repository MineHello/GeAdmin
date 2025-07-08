using Ge.Infrastructure.AppExtensions;
using Serilog;

namespace Ge.Admin.WebApi.Extensions
{
    /// <summary>
    /// 
    /// 通过事件获取WebApplication的状态。
    /// </summary>
    public static class ApplicationSetup
    {
        public static void UseApplicationSetup(this WebApplication app)
        {
            app.Lifetime.ApplicationStarted.Register(() =>
            {
                App.IsRun = true;
            });

            app.Lifetime.ApplicationStopped.Register(() =>
            {
                App.IsRun = false;

                //清除日志
                Log.CloseAndFlush();
            });
        }
    }
}
