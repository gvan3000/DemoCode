using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Balance
{
    public class QueryBalanceResponse
    {
        public List<BalanceInfo>  BalanceList { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BalanceInfo
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string Unit { get; set; }
        public string TresholdLimit { get; set; }
        public string TemplateId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}