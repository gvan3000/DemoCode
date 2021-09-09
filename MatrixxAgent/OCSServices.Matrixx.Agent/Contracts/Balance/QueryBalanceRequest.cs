using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Balance
{
    public class QueryBalanceRequest
    {
        public Guid? ProductId { get; set; }
        public string Msisdn { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
