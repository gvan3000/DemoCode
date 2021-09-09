using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.DELETE)]
    internal class GroupCancelOffer : BaseApiOperation, IPostApiOperation<CancelOfferForGroupRequest, MatrixxResponse>
    {
        internal GroupCancelOffer(string baseUrl) : base(baseUrl)
        {
        }

        internal GroupCancelOffer() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public Task<MatrixxResponse> Execute(CancelOfferForGroupRequest payLoad)
        {
            return Delete<MatrixxResponse>(payLoad);
        }

    }
}
