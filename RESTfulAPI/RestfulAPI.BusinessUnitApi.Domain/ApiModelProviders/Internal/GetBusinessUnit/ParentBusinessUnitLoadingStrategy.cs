using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class ParentBusinessUnitLoadingStrategy : BaseBusinessUnitLoadingStrategy
    {
        public override bool CanHandleRequest(GetBusinessUnitRequest request)
        {
            return request != null
                && request.UserBusinessUnitId.GetValueOrDefault() != Guid.Empty;
        }

        protected override async Task<List<AccountContract>> LoadBusinessUnitsImplementationAsync(GetBusinessUnitRequest request,
            ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            var serviceRequest = new AccountRequest { AccountId = request.UserBusinessUnitId };

            // This one returns sharedWallet and subscription data regardless of request. A new procedure with new loader could be created 
            // to return only required data (based on GetAccountWithChildrenByAccountId + join to plans) if needed by performance
            return await serviceUnitOfWork.AccountService
                .GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(serviceRequest);
        }
    }
}
