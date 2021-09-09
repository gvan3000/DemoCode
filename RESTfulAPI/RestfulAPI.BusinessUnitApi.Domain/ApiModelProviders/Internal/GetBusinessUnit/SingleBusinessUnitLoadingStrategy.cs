using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class SingleBusinessUnitLoadingStrategy : BaseBusinessUnitLoadingStrategy
    {
        public override bool CanHandleRequest(GetBusinessUnitRequest request)
        {
            return request != null
                && !request.IncludeChildren
                && request.FilterBusinessUnitId.GetValueOrDefault() != Guid.Empty;
        }

        protected override async Task<List<AccountContract>> LoadBusinessUnitsImplementationAsync(GetBusinessUnitRequest request,
            ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            Guid requestedAccountId = request.FilterBusinessUnitId.GetValueOrDefault();
            return await serviceUnitOfWork.AccountService.GetAccountsExtendedDataByIdAsync(requestedAccountId);
        }
    }
}
