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
    public class Startup_Files
    {
        public void Configuration(IAppBuilder app)
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
