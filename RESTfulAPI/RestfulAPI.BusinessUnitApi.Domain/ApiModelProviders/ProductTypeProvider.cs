using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Logging;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ProductTypeService;
using System.ServiceModel;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <summary>
    /// ProductTypes Provider
    /// </summary>
    public class ProductTypeProvider : LoggingBase, IProductTypeProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;

        /// <summary>
        /// Initialize ProductTypeProvider
        /// </summary>
        /// <param name="serviceUnitOfWork"></param>
        /// <param name="translators"></param>
        /// <param name="logger"></param>
        public ProductTypeProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, IJsonRestApiLogger logger)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
        }

        /// <summary>
        /// Get all product types by business unit
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<ProductTypeListResponseModel>> GetProductTypesAsync(Guid businessUnitId)
        {
            if (Equals(Guid.Empty, businessUnitId))
            {
                throw new ArgumentException("must be valid business unit id", nameof(businessUnitId));
            }

            var businessUnit = await _serviceUnitOfWork.AccountService.GetAccountByIdAsync(businessUnitId);

            if (businessUnit == null)
            {
                return ProviderOperationResult<ProductTypeListResponseModel>.NotFoundResult($"BusinessUnit with id:{businessUnitId} could not be found");
            }

            var productTypesRequest = new GetProductTypesByCompanyContract
            {
                CompanyId = businessUnit.CompanyId.GetValueOrDefault()
            };

            ProductTypeContract[] serviceResponse;
            try
            {
                serviceResponse = await _serviceUnitOfWork.ProductTypeService.GetProductTypesByCompanyAsync(productTypesRequest);
            }
            catch (FaultException<TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error, "Error getting product types", nameof(GetProductTypesAsync), exc);
                return ProviderOperationResult<ProductTypeListResponseModel>.TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(businessUnitId), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }            

            var productTypes = _translators.ProductTypeContractTranslator.Translate(serviceResponse);

            return ProviderOperationResult<ProductTypeListResponseModel>.OkResult(productTypes);
        }
    }
}
