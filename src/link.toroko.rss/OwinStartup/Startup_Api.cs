using Owin;
using Robot.Property;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace link.toroko.rsshub.OwinStartup
{
    public class Startup_Api
    {
        public static void Configuration(IAppBuilder app)
        {
        //    GlobalConfiguration.Configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver());

            System.Web.Http.HttpConfiguration config = new System.Web.Http.HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
            name: "api",
            routeTemplate: "api"
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApiExt2",
            routeTemplate: "api/{controller}/{action}.{ext}",
            defaults: new { action = RouteParameter.Optional, ext = RouteParameter.Optional },
            constraints: null
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApi2",
            routeTemplate: "api/{controller}/{action}",
            defaults: new { action = RouteParameter.Optional },
            constraints: null
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApiExt1",
            routeTemplate: "api/{controller}/{action}/{id}.{ext}",
            defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional, ext = RouteParameter.Optional },
            constraints: null
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApi1",
            routeTemplate: "api/{controller}/{action}/{id}",
            defaults: new { action = RouteParameter.Optional, id = RouteParameter.Optional },
            constraints: null
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApiExt0",
            routeTemplate: "api/{controller}/{action}/{id0}/{id1}.{ext}",
            defaults: new { action = RouteParameter.Optional, id0 = RouteParameter.Optional, id1 = RouteParameter.Optional, ext = RouteParameter.Optional },
            constraints: null
            );

            config.Routes.MapHttpRoute(
            name: "DefaultApi0",
            routeTemplate: "api/{controller}/{action}/{id0}/{id1}",
            defaults: new { action = RouteParameter.Optional, id0 = RouteParameter.Optional, id1 = RouteParameter.Optional },
            constraints: null
            );

         //   config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "application/xml");
        //    config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            app.UseWebApi(config);
        }
    }

    public class CustomAssemblyResolver : IAssembliesResolver
    {
        public ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);

            string thirdPartySource = Path.Combine(RobotBase.currentfloder, "data\\webapi\\controller") ;

            if (!string.IsNullOrWhiteSpace(thirdPartySource))
            {
                if (Directory.Exists(thirdPartySource))
                {
                    foreach (var file in Directory.GetFiles(thirdPartySource, "*.*", SearchOption.AllDirectories))
                    {
                        if (Path.GetExtension(file) == ".dll")
                        {
                            var externalAssembly = Assembly.LoadFrom(file);
                            assemblies.Add(externalAssembly);
                        }
                    }
                }
            }
            return assemblies;
        }
    }

}
