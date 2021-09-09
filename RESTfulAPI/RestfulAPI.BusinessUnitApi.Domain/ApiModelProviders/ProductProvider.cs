using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using RestfulAPI.TeleenaServiceReferences.ProductService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <inheritdoc />
    public class ProductProvider : IProductProvider
    {
        private readonly IBusinessUnitApiTranslators _translators;
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly ICustomAppConfiguration _appConfiguration;

        /// <summary>
        /// Initialize ProductProvider
        /// </summary>
        /// <param name="serviceUnitOfWork"></param>
        /// <param name="translators"></param>
        /// <param name="appConfiguration"></param>
        public ProductProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, ICustomAppConfiguration appConfiguration)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
            _appConfiguration = appConfiguration;
        }

        /// <inheritdoc />
        public async Task<ProviderOperationResult<ProductsListModel>> GetProductsForBusinessUnitAsync(Guid businessUnitId, Guid companyId, DateTime? creationDateFrom, bool? includeChildren, int pageNumber, int? pageSize)
        {
            var pageSizeInfo = _translators.PageSizeInfoTranslator.Translate(pageSize);

            var request = new GetProductsByAccountRequest
            {
                CompanyId = companyId,
                AccountId = businessUnitId,
                IncludeChildAccounts = includeChildren,
                CreationDateFrom = creationDateFrom,
                PageNumber = pageNumber,
                PageSize = pageSizeInfo.PageSize
            };

            var serviceResponse = await _serviceUnitOfWork.ProductService.GetProductsSearchByAccountIdAsync(request);
            if (serviceResponse?.Products == null || serviceResponse.Products.Length == 0)
            {
                return ProviderOperationResult<ProductsListModel>.OkResult(new ProductsListModel { Products = new List<ProductModel>() });
            }

            var hostName = _appConfiguration.GetConfigurationValue(ConfigurationConstants.RestfulApiDomainNameSection, ConfigurationConstants.RestAPIDomainName);
            var productApiPath = _appConfiguration.GetConfigurationValue(ConfigurationConstants.RestfullApiPathSection, ConfigurationConstants.ProductApi);

            var results = _translators.ProductListTranslator.Translate(serviceResponse.Products);

            results.ForEach(product => product.Location = $"{hostName}/{productApiPath}/products/{product.Id}");

            var result = new ProductsListModel {Products = results};

            if (pageSizeInfo.IsPaged)
            {
                result.PagingInfo = new PagingInfo { TotalProducts = serviceResponse.TotalResults };
            }

            return ProviderOperationResult<ProductsListModel>.OkResult(result);
        }

        /// <inheritdoc />
        public async Task<ProviderOperationResult<ProductImeiListModel>> GetProductImeisForBusinessUnitAsync(GetProductImeiByBusinessUnitDataContract request)
        {
            var serviceResponse = await _serviceUnitOfWork.ProductImeiService.GetProductImeiByBusinessUnitAsync(request);
            if (serviceResponse?.Model == null || serviceResponse.Model.Length == 0)
            {
                return ProviderOperationResult<ProductImeiListModel>.OkResult(new ProductImeiListModel{ Products = new List<ProductImeiModel>() });
            }

            var results = _translators.ProductImeiListTranslator.Translate(serviceResponse.Model);

            return ProviderOperationResult<ProductImeiListModel>.OkResult(results);
        }
    }
}