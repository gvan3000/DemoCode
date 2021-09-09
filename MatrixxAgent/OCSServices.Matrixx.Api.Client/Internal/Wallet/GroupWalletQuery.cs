using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using System;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Wallet
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class GroupWalletQuery : BaseApiOperation, IPostApiOperation<GroupWalletQueryRequest, SubscriberWalletResponse>
    {

        /// <summary>
        /// GroupWalletQuery constructor with endpoints initialization
        /// </summary>
        /// <param name="endpoint"></param>
        internal GroupWalletQuery(Endpoint endpoint) : base(endpoint)
        {

        }

        public Task<SubscriberWalletResponse> Execute(GroupWalletQueryRequest payLoad)
        {
            return Post<GroupWalletQueryRequest, SubscriberWalletResponse>(payLoad);
        }
    }
}
