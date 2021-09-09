using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public class BalancesContractBalanceQuotaListModelTranslator : ITranslate<AccountBalanceWithBucketsContract[], BalanceQuotasListModel>
    {
        private readonly IServiceTypeTranslator _serviceTypeTranslator;
        private readonly IDataTypeCodeTranslator _dataTypeCodeTranslator;


        public BalancesContractBalanceQuotaListModelTranslator(IServiceTypeTranslator serviceTypeTranslator, IDataTypeCodeTranslator dataTypeCodeTranslator)
        {
            _serviceTypeTranslator = serviceTypeTranslator;
            _dataTypeCodeTranslator = dataTypeCodeTranslator;
        }

        /// <summary>
        /// Does translation ignoring Account balances of General Cash service type
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BalanceQuotasListModel Translate(AccountBalanceWithBucketsContract[] input)
        {
            if (input == null || input.Length < 1)
                return null;

            BalanceQuotasListModel response = new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() };

            BalanceQuotaModel quotaModel;
            foreach (var accountBalance in input)
            {
                var translatedServiceTypeCodes = _serviceTypeTranslator.Translate(accountBalance.DataTypeCode);
                bool isGeneralCashAccountBalance = translatedServiceTypeCodes.Count > 1;
                if (accountBalance.Buckets != null && accountBalance.Buckets.Length > 0 && !isGeneralCashAccountBalance)
                {
                    foreach (var bucket in accountBalance.Buckets)
                    {
                        {
                            quotaModel = new BalanceQuotaModel
                            {
                                ServiceTypeCode = translatedServiceTypeCodes.First(),
                                UnitType = _dataTypeCodeTranslator.Translate(accountBalance.DataTypeName, accountBalance.DataTypeCode),
                                Amount = Math.Round(bucket?.InitialAmount ?? 0, 2)
                            };

                            response.BalanceAllowances.Add(quotaModel);
                        }
                    }
                }
            }

            return response;
        }
    }
}
