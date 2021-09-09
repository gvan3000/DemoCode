using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    /// <summary>
    /// BusinessUnit CompanyId Filter
    /// </summary>
    public class BusinessUnitCompanyIdFilter : IBusinessUnitFilter
    {
        /// <summary>
        /// Decision to use filter
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                && request.UserCompanyId != Guid.Empty;
        }

        /// <summary>
        /// Filter BusinessUnits by requested ComapnyId
        /// </summary>
        /// <param name="input">List of all business units</param>
        /// <param name="request">Request contains searched option</param>
        /// <returns></returns>
        public List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            var filteredInput = input.Where(x => x.CompanyId == request.UserCompanyId).ToList();
            return filteredInput;
        }
    }
}
