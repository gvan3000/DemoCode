using Microsoft.Practices.ServiceLocation;
using RestfulAPI.Logging;

namespace RestfulAPI.BusinessUnitApi.Domain
{
    public class LoggerProvider : IJsonRestApiLoggerProvider
    {
        public IJsonRestApiLogger GetLogger()
        {
            var instance = ServiceLocator.Current.GetInstance<IJsonRestApiLogger>();
            return instance;
        }
    }
}
