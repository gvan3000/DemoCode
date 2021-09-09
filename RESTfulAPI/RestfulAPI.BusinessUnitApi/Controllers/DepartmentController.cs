using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using RestfulAPI.AccessProvider.Attributes;
using RestfulAPI.AccessProvider.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.WebApi.Core;

namespace RestfulAPI.BusinessUnitApi.Controllers
{
    /// <summary>
    /// Department Controller
    /// </summary>
    [RoutePrefix("business-units")]
    public class DepartmentController : BaseApiController
    {
        private readonly IDepartmentProvider _departmentProvider;

        /// <summary>
        /// Department Controller initialize
        /// </summary>
        /// <param name="departmentProvider">Department provider</param>
        public DepartmentController(IDepartmentProvider departmentProvider)
        {
            _departmentProvider = departmentProvider;
        }

        /// <summary>
        /// Creates a new department for the business unit
        /// </summary>
        /// <param name="createModel">Model contains department information</param>
        /// <param name="id">Id of the business unit</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/departments")]
        [Description("6.25 Create Department")]
        [ResponseType(typeof(CreateDepartmentResponseModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> Post([FromBody]CreateDepartmentModel createModel, Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var providerResponse = await _departmentProvider.CreateAsync(createModel, id);

            return ActionResultFromProviderOperation(providerResponse);
        }

        /// <summary>
        /// Get all departments of the business unit.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/departments")]
        [ResponseType(typeof(GetDepartmentsModel))]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        [Description("6.24 Get all departments of the business unit")]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var providerResponse = await _departmentProvider.GetDepartmentAsync(id);

            return ActionResultFromProviderOperation(providerResponse);
        }

        [HttpPatch]
        [Route("{id}/departments/{department_id}")]
        [ResponseType(typeof(object))]
        [Description("6.26 Updates a department")]
        [RouteAccessProviderIdSelector(RequestedResourceType.BusinessUnit)]
        public async Task<IHttpActionResult> Patch(Guid department_id, [FromBody]UpdateDepartmentModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            request.Id = department_id;
            var response = await _departmentProvider.UpdateDepartmentAsync(request);
            var result = ActionResultFromProviderOperation(response);

            return result;
        }

    }
}