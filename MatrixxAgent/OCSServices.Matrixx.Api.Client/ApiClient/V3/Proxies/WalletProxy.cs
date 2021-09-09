using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Wallet;
using OCSServices.Matrixx.Api.Client.Internal.Wallet;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IWalletProxy : IDisposable
    {
        Task<SubscriberWalletResponse> QueryWallet(WalletQueryRequest request, Endpoint endpoint);
       
        Task<SubscriberWalletResponse> QueryGroupWallet(GroupWalletQueryRequest request, Endpoint endpoint);
    }
    public class WalletProxy : BaseProxy, IWalletProxy
    {
        
        /// <summary>
        /// Query Wallet with endpoints 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public Task<SubscriberWalletResponse> QueryWallet(WalletQueryRequest request, Endpoint endpoint)
        {
            using (var queryWallet = new WalletQuery(endpoint))
            {
                return queryWallet.Execute(request);
            }
        }


        /// <summary>
        /// QueryGroupWallet overload that accepts endpoint parameter
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SubscriberWalletResponse> QueryGroupWallet(GroupWalletQueryRequest request, Endpoint endpoint)
        {
            using (var queryWallet = new GroupWalletQuery(endpoint))
            {
                return queryWallet.Execute(request);
            }
        }
    }
}
