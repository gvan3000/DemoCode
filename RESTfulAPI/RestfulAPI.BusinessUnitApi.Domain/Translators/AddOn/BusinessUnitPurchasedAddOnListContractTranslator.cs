using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn
{
    public class BusinessUnitPurchasedAddOnListContractTranslator : ITranslate<BusinessUnitPurchasedAddOnListContract, AddOnListModel>
    {
        private ITranslate<BusinessUnitPurchasedAddOnContract, AddOnModel> _innerTranslator;

        /// <summary>
        /// BusinessUnitPurchasedAddOnListContractTranslator
        /// </summary>
        /// <param name="innerTranslator"></param>
        public BusinessUnitPurchasedAddOnListContractTranslator(ITranslate<BusinessUnitPurchasedAddOnContract, AddOnModel> innerTranslator)
        {
            _innerTranslator = innerTranslator;
        }

        /// <summary>
        /// Translate BusinessUnitPurchasedAddOnListContract to AddOnListModel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public AddOnListModel Translate(BusinessUnitPurchasedAddOnListContract input)
        {
            var result = new AddOnListModel() { AddOns = new List<AddOnModel>() };
            if (input?.BusinessUnitPurchasedAddOns == null)
            {
                return result;
            }

            result.AddOns = input.BusinessUnitPurchasedAddOns.Select(x => _innerTranslator.Translate(x)).ToList();

            return result;
        }
    }
}
