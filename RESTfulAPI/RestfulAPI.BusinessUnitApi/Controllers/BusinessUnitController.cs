using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Cache;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.WebApi.Core;
using RestfulAPI.WebApi.Core.ModelBinders;
using RestfulAPI.WebApi.Core.StubsProvider;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Business Unit Controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class BusinessUnitController : BaseApiController
    {
        const string QUERY_NAME = "q";

        private readonly IBusinessUnitProvider _businessUnitProvider;
        private readonly IBusinessUnitCacheProvider _cacheProvider;
        private readonly IBusinessUnitForCompanyCacheProvider _companyCacheProvider;
        private readonly IAddOnProvider _addOnProvider;
        private readonly IAPNProvider _apnProvider;
        private readonly ISmscSettingProvider _smscSettingProvider;
        private readonly IFeatureProvider _featureProvider;
        private readonly IPreferredLanguageProvider _preferredLanguageProvider;

        /// <summary>
        /// Initialize Business unit controller
        /// </summary>
        /// <param name="businessUnitProvider"></param>
        /// <param name="cacheProvider"></param>
        /// <param name="companyCacheProvider"></param>
        /// <param name="addOnProvider"></param>
        /// <param name="apnProvider"></param>
        /// <param name="smscSettingProvider"></param>
        /// <param name="featureProvider"></param>
        /// <param name="preferredLanguageProvider"></param>
        public BusinessUnitController(
            IBusinessUnitProvider businessUnitProvider,
            IBusinessUnitCacheProvider cacheProvider,
            IBusinessUnitForCompanyCacheProvider companyCacheProvider,
            IAddOnProvider addOnProvider,
            IAPNProvider apnProvider,
            ISmscSettingProvider smscSettingProvider,
            IFeatureProvider featureProvider,
            IPreferredLanguageProvider preferredLanguageProvider)
        {
            _businessUnitProvider = businessUnitProvider;
            _cacheProvider = cacheProvider;
            _companyCacheProvider = companyCacheProvider;
            _addOnProvider = addOnProvider;
            _apnProvider = apnProvider;
            _smscSettingProvider = smscSettingProvider;
            _featureProvider = featureProvider;
            _preferredLanguageProvider = preferredLanguageProvider;
        }

        /// <summary>
        /// Search business units by specified criteria: eg. q=name:business_unit_name, q=customer_id:abc123, q=has_shared_wallet:true
        /// </summary>
        /// <param name="query">Query Business Unit by name (Example: /business-units/search?q=name:{full_name})
        /// Query Business Unit by customer_id (Example: /business-units/search?q=customer_id:{customer_id})
        /// Query Business Unit by has_shared_wallet status (Example: /business-units/search?q=has_shared_wallet:{bool})
        /// </param>
        /// <param name="includeChildren">specifies whether to return contract with populated children or not</param>
        /// <returns> data <see cref="BusinessUnitModel"/>filled with information about requested business units or HTTP 404 not found in case person does not exist</returns>
        [HttpGet]
        [Route("search")]
        [ResponseType(typeof(BusinessUnitListModel))]
        [Description("6.1 Get Business Unit")]
        public async Task<IHttpActionResult> Get([ModelBinder(typeof(ColumnSeparatedKeyValueBinder<BusinessUnitSearchModel>), Name = QUERY_NAME)]BusinessUnitSearchModel query, [FromUri(Name = "children")] bool includeChildren = false)
        {
            IHttpActionResult result;


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = new Domain.ApiModelProviders.Contracts.GetBusinessUnitRequest()
            {
                UserCompanyId = UserCompanyId,
                UserBusinessUnitId = UserBusinessUnitId,
                FilterBusinessUnitName = query.Name,
                FilterCustomerId = query.CustomerId,
                FilterHasSharedWallet = query.HasSharedWallet,
                IncludeChildren = includeChildren
            };

            if (!string.IsNullOrEmpty(query.EndUserSubscription))
                request.FilterHasEndUserSubscripion = bool.Parse(query.EndUserSubscription);

            var businessUnitResponse = await _businessUnitProvider.GetBusinessUnitsWithFilteringAsync(request);
            if (businessUnitResponse == null)
            {
                result = this.NotFound($"There are no business units matching requested query");
                return result;
            }

            result = Ok(businessUnitResponse);
            return result;
        }

        /// <summary>
        /// Get the business unit details by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeChildren">specifies whether to return contract with populated children or not</param>
        /// <returns><see cref="BusinessUnitModel"/>filled with information about requested business unit</returns>
        [HttpGet]
        [Route("{id}", Name = "BusinessUnitRoute.GetById")]
        [ResponseType(typeof(BusinessUnitModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.1 Get Business Unit By Id")]
        public async Task<IHttpActionResult> Get(Guid id, [FromUri(Name = "children")] bool includeChildren = false)
        {
            IHttpActionResult result;

            var request = new Domain.ApiModelProviders.Contracts.GetBusinessUnitRequest
            {
                UserCompanyId = UserCompanyId,
                UserBusinessUnitId = UserBusinessUnitId,
                FilterBusinessUnitId = id,
                IncludeChildren = includeChildren
            };

            var businessUnitResponse = await _businessUnitProvider.GetBusinessUnitsWithFilteringAsync(request);
            if (businessUnitResponse == null || businessUnitResponse.BusinessUnits.Count == 0)
            {
                result = this.NotFound($"Requested business unit with id of {id} does not exist");
                return result;
            }

            result = Ok(businessUnitResponse.BusinessUnits.First()); // need to "unpack" result since we are getting the collection here but need to return one instance
            return result;
        }

        /// <summary>
        /// Get all business units
        /// </summary>
        /// <param name="includeChildren">specifies whether to return contract with populated children or not</param>
        /// <returns></returns>
        [HttpGet]
        [Route()]
        [ResponseType(typeof(BusinessUnitListModel))]
        [Description("6.1 Get Business Units")]
        public async Task<IHttpActionResult> Get([FromUri(Name = "children")] bool includeChildren = false)
        {
            IHttpActionResult result;

            var request = new Domain.ApiModelProviders.Contracts.GetBusinessUnitRequest
            {
                UserCompanyId = UserCompanyId,
                UserBusinessUnitId = UserBusinessUnitId,
                FilterBusinessUnitId = UserBusinessUnitId,
                IncludeChildren = includeChildren
            };

            var businessUnitResponse = await _businessUnitProvider.GetBusinessUnitsWithFilteringAsync(request);

            result = Ok(businessUnitResponse);
            return result;
        }

        /// <summary>
        /// Create a new Business Unit from supplied data
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Nothing</returns>
        [HttpPost]
        [Route]
        [ResponseType(typeof(CreateBusinessUnitResponseModel))]
        [ResponseTypeCode(System.Net.HttpStatusCode.Accepted)]
        [Description("6.2 Create Business Unit")]
        public async Task<IHttpActionResult> Post([FromBody]BusinessUnitCreateModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _cacheProvider.ValidateCacheHistory();
            _companyCacheProvider.ValidateCacheHistory();

            //check if parentId is supplied
            if (request.ParentId.GetValueOrDefault() != Guid.Empty)
            {
                bool hasAccess = await HasAccessAsync(RequestedResourceType.BusinessUnit, request.ParentId.GetValueOrDefault());
                if (!hasAccess)
                {
                    return this.NotFound($"Could not find parent business unit with id of {request.ParentId}");
                }
            }

            //set parentId if it is not supplied
            if (request.ParentId == null || request.ParentId == Guid.Empty)
            {
                request.ParentId = UserBusinessUnitId;
            }

            if (request.Propositions != null)
            {
                int? distinctPropositions = request.Propositions.Select(p => p.Id).Distinct().Count();
                if (request.Propositions.Length != distinctPropositions)
                    return BadRequest("Duplicate propositions");
            }

            int? distinctAddOns = request.AddOns?.Select(a => a).Distinct().Count();
            if (request.AddOns?.Count != distinctAddOns)
                return BadRequest("Duplicate add-ons");

            bool allAddOnExists = false;
            if (request.AddOns?.Count > 0)
            {
                allAddOnExists = await _addOnProvider.ValidateAddOns(request.AddOns);
            }

            if (request.AddOns?.Count > 0 && !allAddOnExists)
                return BadRequest("Invalid add-ons provided");

            if (request.UseOABasedSmsRouting.GetValueOrDefault())
            {
                var isEnabled = await _featureProvider.IsCompanyFeatureEnabled(UserCompanyId,
                    CompanyFeatureConstants.SmscSettingsFeatureCode).ConfigureAwait(false);
                if (!isEnabled)
                    return BadRequest("OA Based Sms Routing is not available for current mvno");

            }

            var response = await _businessUnitProvider.CreateAsync(UserCompanyId, UserBusinessUnitId, request, RequestId);

            if (response != null && response.IsSuccess && request.UseOABasedSmsRouting.GetValueOrDefault())
            {
                var email = UserClaims.Claims.FirstOrDefault(x => x.Type == "preferred_username")?.Value; // TODO VALIDATE
                var createSmscSettingsResult = await _smscSettingProvider.CreateOrUpdate(request.UseOABasedSmsRouting.Value, response.Result.Id, email
                    ).ConfigureAwait(false);
                if (!createSmscSettingsResult.IsSuccess)
                {
                    return ActionResultFromProviderOperation(createSmscSettingsResult);
                }
            }

            if (response != null && response.IsSuccess && request.AddOns?.Count > 0)
            {
                var addOnAddResult = await _addOnProvider.AddAllowedAddOnsToBusinessUnit(request.AddOns, response.Result.Id, UserCompanyId);
                if (!addOnAddResult.IsSuccess)
                    return ActionResultFromProviderOperation(addOnAddResult);
            }

            if (response != null && response.IsSuccess)
            {
                _cacheProvider.NotifyBusinessUnitCacheChanged(request.Name, response.Result.Id);
                _companyCacheProvider.NotifyBusinessUnitCacheChanged(request.Name, response.Result.Id);

            }

            var result = ActionResultFromProviderOperation(response);
            return result;
        }

        /// <summary>
        /// Update only one property of the business unit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns>Nothing</returns>
        [HttpPatch]
        [Route("{id}")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.3 Patch Business Unit")]
        public async Task<IHttpActionResult> Patch(Guid id, [FromBody]BusinessUnitPatchModel value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _businessUnitProvider.UpdateBusinessUnitAsync(id, value);
            return ActionResultFromProviderOperation(response);
        }

        /// <summary>
        /// Gets the APNs available for the company
        /// </summary>
        /// <returns></returns>
        [Route("available-apns")]
        [HttpGet]
        [ResponseType(typeof(APNSetList))]
        [Description("6.22 Get available APNs")]
        public async Task<IHttpActionResult> GetAsync()
        {
            var result = await _apnProvider.GetCompanyAPNsAsync(UserCompanyId);
            return Ok(result);
        }

        /// <summary>
        /// Set APN list for business unit.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Route("{id}/apns")]
        [HttpPut]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.19 Update APNs")]
        public async Task<IHttpActionResult> UpdateApns(Guid id, UpdateBusinessUnitApnsModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _apnProvider.UpdateBusinessUnitApnsAsync(id, UserCompanyId, input);
            return ActionResultFromProviderOperation(result);
        }

        /// <summary>
        /// List all the APNs that are linked to the business unit.
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/apns")]
        [ResponseType(typeof(APNsResponseModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.18 Get APNs")]
        public async Task<IHttpActionResult> GetAPNsByBusinessUnitId(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ProviderOperationResult<APNsResponseModel> providerResult = await _apnProvider.GetAPNsAsync(id);

            return ActionResultFromProviderOperation(providerResult);
        }


        /// <summary>
        /// Set the default APN for business unit. This APN will be used for product creation.
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <param name="input">Name of default APN</param>
        /// <returns></returns>
        [Route("{id}/apns/default-apn")]
        [HttpPut]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.20 Update Default APN")]
        public async Task<IHttpActionResult> UpdateDefaultApn(Guid id, UpdateBusinessUnitDefaultApnModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _apnProvider.UpdateBusinessUnitDefaultApnAsync(id, UserCompanyId, input);
            return ActionResultFromProviderOperation(result);
        }

        /// <summary>
        /// Removes the link between the APN and the business unit.
        /// </summary>
        /// <param name="id">Id of the business unit</param>
        /// <param name="apnId">Id of the APN</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}/apns/{apnId}")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.21 Remove APN link")]
        public async Task<IHttpActionResult> RemoveApn(Guid id, Guid apnId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _apnProvider.RemoveApnAsync(id, apnId);

            return ActionResultFromProviderOperation(providerResult);
        }

        /// <summary>
        /// Get the available languages of the business unit.
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/available-languages")]
        [ResponseType(typeof(AvailableLanguagesResponseModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> GetBusinessUnitAvailableLanguages(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _preferredLanguageProvider.GetAvailableLanguagesAsync(id, UserCompanyId);

            return ActionResultFromProviderOperation(providerResult);
        }

        /// <summary>
        /// Get the preferred languages of the business unit.
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/allowed-languages")]
        [ResponseType(typeof(PreferredLanguageResponseModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> GetBusinessUnitPreferredLanguages(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _preferredLanguageProvider.GetAccountLanguagesAsync(id);

            return ActionResultFromProviderOperation(providerResult);
        }

        /// <summary>
        /// Update the preferred languages that are available to the business unit.
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <param name="input">Update preferred languages request model</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{id}/allowed-languages")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> UpdateBusinessUnitPreferredLanguages(Guid id, UpdatePreferredLanguagesRequestModel input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResult = await _preferredLanguageProvider.UpdateAccountLanguagesAsync(id, UserCompanyId, input);

            return ActionResultFromProviderOperation(providerResult);
        }
    }
}
