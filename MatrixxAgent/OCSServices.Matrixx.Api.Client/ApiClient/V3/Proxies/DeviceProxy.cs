using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using OCSServices.Matrixx.Api.Client.Internal.Device;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IDeviceProxy : IDisposable
    {
        Task<CreateObjectResponse> DeviceCreate(CreateDeviceRequest request);
        Task<DeviceQueryResponse> DeviceQuery(IQueryParameters queryParameters, Endpoint endpoint);
        Task<MatrixxResponse> DeviceModify(DeviceModifyRequest request, Endpoint endpoint);
        Task<ResponseDeviceSession> DeviceSessionQuery(IQueryParameters queryParameters, Endpoint endpont);
    }

    public class DeviceProxy : BaseProxy, IDeviceProxy
    {
        public Task<CreateObjectResponse> DeviceCreate(CreateDeviceRequest request)
        {
            using (var deviceCreate = new DeviceCreate())
            {
                return deviceCreate.Execute(request);
            }
        }       
      
        public Task<MatrixxResponse> DeviceModify(DeviceModifyRequest request, Endpoint endpoint)
        {
            using (var deviceModify = new DeviceModify(endpoint))
            {
                return deviceModify.Execute(request);
            }
        }

        public Task<DeviceQueryResponse> DeviceQuery(IQueryParameters queryParameters, Endpoint endpoint)
        {
            using (var query = new DeviceQuery(endpoint))
            {
                return query.Execute(queryParameters);
            }
        }

        public Task<ResponseDeviceSession> DeviceSessionQuery(IQueryParameters queryParameters, Endpoint endpont)
        {
            using (var query = new DeviceSessionQuery(endpont))
            {
                return query.Execute(queryParameters);
            }
        }
    }
}
