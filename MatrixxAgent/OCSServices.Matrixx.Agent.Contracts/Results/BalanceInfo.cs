using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class BalanceInfo
    {
        public string ResourceId { get; set; }

        public string Amount { get; set; }

        public string ReservedAmount { get; set; }

        public string CreditLimit { get; set; }

        public string ThresholdLimit { get; set; }

        public string AvailableAmount { get; set; }

        public string TemplateId { get; set; }

        public string Name { get; set; }

        public string ClassId { get; set; }

        public string ClassName { get; set; }

        public string Category { get; set; }

        public bool IsPrepaid { get; set; }

        public bool IsPeriodic { get; set; }

        public bool IsAggregate { get; set; }

        public bool IsVirtual { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string QuantityUnit { get; set; }
    }
}
