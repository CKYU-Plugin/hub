using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace link.toroko.rsshub.OwinStartup
{
    public  class Startup_Files
    {
        public static void Configuration(IAppBuilder app)
        {
            var fileSystem = new PhysicalFileSystem(Initialization.staticFilesDir);
            var options = new FileServerOptions
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem
            };
            app.UseFileServer(options);
        }
    }

}
