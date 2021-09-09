using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Agent.Contracts.Msisdn
{
    public class SwapMsIsdnRequest
    {
        public string NewMsIsdn { get; set; }
        public string OldMsIsdn { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
