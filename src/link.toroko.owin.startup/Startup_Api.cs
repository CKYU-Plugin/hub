using Owin;
using System.Net.Http.Formatting;
using System.Web.Http;


namespace link.toroko.owin.startup
{
    public class Startup_Api
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
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

            config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "application/xml");
            config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");

            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
            app.UseWebApi(config);
        }
    }
}
