using System.Web.Http;
using WebActivatorEx;
using RestfulAPI.BusinessUnitApi;
using RestfulAPI.Configuration.Swagger;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace RestfulAPI.BusinessUnitApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            GlobalConfiguration.Configuration.EnableSwaggerWithSingleApiVersionAndUI();
        }
    }
}