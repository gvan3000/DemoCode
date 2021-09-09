using OCSServices.Matrixx.Agent.Contracts.Device;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Sim.Swap;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IDevice
    {
        MultiRequest CreateValidateDeviceListSession(ValidateSessionForDeviceListRequest request);
        MultiRequest CreateAddImsiToSubscriberRequest(AddImsiToSubscriberRequest request);
        MultiRequest CreateAddImsisRequest(SwapSimRequest request, List<string> msisdns);
        MsisdnDeviceQueryParameters GetMsisdnDeviceQueryParameters(string identifier);
        DeviceModifyRequest BuildSwapMsIsdnRequest(DeviceQueryResponse request, SwapMsIsdnRequest swapRequest);
        DeviceModifyRequest BuildUpdateMsisdnListRequest(DeviceQueryResponse request, UpdateMsisdnListRequest updateMsisdnListRequest);
        ImsiDeviceQueryParameters GetImsiDeviceQueryParameters(string identifier);
        MultiRequest CreateDeviceDeleteRequestList(List<string> oldImsis);
        DeviceSessionIdParameters GetDeviceSessionsQueryParameters(string deviceId);

    }
}
