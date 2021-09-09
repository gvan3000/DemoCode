using Microsoft.Owin;
using Owin;
using RestfulAPI.ApplicationInsights;
using RestfulAPI.Configuration.IdentityServerAuthentication;
using System.Configuration;
using System.Reflection;
using TeleenaFileLogging;

[assembly: OwinStartup(typeof(RestfulAPI.BusinessUnitApi.Startup))]
namespace RestfulAPI.BusinessUnitApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseIdentityServerBearerTokenAuthFromConfiguration();
            
            log4net.Config.XmlConfigurator.Configure();
            JsonLogger.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            JsonLogger.Environment = ConfigurationManager.AppSettings["EnvironmentName"];

            app.UseApplicationInsightsMiddleware();
        }
    }
}