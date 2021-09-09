using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Add-On Controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class AddOnController : BaseApiController
    {
        private readonly IAddOnProvider _addOnProvider;

        /// <summary>
        /// Initializes AddOn Controller
        /// </summary>
        /// <param name="addOnProvider"></param>
        public AddOnController(IAddOnProvider addOnProvider)
        {
            _addOnProvider = addOnProvider;
        }

        /// <summary>
        /// Gets add-ons for business unit
        /// </summary>
        /// <param name="id">Id of the BusinessUnit</param>
        /// <param name="includeExpired">Expired add-ons filter parameter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/purchased-add-ons")]
        [ResponseType(typeof(AddOnListModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.10	Get Add-Ons")]
        public async Task<IHttpActionResult> Get(Guid id, [FromUri(Name = "include_expired")] bool includeExpired = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var request = new GetPurchasedAddonsBusinessUnitRequest
            {
                 BusinessUnitId = id,
                 IncludeExpired = includeExpired
            };

            var response = await _addOnProvider.GetAddOnsAsync(request);
            if (response == null)
            {
                return this.NotFound($"Add-ons do not exist for Business Unit id {id}");
            }

            return Ok(response);
        }

        /// <summary>
        /// Adds a shared add-on to the business unit
        /// </summary>
        /// <param name="addOn">Id of the add-on</param>
        /// <param name="id">Id of the Business Unit</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/purchased-add-ons")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.9 Post Add-On")]
        public async Task<IHttpActionResult> Post([FromBody]PurchaseAddOnModel addOn, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _addOnProvider.AddAddOnAsync(addOn, id, RequestId);

            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Removes the add-on from the business unit.
        /// </summary>
        /// <param name="deleteModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}/purchased-add-on")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.12 Delete Add-On")]
        public async Task<IHttpActionResult> Delete([FromBody]DeleteAddOnModel deleteModel, Guid id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool hasAccess = await HasAccessAsync(RequestedResourceType.AddOn, deleteModel.AddOnId);
            if (!hasAccess)
            {
                return this.NotFound($"Could not find add on with id: {deleteModel.AddOnId}");
            }

            var response = await _addOnProvider.DeleteAddOnAsync(deleteModel, id);

            return ActionResultFromProviderOperation(response);
        }
    }
}