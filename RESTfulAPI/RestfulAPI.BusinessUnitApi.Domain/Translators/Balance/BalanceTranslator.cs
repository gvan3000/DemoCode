using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    public class BalanceTranslator : IBalanceTranslator
    {
        private readonly IServiceTypeTranslator _serviceTypeTranslator;
        private readonly IDataTypeCodeTranslator _dataTypeCodeTranslator;

        public BalanceTranslator(IServiceTypeTranslator serviceTypeTranslator, IDataTypeCodeTranslator dataTypeCodeTranslator)
        {
            _serviceTypeTranslator = serviceTypeTranslator;
            _dataTypeCodeTranslator = dataTypeCodeTranslator;
        }

        public BalancesResponseModel Translate(AccountBalanceWithBucketsContract[] input)
        {
            if (input == null || input.Length < 1)
                return null;

            BalancesResponseModel response = new BalancesResponseModel();

            BalanceModel balanceItem = null;
            foreach (var accountBalance in input)
            {
                if (accountBalance.Buckets != null && accountBalance.Buckets.Length > 0)
                {
                    foreach (var bucket in accountBalance.Buckets)
                    {
                        balanceItem = new BalanceModel
                        {
                            Balance = accountBalance.BalanceType,
                            UnitType = _dataTypeCodeTranslator.Translate(accountBalance.DataTypeName, accountBalance.DataTypeCode),
                            Amount = Math.Round(bucket.Amount, 2),
                            InitialAmount = bucket.InitialAmount.HasValue ? Math.Round(bucket.InitialAmount.Value, 2) : default(decimal?),
                            StartDate = bucket.StartDate,
                            ExpirationDate = bucket.ExpirationDate,
                            IsAddOn = bucket.IsOneOff.GetValueOrDefault(),
                            IsTransferred = bucket.IsTransferBalance.GetValueOrDefault()
                        };
                        balanceItem.ServiceType = _serviceTypeTranslator.Translate(balanceItem.UnitType);
                        balanceItem.Origin = new BalanceOrigin()
                        {
                            AddOnId = bucket.AddOnId,
                            CommercialOfferId = bucket.CommercialOfferId,
                            AddOnResourceId = bucket.AddOnResourceId
                        };
                        response.Balances.Add(balanceItem);
                    }
                }
            }

            return response;
        }
    }
}