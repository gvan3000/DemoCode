using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// ProductType Provider Interface
    /// </summary>
    public interface IProductTypeProvider
    {
        /// <summary>
        /// Get all product types by business unit
        /// </summary>
        /// <returns></returns>
        Task<ProviderOperationResult<ProductTypeListResponseModel>> GetProductTypesAsync(Guid businessUnitId);
    }
}
