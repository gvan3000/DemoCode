using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class GroupModify : BaseApiOperation, IPostApiOperation<ModifyGroupRequest, MatrixxResponse>
    {
        internal GroupModify(string baseUrlKey) : base(baseUrlKey)
        {
        }

        internal GroupModify() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public async Task<MatrixxResponse> Execute(ModifyGroupRequest payLoad)
        {
            return await Post<ModifyGroupRequest, MatrixxResponse>(payLoad);
        }
    }
}
