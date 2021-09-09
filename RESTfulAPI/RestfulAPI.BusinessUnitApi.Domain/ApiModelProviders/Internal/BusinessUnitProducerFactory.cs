using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal
{
    public class BusinessUnitProducerFactory : IBusinessUnitProducerFactory
    {
        private readonly List<IBusinessUnitLoadingStrategy> _loadingStrategies;
        private readonly List<IBusinessUnitFilter> _filters;

        public BusinessUnitProducerFactory()
        {
            _loadingStrategies = new List<IBusinessUnitLoadingStrategy>();
            // order of these is important, they will be used in order provided
            _loadingStrategies.Add(new SingleBusinessUnitLoadingStrategy());
            _loadingStrategies.Add(new ParentBusinessUnitLoadingStrategy());
            _loadingStrategies.Add(new CompanyIdBusinessUnitIsSharedBalanceLoadingStrategy());
            _loadingStrategies.Add(new CompanyIdBusinessUnitLoadingStrategy());

            _filters = new List<IBusinessUnitFilter>()
            {
                new BusinessUnitHasEndUserSubscriptionFilter(),
                new NoChildrenNameFilter(),
                new NoChildrenBusinessUnitIdFilter(),
                new BusinessUnitIdFilter(),
                new BusinessUnitCustomerIdFilter(),
                new BusinessUnitHasSharedWalletFilter(),
                new BusinessUnitCompanyIdFilter()
            };
        }

        public IBusinessUnitLoadingStrategy GetLoader(GetBusinessUnitRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            return _loadingStrategies.FirstOrDefault(strategy => strategy.CanHandleRequest(request));
        }

        public IBusinessUnitFilter GetFilterForRequest(GetBusinessUnitRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return _filters.FirstOrDefault(f => f.CanApplyFilter(request));
        }
    }
}
