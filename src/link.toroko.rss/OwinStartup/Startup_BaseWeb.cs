using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace link.toroko.rsshub.OwinStartup
{
    public  class Startup_BaseWeb
    {
        public static void Configuration(IAppBuilder app)
        {
            var fileSystem = new PhysicalFileSystem(Initialization.webDir);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                FileSystem = fileSystem
            };

            app.UseFileServer(options);
        }
    }
}
