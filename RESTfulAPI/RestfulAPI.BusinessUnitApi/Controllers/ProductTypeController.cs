using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Product Type Controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class ProductTypeController : BaseApiController
    {
        private readonly IProductTypeProvider _productTypeProvider;
        /// <summary>
        /// Initialize ProductTypeController 
        /// </summary>
        public ProductTypeController(IProductTypeProvider productTypeProvider)
        {
            _productTypeProvider = productTypeProvider;
        }

        /// <summary>
        /// Get all product types by busines unit id
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/product-types")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.14 Get Product Types")]
        public async Task<IHttpActionResult> Get([FromUri(Name = "id")]Guid businessUnitId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _productTypeProvider.GetProductTypesAsync(businessUnitId);

            return ActionResultFromProviderOperation(response);
        }
    }
}