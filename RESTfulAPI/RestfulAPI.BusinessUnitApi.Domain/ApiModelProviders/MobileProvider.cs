using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <summary>
    /// Mobile provider
    /// </summary>
    public class MobileProvider : IMobileProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _businessUnitApiTranslators;
        private readonly ISysCodeConstants _sysCodeConstants;
        private readonly IAvailableMsisdnFactory _availableMsisdnFactory;

        /// <summary>
        /// Initialize Mobile provider
        /// </summary>
        /// <param name="serviceUnitOfWork">wcf service unit of work</param>
        /// <param name="businessUnitApiTranslators">business unit translator</param>
        /// <param name="sysCodeConstants">sysCodeConstants</param>
        /// <param name="availableMsisdnFactory">get available msisdns strategy factory</param>
        public MobileProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators businessUnitApiTranslators,
                              ISysCodeConstants sysCodeConstants, IAvailableMsisdnFactory availableMsisdnFactory)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _businessUnitApiTranslators = businessUnitApiTranslators;
            _sysCodeConstants = sysCodeConstants;
            _availableMsisdnFactory = availableMsisdnFactory;
        }

        /// <summary>
        /// Get avaiable msisdns
        /// </summary>
        /// <param name="request">Contains </param>
        /// <returns><see cref="AvailableMSISDNResponseModel"/></returns>
        public async Task<ProviderOperationResult<AvailableMSISDNResponseModel>> GetAvailableMsisdnsAsync(
            AvailableMsisdnProviderRequest request)
        {
            if (request.MsisdnStatus == string.Empty)
            {
                return ProviderOperationResult<AvailableMSISDNResponseModel>.InvalidInput(
                    nameof(GetAvailableMsisdnsAsync), $"Status should not be empty string.");
            }

            if (!string.IsNullOrEmpty(request.MsisdnStatus) &&
                !request.MsisdnStatus.Equals(Constants.MobileStatusConstants.Available, StringComparison.InvariantCultureIgnoreCase))
            {
                return ProviderOperationResult<AvailableMSISDNResponseModel>.InvalidInput(
                    nameof(GetAvailableMsisdnsAsync), $"Only valid status parameter is {Constants.MobileStatusConstants.Available}.");
            }

            if (request.CountryCode != null && (request.CountryCode.Length != 2 || !request.CountryCode.All(char.IsLetter)))
            {
                return ProviderOperationResult<AvailableMSISDNResponseModel>.InvalidInput(
                    nameof(GetAvailableMsisdnsAsync), $"Country parameter is not a valid ISO2Code");
            }

            if (!request.Page.HasValue || request.Page.Value < 1)
            {
                request.Page = 1;
            }

            if (!request.PerPage.HasValue || request.PerPage.Value < 1)
            {
                request.PerPage = 50;
            }
            else if (request.PerPage.Value > 250)
            {
                request.PerPage = 250;
            }

            var strategy = _availableMsisdnFactory.GetStrategy(request);
            if (strategy == null)
                return ProviderOperationResult<AvailableMSISDNResponseModel>.InvalidInput(nameof(GetAvailableMsisdnsAsync), $"Not able to load appropriate strategy for available msisdns.");

            var serviceResponse = await strategy.GetAvailableMsisdnsAsync(request, _serviceUnitOfWork);

            var translatedResponse = _businessUnitApiTranslators.MsisdnContractTranslator.Translate(serviceResponse);

            if (translatedResponse == null)
                return ProviderOperationResult<AvailableMSISDNResponseModel>.NotFoundResult("Available msisdns can not be found.");

            return ProviderOperationResult<AvailableMSISDNResponseModel>.OkResult(translatedResponse);
        }
    }
}
