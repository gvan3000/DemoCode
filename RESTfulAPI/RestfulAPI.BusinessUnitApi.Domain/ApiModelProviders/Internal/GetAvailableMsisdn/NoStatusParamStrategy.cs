using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.MobileService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public class NoStatusParamStrategy : IAvailableMsisdnStrategy

    {
        private readonly ISysCodeConstants _sysCodeConstants;

        public NoStatusParamStrategy(ISysCodeConstants sysCodeConstants)
        {
            _sysCodeConstants = sysCodeConstants;
        }

        public bool CanHandleRequest(AvailableMsisdnProviderRequest request)
        {
            return request != null && string.IsNullOrEmpty(request.MsisdnStatus) && string.IsNullOrEmpty(request.QueryMsisdnStatus);
        }

        public async Task<MsisdnContract[]> GetAvailableMsisdnsAsync(AvailableMsisdnProviderRequest request, ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            var result = new List<MsisdnContract>();

            Guid availableStatusId = _sysCodeConstants.MobileStatuses[Constants.MobileStatusConstants.Available];

            if (request?.BusinessUnitId == null)
                return result.ToArray();

            var allAccountMsisdnsResponse =
                await serviceUnitOfWork.MobileService.GetAvailableMsisdnsNoStatusAsync(
                    new GetAvailableMsisdnsNoStatusContract()
                    {
                        AccountId = request.BusinessUnitId.GetValueOrDefault(),
                        NumberStatusId = availableStatusId,
                        CountryCode = request.CountryCode,
                        PerPage = request.PerPage.GetValueOrDefault(),
                        Page = request.Page.GetValueOrDefault()
                    }
                   );

            if (allAccountMsisdnsResponse != null)
            {
                result.AddRange(allAccountMsisdnsResponse);
            }


            return result.ToArray();
        }
    }
}
