using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Threshold;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Threshold : BaseMessageBuilder, IThreshold
    {
        public SubscriberAddThresholdRequest BuildSubscriberAddThresholdRequest(AddThresholdToSubscriberRequest request)
        {
            if (request == null)
                return null;

            return new SubscriberAddThresholdRequest
            {
                BalanceResourceId = request.ResourceId,
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                },
                ThresholdData = new SubscriberThresholdData
                {
                    ThresholdCollection = new ThresholdCollection
                    {
                        ThresholdId = request.ThresholdId,
                        Amount = request.Amount
                    }
                }
            };
        }

        public GroupAddThresholdRequest BuildGroupAddThresholdRequest(AddThresholdToGroupRequest request)
        {
            if (request == null)
                return null;

            return new GroupAddThresholdRequest
            {
                BalanceResourceId = request.ResourceId,
                GroupSearchData = new GroupSearchData()
                {
                    SearchCollection = new SearchCollection()
                    {
                        ExternalId = request.BusinessUnitId.ToString().ToUpper()
                    }
                },
                ThresholdData = new SubscriberThresholdData
                {
                    ThresholdCollection = new ThresholdCollection
                    {
                        ThresholdId = request.ThresholdId,
                        Amount = request.Amount
                    }
                }
            };
        }

        public SubscriberSetThresholdToInfinityRequest BuildSubscriberSetThresholdToInfinityRequest(SetThresholdSubscriberToInfinityRequest request)
        {
            if (request == null)
                return null;

            return new SubscriberSetThresholdToInfinityRequest
            {
                BalanceResourceId = request.ResourceId,
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                },
                ThresholdData = new ThresholdToInfinityData
                {
                    ThresholdCollection = new ThresholdToInfinityCollection
                    {
                        ThresholdId = request.ThresholdId,
                    }
                }
            };
        }
    }
}
