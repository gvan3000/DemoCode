using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public interface IBusinessUnitLoadingStrategy
    {
        bool CanHandleRequest(GetBusinessUnitRequest request);
        Task<List<AccountContract>> LoadBusinessUnitsAsync(GetBusinessUnitRequest request, ITeleenaServiceUnitOfWork serviceUnitOfWork);
    }
}