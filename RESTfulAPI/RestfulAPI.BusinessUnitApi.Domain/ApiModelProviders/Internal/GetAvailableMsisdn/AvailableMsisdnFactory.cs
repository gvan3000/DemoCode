using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.TeleenaServiceReferences;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public class AvailableMsisdnFactory : IAvailableMsisdnFactory
    {
        private readonly List<IAvailableMsisdnStrategy> strategies;
        private readonly ISysCodeConstants _sysCodeConstants;

        public AvailableMsisdnFactory(ISysCodeConstants sysCodeConstants)
        {
            _sysCodeConstants = sysCodeConstants;

            strategies = new List<IAvailableMsisdnStrategy>();

            strategies.Add(new NoStatusParamStrategy(_sysCodeConstants));
            strategies.Add(new WithStatusParamStrategy(_sysCodeConstants));
        }

        public IAvailableMsisdnStrategy GetStrategy(AvailableMsisdnProviderRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            return strategies.FirstOrDefault(x => x.CanHandleRequest(request));
        }
    }
}
