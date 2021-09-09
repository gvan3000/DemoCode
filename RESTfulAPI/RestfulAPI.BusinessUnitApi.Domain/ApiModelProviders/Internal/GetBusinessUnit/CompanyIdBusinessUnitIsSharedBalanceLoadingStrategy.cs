using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class CompanyIdBusinessUnitIsSharedBalanceLoadingStrategy : BaseBusinessUnitLoadingStrategy
    {
        public override bool CanHandleRequest(GetBusinessUnitRequest request)
        {
            return request != null 
                && request.UserCompanyId != Guid.Empty 
                && (!string.IsNullOrEmpty(request.FilterHasSharedWallet) || request.FilterHasEndUserSubscripion != null);
        }

        protected override async Task<List<AccountContract>> LoadBusinessUnitsImplementationAsync(GetBusinessUnitRequest request,
            ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            var serviceRequest = new AccountRequest { CompanyId = request.UserCompanyId };
            return await serviceUnitOfWork.AccountService
                .GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(serviceRequest);
        }
    }
}
