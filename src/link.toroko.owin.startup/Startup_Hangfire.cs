using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.owin.startup
{
    public class Startup_Hangfire
    {
        public void Configuration(IAppBuilder app)
        {
            ////指定Hangfire使用内存存储后台任务信息
            GlobalConfiguration.Configuration.UseMemoryStorage();
            ////启用HangfireServer这个中间件（它会自动释放）
            app.UseHangfireServer();
            ////启用Hangfire的仪表盘（可以看到任务的状态，进度等信息）
            app.UseHangfireDashboard("");
            //启用WebApi
        }


    }
}
