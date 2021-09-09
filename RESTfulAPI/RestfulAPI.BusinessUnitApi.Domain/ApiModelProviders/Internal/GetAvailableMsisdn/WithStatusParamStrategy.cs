using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.MobileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public class WithStatusParamStrategy : IAvailableMsisdnStrategy
    {
        private readonly ISysCodeConstants _sysCodeConstants;

        public WithStatusParamStrategy(ISysCodeConstants sysCodeConstants)
        {
            _sysCodeConstants = sysCodeConstants;
        }

        public bool CanHandleRequest(AvailableMsisdnProviderRequest request)
        {
            return request != null && (!string.IsNullOrEmpty(request.MsisdnStatus) || !string.IsNullOrEmpty(request.QueryMsisdnStatus));
        }

        public async Task<MsisdnContract[]> GetAvailableMsisdnsAsync(AvailableMsisdnProviderRequest request, ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            var result = new List<MsisdnContract>();

            if (string.IsNullOrEmpty(request.MsisdnStatus) && string.IsNullOrEmpty(request.QueryMsisdnStatus))
            {
                throw new ArgumentException($"{nameof(WithStatusParamStrategy)} needs status parameter supplied.");
            }

            var statusString = !string.IsNullOrWhiteSpace(request.MsisdnStatus) ? request.MsisdnStatus.ToUpper() : request.QueryMsisdnStatus.ToUpper();
            var selectedStatusId = _sysCodeConstants.MobileStatuses[statusString];

            var serviceResponse = await serviceUnitOfWork.MobileService.GetAvailableMsisdnsWithStatusAsync(
                new GetAvailableMsisdnsWithStatusContract()
                {
                    AccountId = request.BusinessUnitId.GetValueOrDefault(),
                    CountryCode = request.CountryCode,
                    NumberStatusId = selectedStatusId,
                    PerPage = request.PerPage.GetValueOrDefault(),
                    Page = request.Page.GetValueOrDefault()
                });

            if (serviceResponse != null && serviceResponse.Any())
               result.AddRange(serviceResponse);

            return result.ToArray();
        }
    }
}
