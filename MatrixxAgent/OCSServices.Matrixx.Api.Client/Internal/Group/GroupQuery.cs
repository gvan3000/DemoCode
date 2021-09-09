using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Group;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Group
{
    [ApiVersion(ApiVersion.v3, HttpMethod.GET)]
    internal class GroupQuery : BaseApiOperation, IGetApiOperation<GroupQueryResponse>
    {
       
        internal GroupQuery(Endpoint endpoint) : base(endpoint)
        {
        }

        public async Task<GroupQueryResponse> Execute(IQueryParameters parameters)
        {
            return await Get<GroupQueryResponse>(parameters);
        }
    }
}
