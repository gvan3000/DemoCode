using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Returns list of products for specified business unit
    /// </summary>
    public interface IProductProvider
    {
        /// <summary>
        /// Gets the list of products for requested business unit with some additional options
        /// </summary>
        /// <param name="businessUnitId">Business unit id supplied in parameter</param>
        /// <param name="companyId">Company id from user claims that business unit belongs to</param>
        /// <param name="includeChildren">Recursively return products for all sub business units, can be combined with <paramref name="creationDateFrom"/></param>
        /// <param name="creationDateFrom">Used to return only does products that are created after specified date, can be combined with <paramref name="includeChildren"/></param>
        /// <param name="pageNumber">number of page, 1 based</param>
        /// <param name="pageSize">number of results per page</param>
        /// <returns>List of products</returns>
        Task<ProviderOperationResult<ProductsListModel>> GetProductsForBusinessUnitAsync(Guid businessUnitId, Guid companyId, DateTime? creationDateFrom, bool? includeChildren, int pageNumber, int? pageSize);

        /// <summary>
        /// Gets a list of ProductImei records coupled with iccid
        /// </summary>
        /// <param name="request">Object containing the request parameters</param>
        /// <returns></returns>
        Task<ProviderOperationResult<ProductImeiListModel>> GetProductImeisForBusinessUnitAsync(GetProductImeiByBusinessUnitDataContract request);
    }
}