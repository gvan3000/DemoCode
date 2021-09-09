using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public abstract class BaseBusinessUnitLoadingStrategy : IBusinessUnitLoadingStrategy
    {
        public abstract bool CanHandleRequest(GetBusinessUnitRequest request);

        public async Task<List<AccountContract>> LoadBusinessUnitsAsync(GetBusinessUnitRequest request,
            ITeleenaServiceUnitOfWork serviceUnitOfWork)
        {
            ValidateRequest(request);

            List<AccountContract> accounts = await LoadBusinessUnitsImplementationAsync(request, serviceUnitOfWork);
            return accounts.Distinct(new AccountEqualityComparer()).ToList();
        }

        protected virtual void ValidateRequest(GetBusinessUnitRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

        protected abstract Task<List<AccountContract>> LoadBusinessUnitsImplementationAsync(GetBusinessUnitRequest request, 
            ITeleenaServiceUnitOfWork serviceUnitOfWork);
    }
}