using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    class FeatureProvider : LoggingBase, IFeatureProvider
    {
        private readonly ITeleenaServiceUnitOfWork _teleenaServiceUnitOfWork;

        public FeatureProvider(IJsonRestApiLogger logger, ITeleenaServiceUnitOfWork teleenaServiceUnitOfWork) : base(logger)
        {
            _teleenaServiceUnitOfWork = teleenaServiceUnitOfWork;
        }


        public Task<bool> IsCompanyFeatureEnabled(Guid companyId, string featureCode)
        {
            return _teleenaServiceUnitOfWork.FeatureService.IsCompanyFeatureEnabledAsync(companyId, featureCode);
        }
    }
}
