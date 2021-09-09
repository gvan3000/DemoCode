using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Wallet
{
    public class QueryWalletResponse
    {
        public List<WalletInfo> SimpleBalanceList { get; set; }
        public List<WalletInfo> PeriodicBalanceList { get; set; }
        public List<WalletInfo> SimpleAggregatedBalanceList { get; set; }
        public List<WalletInfo> PeriodicAggregateBalanceList { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<BillCycleInfo> MatrixxBillCycleList { get; set; }
    }

    public class WalletInfo
    {
        public int ResourceId { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string ReservedAmount { get; set; }
        public string Unit { get; set; }
        public string TresholdLimit { get; set; }
        public string TemplateId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class BillCycleInfo
    {
        public int BillingCycleId { get; set; }
        public int Period { get; set; }
        public int PeriodInterval { get; set; }
        public int DateOffset { get; set; }
        public DateTime? CurrentPeriodStartTime { get; set; }
        public DateTime? CurrentPeriodEndTime { get; set; }
        public int CurrentPeriodDuration { get; set; }
        public int? CurrentPeriodOffset { get; set; }
    }
}