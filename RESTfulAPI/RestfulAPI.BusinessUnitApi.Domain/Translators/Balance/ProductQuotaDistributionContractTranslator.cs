using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public class ProductQuotaDistributionContractTranslator : ITranslate<ProductQuotaDistributionContract, BalanceQuotasListModel>
    {
        public BalanceQuotasListModel Translate(ProductQuotaDistributionContract input)
        {
            BalanceQuotasListModel output = new BalanceQuotasListModel();
            output = new BalanceQuotasListModel();
            output.BalanceAllowances = new List<BalanceQuotaModel>();
            if (input == null || !input.Quotas.Any())
            {
                return output;
            }

            BalanceQuotaModel model = null;
            foreach (var item in input.Quotas)
            {
                BalanceConstants.ServiceType serviceTypeCode;
                if (!Enum.TryParse(item.ServiceLevelType?.ToUpperInvariant(), out serviceTypeCode))
                {
                    throw new ArgumentException($"Invalid service type code \"{item.ServiceLevelType}\" in {nameof(ProductQuotaDistributionContract)}");
                }
                model = new BalanceQuotaModel()
                {
                    Amount = Math.Round(item.Amount, 2),
                    ServiceTypeCode = serviceTypeCode,
                    UnitType = UnitTypeTranslate(item.ServiceLevelType)
                };
                output.BalanceAllowances.Add(model);
            }

            return output;
        }


        private string UnitTypeTranslate(string serviceTypeCode)
        {
            switch (serviceTypeCode)
            {
                case "VOICE":
                    return "MINUTES";
                case "SMS":
                    return "SMS";
                case "DATA":
                    return "MB";
                case "QUOTA":
                    return "MONETARY";
                default:
                    return null;
            }
        }
    }
}
