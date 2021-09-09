using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Multi;
using OCSServices.Matrixx.Api.Client.Internal.Multi;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IMultiProxy : IDisposable
    {
        //Didn't not remove Obselete construct because of Handler using a method in agent which uses this constructor
        [Obsolete]
        Task<MultiResponse> RequestMulti(MultiRequest request);
        Task<MultiResponse> RequestMulti(MultiRequest request, Endpoint endpoint);
    }

    public class MultiProxy : BaseProxy, IMultiProxy
    {
        public Task<MultiResponse> RequestMulti(MultiRequest request)
        {
            using (var requestMulti = new RequestMulti())
            {
                return requestMulti.Execute(request);
            }
        }

        public Task<MultiResponse> RequestMulti(MultiRequest request, Endpoint endpoint)
        {
            using (var requestMulti = new RequestMulti(endpoint))
            {
                return requestMulti.Execute(request);
            }
        }
    }
}
