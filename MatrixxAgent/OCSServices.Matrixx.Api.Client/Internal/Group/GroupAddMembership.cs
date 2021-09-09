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
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class GroupAddMembership : BaseApiOperation, IPostApiOperation<AddGroupMembershipRequest, MatrixxResponse>
    {
        internal GroupAddMembership(string baseUrl) : base(baseUrl)
        {
        }

        internal GroupAddMembership() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public Task<MatrixxResponse> Execute(AddGroupMembershipRequest payLoad)
        {
            return Post<AddGroupMembershipRequest, MatrixxResponse>(payLoad);
        }
    }
}
