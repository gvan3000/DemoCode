using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.WebApi.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Propositions controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class PropositionsController : BaseApiController
    {
        private readonly IPropositionsProvider _propositionsProvider;

        /// <summary>
        /// Initializes Propositions Controller
        /// </summary>
        /// <param name="propositionsProvider"></param>
        public PropositionsController(IPropositionsProvider propositionsProvider)
        {
            _propositionsProvider = propositionsProvider;
        }
        /// <summary>
        /// Get a list of all available propositions for the business unit
        /// </summary>
        /// <param name="id">Id of a requested business unit</param>
        /// <returns><see cref="PropositionsResponseModel"/>filled with the list of all availabe propositions for the business unit</returns>
        [HttpGet]
        [Route("{id}/propositions")]
        [ResponseType(typeof(PropositionsResponseModel))]
        [RouteAccessProviderIdSelector(AccessProvider.Contracts.RequestedResourceType.BusinessUnit)]
        [Description("6.6 Get Propositions")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _propositionsProvider.GetPropositionsAsync(id);
            if (response == null)
                result = this.NotFound($"There are no propositions for requested business unit with id of {id}");
            else
                result = Ok(response);

            return result;
        }
    }
}
