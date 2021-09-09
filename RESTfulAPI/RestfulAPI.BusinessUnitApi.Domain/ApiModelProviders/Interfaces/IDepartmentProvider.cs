using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Department BU interface
    /// </summary>
    public interface IDepartmentProvider
    {
        /// <summary>
        /// Create Department of the business unit
        /// </summary>
        /// <param name="businessUnitId">Id of the Business Unit</param>
        /// <param name="model">Model contains department information</param>
        /// <returns>data contract with id of the newly created department</returns>
        Task<ProviderOperationResult<CreateDepartmentResponseModel>> CreateAsync(CreateDepartmentModel model, Guid businessUnitId);
        
        /// <summary>
        /// Get departments of the business unit
        /// </summary>
        /// <param name="busineesUnitId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<GetDepartmentsModel>> GetDepartmentAsync(Guid busineesUnitId);

        /// <summary>
        /// Updates department cost center
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> UpdateDepartmentAsync(UpdateDepartmentModel request);
    }
}
