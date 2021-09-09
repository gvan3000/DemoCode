using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// company balance profile controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class BalanceProfileController: BaseApiController
    {
        private readonly IBalanceProfileProvider _provider;

        public BalanceProfileController(IBalanceProfileProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Get a list of available company balances for given business unit
        /// </summary>
        /// <param name="id">business unit id</param>
        /// <returns>list of company balances with ids and codes, ids are used in create product contract</returns>
        [HttpGet]
        [Route("{id}/balance-profiles")]
        [ResponseType(typeof(BalanceProfileListModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.16 Get Balance Profiles")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _provider.GetBalanceProfilesAsync(id);
            return ActionResultFromProviderOperation(result);
        }
    }
}