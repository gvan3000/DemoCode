using SplitProvisioning.Base.Data;


namespace OCSServices.Matrixx.Agent.Contracts.Balance
{
    public class QueryGroupBalanceRequest
    {
        public string ExternalId { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
