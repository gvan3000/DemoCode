using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.SimServiceV2;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sim = RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels.Sim;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class SimProvider : ISimProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _businessUnitApiTranslators;

        public SimProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators businessUnitApiTranslators)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _businessUnitApiTranslators = businessUnitApiTranslators;
        }

        public async Task<ProviderOperationResult<AvailableSIMResponseModel>> GetAvailableSIMsAsync(Guid businessUnitId, string status)
        {
            string[] serviceResponse = new string[] { };
            var simStatusId = _businessUnitApiTranslators.SimStatusTranslator.Translate(status);
            if (simStatusId.Equals(default(Guid)))
                return ProviderOperationResult<AvailableSIMResponseModel>.InvalidInput(nameof(status), $"{status} status does not exist.");

            var contract = new GetAvailableSimsContract
            {
                SimStatusId = simStatusId,
                AccountId = businessUnitId
            };

            try
            {
                serviceResponse = await _serviceUnitOfWork.SimService.GetAvailableSimsAsync(contract);
            }
            catch (Exception ex)
            {
                if (IsCompanyFeatureEnabledException(ex))
                {
                    return ProviderOperationResult<AvailableSIMResponseModel>.InvalidInput(nameof(businessUnitId),
                        GetCompanyFeatureExceptionMessageOnly(ex));
                }
            }

            if (serviceResponse == null || serviceResponse.Count() == 0)
                return ProviderOperationResult<AvailableSIMResponseModel>.NotFoundResult("No available sims were found.");

            AvailableSIMResponseModel retVal = new AvailableSIMResponseModel()
            {
                Iccid = serviceResponse.ToList()
            };

            return ProviderOperationResult<AvailableSIMResponseModel>.OkResult(retVal);
        }

        public async Task<ProviderOperationResult<AvailableSimResponseV2Model>> GetAvailableSIMsAsync(AvailableSimProviderRequest request)
        {
            bool isSimStatusIsNullOrWhitespace = string.IsNullOrWhiteSpace(request.Status);
            AvailableSimsPaginatedResponseContract serviceResponse = new AvailableSimsPaginatedResponseContract();
            Guid simStatusId = isSimStatusIsNullOrWhitespace
                ? default(Guid)
                : _businessUnitApiTranslators.SimStatusTranslator.Translate(request.Status);
            var perPage = request.PerPage.GetValueOrDefault(50);
            var page = request.Page.GetValueOrDefault(1);

            var simRequestValidator = new SimRequestValidator(request.Status, simStatusId, isSimStatusIsNullOrWhitespace, perPage, page);
            simRequestValidator.Validate();

            if (simRequestValidator.InvalidInputResult != null)
            {
                return simRequestValidator.InvalidInputResult;
            }

            try
            {
                serviceResponse = await _serviceUnitOfWork.SimService.GetAvailableSimsPaginatedAsync(
                    BuildAvailableSimsPaginatedContract(request, simStatusId, perPage, page));
            }
            catch (Exception ex)
            {
                if (IsCompanyFeatureEnabledException(ex))
                {
                    return ProviderOperationResult<AvailableSimResponseV2Model>.InvalidInput(nameof(request),
                        GetCompanyFeatureExceptionMessageOnly(ex));
                }
            }

            if (serviceResponse?.Sims == null || serviceResponse.Sims.Length < 1)
                return ProviderOperationResult<AvailableSimResponseV2Model>.NotFoundResult("No sims were found for your request.");

            return ProviderOperationResult<AvailableSimResponseV2Model>.OkResult(BuildAvailableSimModelResponse(serviceResponse));
        }

        /// <summary>
        /// Will Check if the Given Exception is of CompanyFeatureEnabled
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsCompanyFeatureEnabledException(Exception ex)
        {
            string first = ex.Message.Split('-').FirstOrDefault();

            return first != null && first.Contains("CompanyFeature");
        }

        /// <summary>
        /// Will remove cause appended by TeleenaException
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string GetCompanyFeatureExceptionMessageOnly(Exception ex)
        {
            if (IsCompanyFeatureEnabledException(ex))
            {
                var brokenMessage = ex.Message.Split('-');
                string newMessage = string.Empty;
                for (var index = 1; index < brokenMessage.Length; index++)
                {
                    var word = brokenMessage[index];
                    newMessage += word + "-";
                }

                //remove extra char '-' from last
                return newMessage.Trim().Substring(0, newMessage.Length - 2);
            }
            return ex.Message;
        }
        private static GetAvailableSimsPaginatedContract BuildAvailableSimsPaginatedContract(AvailableSimProviderRequest request, Guid simStatusId, int perPage, int page)
        {
            return new GetAvailableSimsPaginatedContract
            {
                AccountId = request.AccountId,
                SimStatusId = simStatusId,
                Take = perPage,
                Skip = perPage * (page - 1)
            };
        }

        private AvailableSimResponseV2Model BuildAvailableSimModelResponse(AvailableSimsPaginatedResponseContract serviceResponse)
        {
            return new AvailableSimResponseV2Model
            {
                Sims = serviceResponse.Sims.Select(sim => new Sim
                {
                    Status = _businessUnitApiTranslators.SimStatusGuidToStringTranslator.Translate(sim.Status).Status,
                    Iccid = sim.Iccid,
                    EID = sim.Eiccid
                }),
                Paging = new Paging { TotalResults = serviceResponse.TotalResults }
            };
        }

        private class SimRequestValidator
        {
            public ProviderOperationResult<AvailableSimResponseV2Model> InvalidInputResult { get; private set; }

            private readonly string simStatus;
            private readonly Guid simStatusId;
            private readonly bool isSimStatusIsNullOrWhitespace;
            private readonly int perPage;
            private readonly int page;

            public SimRequestValidator(string simStatus, Guid simStatusId, bool isSimStatusIdNullOrWhitespace, int perPage, int page)
            {
                this.simStatus = simStatus;
                this.simStatusId = simStatusId;
                this.isSimStatusIsNullOrWhitespace = isSimStatusIdNullOrWhitespace;
                this.perPage = perPage;
                this.page = page;

                InvalidInputResult = null;
            }

            public void Validate()
            {
                this.ValidateGuid()
                    .ValidatePerPage()
                    .ValidatePage();
            }

            private SimRequestValidator ValidateGuid()
            {
                if (InvalidInputResult == null && !isSimStatusIsNullOrWhitespace && simStatusId.Equals(default(Guid)))
                {
                    InvalidInputResult = ProviderOperationResult<AvailableSimResponseV2Model>.InvalidInput(
                        nameof(simStatus), $"'{simStatus}' status does not exist.");
                }
                return this;
            }

            private SimRequestValidator ValidatePerPage()
            {
                if (InvalidInputResult == null && (perPage < 1 || perPage > 250))
                {
                    InvalidInputResult = ProviderOperationResult<AvailableSimResponseV2Model>.InvalidInput(
                        nameof(perPage), $"Available range for {nameof(perPage)} param: 1-250");
                }
                return this;
            }

            private SimRequestValidator ValidatePage()
            {
                if (InvalidInputResult == null && page < 1)
                {
                    InvalidInputResult = ProviderOperationResult<AvailableSimResponseV2Model>.InvalidInput(
                        nameof(page), $"{nameof(page)} should be greater than 0");
                }
                return this;
            }
        }
    }
}
