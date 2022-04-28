using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApiFox
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var js = config.Formatters.JsonFormatter;
            //js.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            js.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Configuración y servicios de API web
            // Rutas de API web
            config.MapHttpAttributeRoutes();
            config.EnableCors();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
