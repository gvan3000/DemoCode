using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    /// <summary>
    /// product Allowed Balances Contract Translator
    /// </summary>
    public class ProductAllowedBalancesContractTranslator : ITranslate<List<ProductAllowedBalancesContract>, ProductAllowedBalanceList>
    {
        /// <summary>
        /// Translate list of product allowed balances contract to model
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ProductAllowedBalanceList Translate(List<ProductAllowedBalancesContract> input)
        {
            if (input == null)
            {
                return null;
            }

            ProductAllowedBalanceList result = new ProductAllowedBalanceList { ProductAllowedBalances = new List<ProductAllowedBalanceModel>() };

            var balancesGrouped = input.GroupBy(x => new { x.ProductId }, (key, group) => new
            {
                ProductId = key.ProductId,
                Grouped = group.ToList()
            }).ToList();

            foreach (var balance in balancesGrouped)
            {
                var balanceModel = new ProductAllowedBalanceModel
                {
                    ProductId = balance.ProductId
                };

                var balanceAllowances = new List<BalanceAllowanceModel>();
                balanceAllowances = balance.Grouped.Select(b =>
                    new BalanceAllowanceModel
                    {
                        Amount = b.Amount,
                        OutstandingAmount = b.OutstandingAmount,
                        ServiceTypeCode = b.ServiceTypeCode,
                        UnitType = b.UnitType
                    }).ToList();

                balanceModel.BalanceAllowances = balanceAllowances.Distinct().ToList();

                result.ProductAllowedBalances.Add(balanceModel);
            }

            return result;
        }
    }
}
