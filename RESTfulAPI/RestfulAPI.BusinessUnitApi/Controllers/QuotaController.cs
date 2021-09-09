using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    [RoutePrefix("business-units")]
    public class QuotaController: BaseApiController
    {
        private readonly IQuotaDistributionProvider _provider;

        public QuotaController(IQuotaDistributionProvider provider)
        {
            _provider = provider;
        }

        [HttpPatch]
        [Route("{id}/quota")]
        [ResponseType(typeof(object))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.32 Set Shared Subscription Quota")]

        public async Task<IHttpActionResult> Patch(Guid id, SetBusinessUnitQuotaModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var providerResult = await _provider.SetBusinessUnitQuota(id, input, UserClaims);
            return ActionResultFromProviderOperation(providerResult);
        }
    }
}