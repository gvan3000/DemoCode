using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Multi;
using SplitProvisioning.Base.Data;
using System;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Multi
{
    [ApiVersion(ApiVersion.v3, HttpMethod.GET)]
    internal class RequestMulti : BaseApiOperation, IPostApiOperation<MultiRequest, MultiResponse>
    {
        [Obsolete]
        internal RequestMulti(string baseUrl) : base(baseUrl)
        {
        }

        [Obsolete]
        internal RequestMulti() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        internal RequestMulti(Endpoint endpoint) : base(endpoint)
        {

        }

        public async Task<MultiResponse> Execute(MultiRequest payLoad)
        {
            var count = 0;
            MultiResponse result = null;
            while (count < 3)
            {
                result = await Post<MultiRequest, MultiResponse>(payLoad).ConfigureAwait(false);
                if (result.Code != 22)
                {
                    return result;
                }

                await Task.Delay(3000);
                count++;
            }
            return result;
        }
    }
}
