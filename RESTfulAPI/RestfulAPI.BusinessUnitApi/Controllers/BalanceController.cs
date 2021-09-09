using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Shared Balance Controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class BalanceController : BaseApiController
    {
        private readonly IBalanceProvider _balanceProvider;
        private readonly IQuotaDistributionProvider _quotaDistributionProvider;

        /// <summary>
        /// Initialize new object of type BalanceController
        /// </summary>
        /// <param name="balanceProvider"></param>
        /// <param name="quotaDistributionProvider"></param>
        public BalanceController(IBalanceProvider balanceProvider, IQuotaDistributionProvider quotaDistributionProvider)
        {
            _balanceProvider = balanceProvider;
            _quotaDistributionProvider = quotaDistributionProvider;
        }

        /// <summary>
        /// Get a list of all balances of the business unit, if available.
        /// </summary>
        /// <param name="id">Id of a business unit</param>
        /// <returns> Shared balances response model filled with information about requested business unit balances or HTTP 404 not found in case business unit does not exist</returns>
        [HttpGet]
        [Route("{id}/balances")]
        [ResponseType(typeof(BalancesResponseModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.5 Get Shared Balance")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProviderOperationResult<BalancesResponseModel> model = await _balanceProvider.GetBalancesAsync(id);

            return ActionResultFromProviderOperation(model);
        }

        /// <summary>
        /// Set the amount to be used by a product on a shared balance.
        /// </summary>
        /// <param name="id">Id of the business unit</param>
        /// <param name="productId">Id of the product</param>
        /// <param name="model">Model contains data for set balance per product</param>
        /// <returns>Returns 202 if successful</returns>
        [HttpPost]
        [Route("{id}/balance/product/{productId}")]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.11 Set Balance Amount for Product")]
        public async Task<IHttpActionResult> Post([FromUri]Guid id, [FromUri]Guid productId, [FromBody]SetBalanceModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hasAccess = await HasAccessAsync(RequestedResourceType.Product, productId);
            if (!hasAccess)
            {
                return this.NotFound($"Could not find Product by id : {productId}");
            }

            var response = await _balanceProvider.SetBalanceAsync(model, id, productId, RequestId);

            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Gets shared balance for product
        /// </summary>
        /// <param name="id">BusinessUnitId</param>
        /// <param name="productId">ProductId</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/balance/product/{productId}")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.13 Get Balance Amount for Product")]
        public async Task<IHttpActionResult> Get([FromUri]Guid id, [FromUri]Guid productId)
        {
            var hasAccess = await HasAccessAsync(RequestedResourceType.Product, productId);
            if (!hasAccess)
            {
                return this.NotFound($"Could not find Product by id : {productId}");
            }

            var response = await _quotaDistributionProvider.GetSharedBalancesForProductAsync(id, productId);

            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Get All Shared Balance Products
        /// </summary>
        /// <param name="id">Id of the business unit</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/balances/products")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.17 get All Shared Balance Products")]
        public async Task<IHttpActionResult> GetAllShared([FromUri]Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _quotaDistributionProvider.GetAllSharedBalancesPerBusinessUnitAsync(id);

            return ActionResultFromProviderOperation(response);
        }
    }
}
