using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace link.toroko.owin.startup
{
    public class Startup_BaseWeb
    {
        public void Configuration(IAppBuilder app)
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
