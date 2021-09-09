using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.WebApi.Core;
using RestfulAPI.WebApi.Core.ModelBinders;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Availabe MSISDN cotroller
    /// </summary>
    [RoutePrefix("business-units")]
    public class AvailableMSISDNController : BaseApiController
    {
        private readonly IMobileProvider _mobileProvider;
        const string QUERY_NAME = "q";

        /// <summary>
        /// Initializes Available MSISDN Controller
        /// </summary>
        /// <param name="mobileProvider"></param>
        public AvailableMSISDNController(IMobileProvider mobileProvider)
        {
            _mobileProvider = mobileProvider;
        }

        /// <summary>
        /// Get a list of all available MSISDNS for the business unit
        /// </summary>
        /// <param name="id">Business unit id</param>
        /// <param name="query">Contains msisdns status filter</param>
        /// <param name="msisdnStatus">Msisdns status filter</param>
        /// <param name="includeChildren">Business unit children flag</param>
        /// <param name="country">Country filter, Optional</param>
        /// <param name="perPage">Total number of msisdns per page, used for pagination. Default value is 50. Maximum value is 250</param>
        /// <param name="page">Total number of msisdns per page, used for pagination. Default value is 1</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/msisdns")]
        [ResponseType(typeof(AvailableMSISDNResponseModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.7 Get Available MSISDN")]
        public async Task<IHttpActionResult> Get(Guid id,
                                                 [ModelBinder(typeof(ColumnSeparatedKeyValueBinder<AvailableMSISDNSearchModel>), Name = QUERY_NAME)]AvailableMSISDNSearchModel query,
                                                 [FromUri(Name = "status")]string msisdnStatus = null,
                                                 [FromUri(Name = "children")] bool includeChildren = false,
                                                 [FromUri(Name = "country")] string country = null,
                                                 [FromUri(Name = "per_page")] int? perPage = null,
                                                 [FromUri(Name = "page")] int? page = null)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var request = new AvailableMsisdnProviderRequest()
            {
                BusinessUnitId = id,
                MsisdnStatus = msisdnStatus,
                QueryMsisdnStatus = query?.Status,
                IncludeChildren = includeChildren,
                CountryCode = country,
                Page = page,
                PerPage = perPage
            };

            var response = await _mobileProvider.GetAvailableMsisdnsAsync(request);

            result = ActionResultFromProviderOperation(response);

            return result;
        }
    }
}
