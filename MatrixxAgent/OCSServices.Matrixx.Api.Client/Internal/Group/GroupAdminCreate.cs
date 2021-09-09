using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;
using System;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class GroupAdminCreate : BaseApiOperation, IPostApiOperation<CreateSubscriberRequest, MatrixxResponse>
    {
       
        internal GroupAdminCreate(Endpoint endpoint) : base(endpoint)
        {
        }

        public async Task<MatrixxResponse> Execute(CreateSubscriberRequest payLoad)
        {
            return await Post<CreateSubscriberRequest, MatrixxResponse>(payLoad);
        }
    }
}

