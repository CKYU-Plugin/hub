using Hangfire;
using Hangfire.MemoryStorage;
using Owin;

namespace link.toroko.rsshub.OwinStartup
{
    public class Startup_Hangfire
    {
        public static void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();
            app.UseHangfireServer();
            app.UseHangfireDashboard("");
        }
    }
}
