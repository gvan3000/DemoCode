using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;
using static RestfulAPI.Constants.BalanceConstants;
using System.Linq;
using RestfulAPI.TeleenaServiceReferences.Constants;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition
{
    /// <summary>
    /// PropositionsContract translator
    /// </summary>
    public class PropositionsContractTranslator : IPropositionContractTranslator
    {
        /// <summary>
        /// Converts availabe propositions to PropositionsResponseModel
        /// </summary>
        /// <param name="activePropositions"></param>
        /// <param name="productCreationPropositions"></param>
        /// <returns></returns>
        public PropositionsResponseModel Translate(PropositionsContract activePropositions, PropositionsContract productCreationPropositions)
        {
            if (activePropositions == null || activePropositions.PropositionContracts == null || activePropositions.PropositionContracts.Count <= 0)
                return null;

            PropositionModel itemToAdd = null;
            PropositionsResponseModel output = new PropositionsResponseModel();
            foreach (var item in activePropositions.PropositionContracts)
            {
                itemToAdd = Translate(item);
                itemToAdd.IsAvailableForProductCreation = productCreationPropositions != null
                    && productCreationPropositions.PropositionContracts.Any(contract => contract.Id == item.Id);
                output.Propositions.Add(itemToAdd);
            }

            return output;
        }

        private PropositionModel Translate(PropositionContract contract)
        {
            PropositionModel model = new PropositionModel()
            {
                BalanceProfileCode = contract.BalanceProfileCode,
                CoPropositionCode = contract.CommercialOfferPropositionCode,
                CoConfigurations = CommercialOfferConfigurationsContractTranslate(contract.CommercialOfferConfigurationsContract),
                Code = contract.Code,
                Id = contract.Id,
                Name = contract.Name,
                ProductLifeCycleCode = contract.ProductLifeCycleCode,
                ProductTypeCode = contract.ProductTypeCode,
                TestComPropositionCode = contract.TestCommercialOfferPropositionCode
            };

            return model;
        }

        private List<CommercialOfferConfig> CommercialOfferConfigurationsContractTranslate(CommercialOfferConfigurationsContract contract)
        {
            if (contract == null || contract.CommercialOfferConfigurationContracts == null || contract.CommercialOfferConfigurationContracts.Count <= 0)
                return null;

            List<CommercialOfferConfig> output = new List<CommercialOfferConfig>();
            foreach (var item in contract.CommercialOfferConfigurationContracts)
            {
                CommercialOfferConfig commercialOfferConfig = new CommercialOfferConfig()
                {
                    BlackZones = item.BlackListedZones?.Zones,
                    BundleAmount = item.BundleAmount,
                    Code = item.Code,
                    Id = item.Id,
                    IsSharedWallet = item.IsSharedWallet,
                    Name = item.Name,
                    WhiteZones = item.WhiteListedZones?.Zones,
                    Quota = item.Quota,
                    ServiceTypeCode = (ServiceType)Enum.Parse(typeof(ServiceType), item.ServiceLevelTypeCode, true),
                    SubscriptionCode = item.SubscriptionTypeCode,
                    TresholdLimits = TresHoldLimitsTranslate(item.ThresHoldLimits),
                    PeriodCode = (PeriodCode)Enum.Parse(typeof(PeriodCode), item.BundlePeriod.PeriodeCode, true),
                    AvailableZones = TranslateZoneType(item.ZoneType),
                    Metering = TranslateMetering(item),
                    FUPMeterings = TranslateFupMetering(item)
                };
                output.Add(commercialOfferConfig);
            }

            return output;
        }

        private List<Tresholds> TresHoldLimitsTranslate(List<CommercialOfferThresHoldLimitContract> input)
        {
            if (input == null)
                return null;

            var output = new List<Tresholds>();

            input.Where(x => x != null && x.Type != null && x.Type != PropositionConstants.METERING_THRESHOLD_TYPE).ToList()
                .ForEach(y => output.Add(new Tresholds { Id = y.Id, Limit = y.Limit, Type = (TypeOfTreshold)Enum.Parse(typeof(TypeOfTreshold), y.Type, true) }));

            return output;
        }

        private int? TranslateZoneType(string types)
        {
            if (types == null)
                return null;

            string digitsInType = string.Empty;

            foreach (var letter in types)
            {
                if (!Char.IsDigit(letter))
                    break;
                digitsInType += letter;
            }

            if (String.IsNullOrEmpty(digitsInType))
                return null;

            return Convert.ToInt32(digitsInType);
        }

        private Metering TranslateMetering(CommercialOfferConfigurationContract input)
        {
            if (input == null)
                return null;

            var thresholdLimits = new List<ThresholdLimits>();

            if (input.ThresHoldLimits == null)
                thresholdLimits = null;
            else
                thresholdLimits = input.ThresHoldLimits.Where(t => t.Type == PropositionConstants.METERING_THRESHOLD_TYPE)
                    .Select(t => new ThresholdLimits { Limit = t.Limit }).ToList();

            decimal amount = 0;
            if (thresholdLimits != null && thresholdLimits.Count > 0)
                amount = thresholdLimits.Select(l => l.Limit).Max();
            else
                return null;

            var zones = input.QuotaMeteringZones == null ? new List<int>() : input.QuotaMeteringZones.Zones;

            var output = new Metering
            {
                MeterType = "Quota Meter",
                Zones = zones,
                Amount = amount,
                ThresholdLimits = thresholdLimits
            };

            return output;
        }

        private List<Metering> TranslateFupMetering(CommercialOfferConfigurationContract input)
        {
            List<Metering> ret = new List<Metering>();
            var thresholdExtendedLimits = new List<ExtendedThresholdLimits>();
            var thresholdLimits = new List<ThresholdLimits>();

            if (input.ThresHoldLimits != null)
                thresholdExtendedLimits = input.ThresHoldLimits.Where(t => t.Type == PropositionConstants.FUP_METERING_THRESHOLD_TYPE)
                    .Select(t => new ExtendedThresholdLimits { Limit = t.Limit, DataTypeUnitName = t.DataTypeUnitName, MeterTypeName = t.MeterTypeName }).ToList();

            var uniqueMeters = thresholdExtendedLimits.Select(m => m.MeterTypeName).Distinct();

            foreach (var meterTypeName in uniqueMeters)
            {
                var subThresholdExtendedLimits = new List<ExtendedThresholdLimits>();
                subThresholdExtendedLimits = thresholdExtendedLimits.Where(m => m.MeterTypeName == meterTypeName).ToList();

                decimal amount = 0;
                string unitType = "";
                if (subThresholdExtendedLimits != null && subThresholdExtendedLimits.Count > 0)
                {
                    amount = subThresholdExtendedLimits.Select(l => l.Limit).Max();
                    unitType = subThresholdExtendedLimits.Select(m => m.DataTypeUnitName).FirstOrDefault();
                    thresholdLimits = subThresholdExtendedLimits.Select(t => new ThresholdLimits { Limit = t.Limit }).ToList();
                }

                var meter = new Metering
                {
                    Amount = amount,
                    ThresholdLimits = thresholdLimits,
                    MeterType = meterTypeName,
                    UnitType = unitType,
                    Zones = new List<int>()
                };

                if (ret == null)
                    ret = new List<Metering>();

                if (meter.Amount != 0 && meter.ThresholdLimits.Count > 0)
                    ret.Add(meter);
            }

            return (ret != null && ret.Count > 0) ? ret : null;
        }
    }
}
