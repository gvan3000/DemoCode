using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class GroupModifyOffer : BaseApiOperation, IPostApiOperation<ModifyOfferForGroupRequest, MatrixxResponse>
    {
        internal GroupModifyOffer(Endpoint endpoint) : base(endpoint)
        {
        }

        public async Task<MatrixxResponse> Execute(ModifyOfferForGroupRequest payLoad)
        {
            return await Post<ModifyOfferForGroupRequest, MatrixxResponse>(payLoad);
        }
    }
}
