using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class GroupPurchaseOffer : BaseApiOperation, IPostApiOperation<PurchaseGroupOfferRequest, AddOfferToSubscriberResponse>
    {
       
        internal GroupPurchaseOffer(Endpoint endpoint) : base(endpoint)
        {
        }
        public async Task<AddOfferToSubscriberResponse> Execute(PurchaseGroupOfferRequest payLoad)
        {
            return await Post<PurchaseGroupOfferRequest, AddOfferToSubscriberResponse>(payLoad);
        }
    }
}
