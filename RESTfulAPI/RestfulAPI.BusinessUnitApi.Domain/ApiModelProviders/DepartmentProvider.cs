using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{    
    /// <summary>
    /// Department Provider
    /// </summary>
    public class DepartmentProvider : LoggingBase, IDepartmentProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _businessUnitApiTranslators;

        /// <summary>
        /// Department Provider Initialize
        /// </summary>
        /// <param name="serviceUnitOfWork">TelenaServiceUnitOfWork</param>
        /// <param name="businessUnitApiTranslators">business unit translators</param>
        /// <param name="logger">Logger</param>
        public DepartmentProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork,
                                  IBusinessUnitApiTranslators businessUnitApiTranslators,
                                  IJsonRestApiLogger logger)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _businessUnitApiTranslators = businessUnitApiTranslators;
        }        

        /// <summary>
        /// Creates a new department of the business unit
        /// </summary>
        /// <param name="model">Model contains department information</param>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <returns><see cref="CreateDepartmentResponseModel"/></returns>
        public async Task<ProviderOperationResult<CreateDepartmentResponseModel>> CreateAsync(CreateDepartmentModel model, Guid businessUnitId)
        {
            var createDepartmentContract = _businessUnitApiTranslators.CreateDepartmentContractTranslator.Translate(model, businessUnitId);

            DepartmentCostCenterContract createDepartmentResponseContract = null;
            try
            {
                createDepartmentResponseContract = await _serviceUnitOfWork.DepartmentCostCenterService.AddDepartmentCostCenterAsync(createDepartmentContract);
            }
            catch (FaultException<TeleenaServiceReferences.DepartmentCostCenterService.TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<CreateDepartmentResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(model), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }
            catch (Exception exc)
            {
                Logger.LogException(LogSeverity.Error, $"{nameof(DepartmentProvider)} CreateDepartment failed", nameof(CreateAsync), exc);
                return ProviderOperationResult<CreateDepartmentResponseModel>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(model), MessageConstants.GeneralErrorMessage);
            }            

            var createDepartmentResponseModel = _businessUnitApiTranslators.DepartmentModelTranslator.Translate(createDepartmentResponseContract);

            return ProviderOperationResult<CreateDepartmentResponseModel>.OkResult(createDepartmentResponseModel);
        }

        /// <summary>
        /// Get departments for the business unit
        /// </summary>
        /// <param name="busineesUnitId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<GetDepartmentsModel>> GetDepartmentAsync(Guid busineesUnitId)
        {
            var input = new GetDepartmentCostCenterByAccountContract { AccountId = busineesUnitId };
          
            var response = await _serviceUnitOfWork.DepartmentCostCenterService.GetDepartmentCostCenterByAccountAsync(input);
            GetDepartmentsModel model = _businessUnitApiTranslators.GetDepartmentsModelTranslator.Translate(response.ToList());

            return ProviderOperationResult<GetDepartmentsModel>.OkResult(model);
        }


        /// <summary>
        /// Update Department
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> UpdateDepartmentAsync(UpdateDepartmentModel request)
        {
            var updateDepartmentContract = _businessUnitApiTranslators.UpdateDepartmentContractTranslator.Translate(request);

            try
            {
                var response = await _serviceUnitOfWork.DepartmentCostCenterService.UpdateDepartmentCostCenterAsync(updateDepartmentContract);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, "request", ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            var responseMessage = ProviderOperationResult<object>.AcceptedResult();
            return responseMessage;
        }
    }
}
