using Newtonsoft.Json;
using RestfulAPI.AccessProvider.Configuration;
using RestfulAPI.BusinessUnitApi.Domain;
using RestfulAPI.Logging;
using RestfulAPI.WebApi.Core;
using System.Web.Http;
using Newtonsoft.Json.Converters;
using RestfulAPI.Configuration.JsonFormatter;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var startupLogger = config.DependencyResolver.GetService(typeof(IJsonRestApiLogger)) as IJsonRestApiLogger; // don't use this to pass to other parts of the system
            startupLogger.Log(LogSeverity.Info, $"starting api {typeof(WebApiConfig).Assembly.FullName}", nameof(Register));

            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            //config.MessageHandlers.Add(new LogMessageHandler(new LoggerProvider()));

            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(Microsoft.Owin.Security.OAuth.OAuthDefaults.AuthenticationType));
            config.ConfigureJsonFormatter();

            // Web API routes
            config.MapHttpAttributeRoutes();

            //logging
            var apiVersion = typeof(WebApiConfig).Assembly.GetName().Version;
            config.Filters.Add(new RequestAndResponseIdWritterFilter(apiVersion));
            config.Filters.Add(new LogMessageActionFilter(new LoggerProvider()));
            config.Filters.Add(new ExceptionLoggingFilter(new LoggerProvider()));

            //validation
            config.Filters.Add(new AuthorizeAttribute());
            config.Filters.Add(new ValidateEmptyRequestBodyFilter());

#if STUBS
            config.Filters.Add(new StubsActionFilter());
#endif

            config.RegisterAccessProvider();

        }
    }
}
