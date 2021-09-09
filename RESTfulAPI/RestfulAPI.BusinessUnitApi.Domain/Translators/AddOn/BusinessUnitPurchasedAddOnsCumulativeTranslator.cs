using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;
using System.Collections.Generic;
using System.Linq;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.AddOnEnums;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    /// <summary>
    /// BusinessUnitPurchasedAddOns Translator
    /// </summary>
    public class BusinessUnitPurchasedAddOnsCumulativeTranslator : ITranslate<BusinessUnitPurchasedAddOnListContract, AddOnCumulativeListModel>
    {
        /// <summary>
        /// Translate BusinessUnits Purchased AddOns Contract to AddOnCumulativeList Model
        /// </summary>
        /// <param name="input">BusinessUnitPurchasedAddOnListContract</param>
        /// <returns><see cref="AddOnCumulativeListModel"/></returns>
        public AddOnCumulativeListModel Translate(BusinessUnitPurchasedAddOnListContract input)
        {
            if (input == null || input.BusinessUnitPurchasedAddOns == null)
            {
                return null;
            }

            var result = new AddOnCumulativeListModel { AddOns = new List<AddOnCumulativeModel>() };

            var addOnsGrouped = input.BusinessUnitPurchasedAddOns.GroupBy(x => new { x.Id, x.Name, x.PcrfTypeCode, x.AddOnType, x.StartDate, x.EndDate, x.Resourceid, x.IsOneOff, x.DataSpeedId }, (key, group) => new
            {
                AddOnId = key.Id,
                AddOnType = key.AddOnType,
                AddOnName = key.Name,
                AddOnPcrfType = key.PcrfTypeCode,
                AddOnStartDate = key.StartDate,
                AddOnEndDate = key.EndDate,
                AddOnResourceId = key.Resourceid,
                DataSpeedId = key.DataSpeedId,
                Grouped = group.ToList(),
                IsOneOff = key.IsOneOff
            }).ToList();

            foreach (var addOn in addOnsGrouped)
            {
                var addOnModel = new AddOnCumulativeModel
                {
                    Id = addOn.AddOnId,
                    Name = addOn.AddOnName,
                    Type = addOn.AddOnType,
                    PcrfType = addOn.AddOnPcrfType,
                    StartDate = addOn.AddOnStartDate,
                    EndDate = addOn.AddOnEndDate,
                    ResourceId = addOn.AddOnResourceId,
                    IsOneTime = addOn.IsOneOff,
                    SpeedId = addOn.DataSpeedId
                };

                var definitions = new List<AddOnDefinitionModel>();

                definitions = addOn.Grouped.Select(a => new AddOnDefinitionModel { Amount = a.Amount, UnitType = UnitTypeTranslate(a), ServiceTypeCode = ServiceTypeCodeTranslate(a) }).ToList();
                addOnModel.Definitions = definitions.Distinct().ToList();

                result.AddOns.Add(addOnModel);               
            }

            return result;
        }       

        private UnitType UnitTypeTranslate(BusinessUnitPurchasedAddOnContract addOn)
        {
            UnitType unitType = UnitType.MB;

            if (addOn.AddOnType == AddOnConstants.PCRF || addOn.AddOnType == AddOnConstants.DATALIMIT)
            {
                unitType = addOn.AddOnType != null && addOn.AddOnSubType != null ? (UnitType)Enum.Parse(typeof(UnitType), addOn.UnitType) : UnitType.MB;
            }

            if (addOn.AddOnType == AddOnConstants.ROAMING)
            {
                unitType = (UnitType)Enum.Parse(typeof(UnitType), ConvertServiceTypeCodeToUnitType(addOn.ServiceTypeCode));
            }

            if (!string.IsNullOrEmpty(addOn.AddOnType) && addOn.AddOnType == AddOnConstants.CASH)
            {
                unitType = (UnitType)Enum.Parse(typeof(UnitType), addOn.UnitType);
            }

            return unitType;
        }
        private ServiceTypeCode? ServiceTypeCodeTranslate(BusinessUnitPurchasedAddOnContract addOn)
        {
            ServiceTypeCode? resultServiceTypeCode = null;

            resultServiceTypeCode = !string.IsNullOrEmpty(addOn.ServiceTypeCode) ? (ServiceTypeCode)Enum.Parse(typeof(ServiceTypeCode), addOn.ServiceTypeCode) : resultServiceTypeCode;

            if (addOn.AddOnType == AddOnConstants.CASH && !string.IsNullOrEmpty(addOn.AddOnSubType))
            {
                resultServiceTypeCode = (ServiceTypeCode)Enum.Parse(typeof(ServiceTypeCode), addOn.AddOnSubType);
            }

            return resultServiceTypeCode;
        }


        private string ConvertServiceTypeCodeToUnitType(string serviceTypeCode)
        {
            switch (serviceTypeCode)
            {
                case AddOnConstants.VOICE:
                    return "MINUTES";
                case AddOnConstants.SMS:
                    return "SMS";
                case AddOnConstants.DATA:
                    return "MB";
                default:
                    return default(string);
            }
        }
    }
}
