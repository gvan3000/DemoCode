using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Internal.Balance;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IBalanceProxy : IDisposable
    {
        Task<MatrixxResponse> SubscriberAdjustBalance(SubscriberAdjustBalanceRequest request, Endpoint endpoint);
        Task<MatrixxResponse> SubscriberAddThreshold(SubscriberAddThresholdRequest request, Endpoint endpoint);
        Task<MatrixxResponse> GroupAddThreshold(GroupAddThresholdRequest request, Endpoint endpoint);    
        Task<MatrixxResponse> GroupAdjustBalance(GroupAdjustBalanceRequest request, Endpoint endpoint);
        Task<MatrixxResponse> SubscriberSetThresholdToInfinity(SubscriberSetThresholdToInfinityRequest request, Endpoint endpoint);
    }

    public class BalanceProxy : BaseProxy, IBalanceProxy
    {
        public Task<MatrixxResponse> GroupAddThreshold(GroupAddThresholdRequest request, Endpoint endpoint)
        {
            using (var groupAddThreshold = new GroupAddThreshold(endpoint))
            {
                return groupAddThreshold.Execute(request);
            }
        }
      
       
        public Task<MatrixxResponse> SubscriberSetThresholdToInfinity(SubscriberSetThresholdToInfinityRequest request, Endpoint endpoint)
        {
            using (var subscriberAddThreshold = new SubscriberAddThreshold(endpoint))
            {
                return subscriberAddThreshold.Execute(request);
            }
        }

        public Task<MatrixxResponse> SubscriberAdjustBalance(SubscriberAdjustBalanceRequest request, Endpoint endpoint)
        {
            using (var subscriberAdjustBalance = new SubscriberAdjustBalance(endpoint))
            {
                return subscriberAdjustBalance.Execute(request);
            }
        }

        public Task<MatrixxResponse> GroupAdjustBalance(GroupAdjustBalanceRequest request, Endpoint endpoint)
        {
            using (var groupAdjustBalance = new GroupAdjustBalance(endpoint))
            {
                return groupAdjustBalance.Execute(request);
            }
        }

        public Task<MatrixxResponse> SubscriberAddThreshold(SubscriberAddThresholdRequest request, Endpoint endpoint)
        {
            using (var subscriberAddThreshold = new SubscriberAddThreshold(endpoint))
            {
                return subscriberAddThreshold.Execute(request);
            }
        }

       


    }
}
