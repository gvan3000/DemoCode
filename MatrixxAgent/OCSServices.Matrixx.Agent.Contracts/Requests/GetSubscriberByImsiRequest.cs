namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class GetSubscriberByImsiRequest
    {
        public string Imsi { get; set; }

        public GetSubscriberByImsiRequest()
        {

        }

        public GetSubscriberByImsiRequest(string imsi)
        {
            Imsi = imsi;
        }
    }
}
