using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;
using System;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Balance
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class GroupAdjustBalance : BaseApiOperation,
        IPostApiOperation<GroupAdjustBalanceRequest, MatrixxResponse>
    {
        internal GroupAdjustBalance(string baseUrl) : base(baseUrl)
        {
        }

        [Obsolete]
        internal GroupAdjustBalance() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        internal GroupAdjustBalance(Endpoint endpoint) : base(endpoint)
        {
        }

        public Task<MatrixxResponse> Execute(GroupAdjustBalanceRequest payLoad)
        {
            return Post<GroupAdjustBalanceRequest, MatrixxResponse>(payLoad);
        }
    }
}
