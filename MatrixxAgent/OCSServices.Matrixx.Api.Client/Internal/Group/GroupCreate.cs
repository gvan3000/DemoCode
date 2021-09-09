using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    internal class GroupCreate : BaseApiOperation, IPostApiOperation<CreateGroupRequest, CreateObjectResponse>
    {
        internal GroupCreate(string baseUrlKey) : base(baseUrlKey)
        {
        }
        internal GroupCreate() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public async Task<CreateObjectResponse> Execute(CreateGroupRequest payLoad)
        {
            return await Post<CreateGroupRequest, CreateObjectResponse>(payLoad);
        }
    }
}
