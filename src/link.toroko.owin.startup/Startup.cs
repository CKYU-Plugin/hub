using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace link.toroko.owin.startup
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            var fileSystem = new PhysicalFileSystem(Initialization.webDir);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                FileSystem = fileSystem
            };

            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { action = RouteParameter.Optional },
            constraints: null
            );

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseWebApi(config);
            app.UseFileServer(options);
        }
    }
}
