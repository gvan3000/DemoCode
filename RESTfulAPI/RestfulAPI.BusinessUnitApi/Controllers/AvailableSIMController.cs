using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels;
using RestfulAPI.Constants;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Available SIM controller 
    /// </summary>
    [RoutePrefix("business-units")]
    public class AvailableSIMController : BaseApiController
    {
        private readonly ISimProvider _simProvider;

        /// <summary>
        /// Initializes Available SIM Controller
        /// </summary>
        /// <param name="simProvider"></param>
        public AvailableSIMController(ISimProvider simProvider)
        {
            _simProvider = simProvider;
        }

        /// <summary>
        /// Get a list of all available SIM for the business unit
        /// </summary>
        /// <param name="id">Id of a requested business unit</param>
        /// <param name="query">Sim status filter</param>
        /// <returns><see cref="AvailableSIMResponseModel"/>list of all available SIMs for the business unit filled with the information</returns>
        [HttpGet]
        [Route("{id}/sim")]
        [ResponseType(typeof(AvailableSIMResponseModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.8 Get Available SIM")]
        public async Task<IHttpActionResult> Get(Guid id, [FromUri(Name = "q")]string query)
        {
            IHttpActionResult result;
            if (string.IsNullOrWhiteSpace(query))
            {
                ModelState.AddModelError("status", MessageConstants.InvalidInputMessage);
                return BadRequest(ModelState);
            }

			var regexMatch = Regex.Match(query, "status:(.*)$");
			if (!regexMatch.Success)
			{
				ModelState.AddModelError("status", MessageConstants.InvalidInputMessage);
				return BadRequest(ModelState);
			}
			string status = regexMatch.Groups[1].Value;
			var providerResponse = await _simProvider.GetAvailableSIMsAsync(id, status);

            result = ActionResultFromProviderOperation(providerResponse);
            return result;
        }

		[HttpGet]
		[Route("{id}/sim/v2")]
		[ResponseType(typeof(AvailableSIMResponseModel))]
		[RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
		[Description("6.8 Get Available SIM (v2)")]
		public async Task<IHttpActionResult> GetV2(Guid id, 
			string status = "", 
			[FromUri(Name = "per_page")] int? perPage = null, 
			int? page = null)
		{
			var providerResponse = await _simProvider.GetAvailableSIMsAsync(new AvailableSimProviderRequest
			{
				AccountId = id,
				Status = status,
				PerPage = perPage,
				Page = page
			});
            return ActionResultFromProviderOperation(providerResponse);
        }
    }
}
