using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Api.Client.Internal.Wallet
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class WalletQuery : BaseApiOperation, IPostApiOperation<WalletQueryRequest, SubscriberWalletResponse>
    {

        /// <summary>
        /// WalletQuery constructor with endpoints initialization
        /// </summary>
        /// <param name="endpoint"></param>
        internal WalletQuery(Endpoint endpoint) : base(endpoint)
        {

        }

        public Task<SubscriberWalletResponse> Execute(WalletQueryRequest payLoad)
        {
            return Post<WalletQueryRequest, SubscriberWalletResponse>(payLoad);
        }
    }
}