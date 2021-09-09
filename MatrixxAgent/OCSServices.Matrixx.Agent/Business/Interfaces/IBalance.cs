using OCSServices.Matrixx.Agent.Contracts.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IBalance
    {
        IQueryParameters GetQueryBalanceParametersByProductId(Guid productId);
        IQueryParameters GetQueryBalanceParametersByMsisdn(string msisdn);
        SubscriberAdjustBalanceRequest GetAdjustBalanceRequest(FlexTopupRequest request, int resourceId);
        SubscriberAdjustBalanceRequest GetAdjustBalanceRequest(AdjustBalanceForSubscriberRequest request);
        GroupAdjustBalanceRequest GetGroupAdjustBalanceRequest(AdjustBalanceForGroupRequest request);
    }
}
