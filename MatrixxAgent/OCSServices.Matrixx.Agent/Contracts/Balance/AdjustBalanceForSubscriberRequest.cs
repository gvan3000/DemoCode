using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Balance
{
    public class AdjustBalanceForSubscriberRequest
    {
        public Guid ProductId { get; set; }
        public int BalanceResourceId { get; set; }
        public int AdjustType { get; set; }
        public decimal Amount { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
        public string Reason { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
