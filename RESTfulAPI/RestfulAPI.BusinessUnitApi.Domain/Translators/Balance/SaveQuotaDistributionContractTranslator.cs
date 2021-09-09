using System;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using System.Linq;
using RestfulAPI.Constants;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public class SaveQuotaDistributionContractTranslator : ISaveQuotaDistributionContractTranslator
    {
        const int DATA_UNIT_SCALING_FACTOR = 1024;

        public SaveQuotaDistributionContract Translate(Guid businessUnitId, Guid productId, PropositionInfoModel propositionInfo, SetBalanceModel request)
        {
            if (request == null)
            {
                return null;
            }

            var output = new SaveQuotaDistributionContract
            {
                AccountId = businessUnitId,
                ProductIds = new Guid[] { productId },
                QuotaAmounts = new SaveQuotaDistributionAmountContract[]
                {
                    new SaveQuotaDistributionAmountContract
                        {
                            Amount = CalculateAmount(request),
                            CommercialOfferDefinitionCode = SetCommercialOfferCode(propositionInfo, request),
                            Remove = false
                        }
                }
            };


            return output;
        }

        private string SetCommercialOfferCode(PropositionInfoModel propositionInfo, SetBalanceModel request)
        {
            string commercialOfferCode;
            if (request.ServiceTypeCode.ToString().Equals(BalanceConstants.SERVICE_TYPE_CODE_QUOTA_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                commercialOfferCode = propositionInfo.QuotaOfferCode;
            }
            else
            {
                commercialOfferCode = propositionInfo.CommercialOfferDefinitions
                                            .Where(c => c.ServiceTypeCode.ToUpperInvariant() == request.ServiceTypeCode.ToString().ToUpperInvariant())
                                            .Select(x => x.CommercialOfferDefinitionCode).FirstOrDefault();
            }
            return commercialOfferCode;
        }

        private int CalculateAmount(SetBalanceModel input)
        {
            decimal amount = input.Amount.GetValueOrDefault();

            if (input.UnitTypeValue == BusinessUnitsEnums.UnitType.kB)
            {
                amount /= DATA_UNIT_SCALING_FACTOR;
            }
            else if (input.UnitTypeValue == BusinessUnitsEnums.UnitType.GB)
            {
                amount *= DATA_UNIT_SCALING_FACTOR;
            }

            int roundedAmount = (int)Math.Round(amount, MidpointRounding.AwayFromZero);

            return roundedAmount;
        }


    }
}
