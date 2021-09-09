using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Balance
{
    public class BalanceInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "ResourceId")]
        public int ResourceId { get; set; }

        [MatrixxContractMember(Name = "Amount")]
        public string Amount { get; set; }

        [MatrixxContractMember(Name = "ReservedAmount")]
        public string ReservedAmount { get; set; }

        [MatrixxContractMember(Name = "CreditLimit")]
        public string CreditLimit { get; set; }

        [MatrixxContractMember(Name = "ThresholdLimit")]
        public string ThresholdLimit { get; set; }

        [MatrixxContractMember(Name = "AvailableAmount")]
        public string AvailableAmount { get; set; }

        [MatrixxContractMember(Name = "TemplateId")]
        public string TemplateId { get; set; }

        [MatrixxContractMember(Name = "Name")]
        public string Name { get; set; }

        [MatrixxContractMember(Name = "ClassId")]
        public string ClassId { get; set; }

        [MatrixxContractMember(Name = "ClassName")]
        public string ClassName { get; set; }

        [MatrixxContractMember(Name = "Category")]
        public int Category { get; set; }

        [MatrixxContractMember(Name = "IsPrepaid")]
        public bool IsPrepaid { get; set; }

        [MatrixxContractMember(Name = "IsPeriodic")]
        public bool IsPeriodic { get; set; }

        [MatrixxContractMember(Name = "IsAggregate")]
        public bool IsAggregate { get; set; }

        [MatrixxContractMember(Name = "IsVirtual")]
        public bool IsVirtual { get; set; }

        [MatrixxContractMember(Name= "StartTime")]
        public DateTime StartTime { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime EndTime { get; set; }

        [MatrixxContractMember(Name = "QuantityUnit")]
        public string QuantityUnit { get; set; }
    }
}