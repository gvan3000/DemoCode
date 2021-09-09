namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class AddImsiToSubscriberRequest
    {
        public string SubscriberExternalId { get; set; }
        public string NewImsi { get; set; }
    }
}
