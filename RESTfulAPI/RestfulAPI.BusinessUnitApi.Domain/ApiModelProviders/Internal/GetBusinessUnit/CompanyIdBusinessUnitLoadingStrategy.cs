using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class CompanyIdBusinessUnitLoadingStrategy : BaseBusinessUnitLoadingStrategy
    {
        public override bool CanHandleRequest(GetBusinessUnitRequest request)
        {
            return request != null && request.UserCompanyId != Guid.Empty;
        }

        protected override async Task<List<AccountContract>> LoadBusinessUnitsImplementationAsync(GetBusinessUnitRequest request, 
            ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            var serviceRequest = new GetAccountsByCompanyContract()
            {
                CompanyId = request.UserCompanyId
            };
            return await serviceUnitOfWork.AccountService.GetAccountsByCompanyAsync(serviceRequest);
        }
    }
}
