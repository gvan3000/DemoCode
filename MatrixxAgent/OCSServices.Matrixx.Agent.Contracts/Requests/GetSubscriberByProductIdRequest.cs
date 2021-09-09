using System;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class GetSubscriberByProductIdRequest
    {
        public Guid? ProductId { get; set; }
    }
}
