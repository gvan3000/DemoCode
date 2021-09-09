using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class GroupRemove : BaseApiOperation, IPostApiOperation<DeleteGroupRequest, MatrixxResponse>
    {
        internal GroupRemove(Endpoint endpoint) : base(endpoint)
        {
        }

        public async Task<MatrixxResponse> Execute(DeleteGroupRequest payLoad)
        {
            return await Post<DeleteGroupRequest, MatrixxResponse>(payLoad);
        }
    }
}