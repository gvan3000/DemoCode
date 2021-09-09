using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber;
using OCSServices.Matrixx.Api.Client.Internal.Subscriber;
using SplitProvisioning.Base.Data;
using System;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface ISubscriberProxy : IDisposable
    {

        [Obsolete("This method is deprecated and new overload that is accepting Endpoint as a parrem is used.")]
        Task<SubscriberQueryResponse> SubscriberQuery(IQueryParameters queryParameters);
        Task<SubscriberQueryResponse> SubscriberQuery(IQueryParameters queryParameters, Endpoint endpoint);
        Task<MatrixxResponse> SubscriberModify(ModifySubscriberRequest request, Endpoint endpoint);
    }
    public class SubscriberProxy : BaseProxy, ISubscriberProxy
    {

        public Task<SubscriberQueryResponse> SubscriberQuery(IQueryParameters queryParameters)
        {
            using (var subscriberQuery = new SubscriberQuery())
            {
                return subscriberQuery.Execute(queryParameters);
            }
        }
        public Task<SubscriberQueryResponse> SubscriberQuery(IQueryParameters queryParameters, Endpoint endpoint)
        {
            using (var subscriberQuery = new SubscriberQuery(endpoint))
            {
                return subscriberQuery.Execute(queryParameters);
            }
        }


        public Task<MatrixxResponse> SubscriberModify(ModifySubscriberRequest request, Endpoint endpoint)
        {
            using (var subscriberModify = new SubscriberModify(endpoint))
            {
                return subscriberModify.Execute(request);
            }
        }


    }
}
