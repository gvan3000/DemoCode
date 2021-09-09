using System;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class ModifySubscriberRequest
    {
        public Guid? ProductId { get; set; }
        public int Status { get; set; }
    }
}
