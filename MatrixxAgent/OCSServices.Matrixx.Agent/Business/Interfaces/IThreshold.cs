using OCSServices.Matrixx.Agent.Contracts.Threshold;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IThreshold
    {
        SubscriberAddThresholdRequest BuildSubscriberAddThresholdRequest(AddThresholdToSubscriberRequest request);
        GroupAddThresholdRequest BuildGroupAddThresholdRequest(AddThresholdToGroupRequest request);
        SubscriberSetThresholdToInfinityRequest BuildSubscriberSetThresholdToInfinityRequest(SetThresholdSubscriberToInfinityRequest request);
    }
}
