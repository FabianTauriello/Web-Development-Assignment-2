using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Web.Routing;

namespace PowerInterruptions
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteTable.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{year}/{lga}",
               defaults: new
               {
                   year = System.Web.Http.RouteParameter.Optional,
                   lga = System.Web.Http.RouteParameter.Optional
               }
           );
        }
    }
}