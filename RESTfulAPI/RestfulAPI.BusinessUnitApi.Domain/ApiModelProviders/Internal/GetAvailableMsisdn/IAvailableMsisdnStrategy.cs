using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.MobileService;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public interface IAvailableMsisdnStrategy
    {
        bool CanHandleRequest(AvailableMsisdnProviderRequest request);
        Task<MsisdnContract[]> GetAvailableMsisdnsAsync(AvailableMsisdnProviderRequest request, ITeleenaServiceUnitOfWork serviceUnitOfWork);
    }
}
