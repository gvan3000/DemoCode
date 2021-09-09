using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ProductAllowedBalancesContractUT
    {
        private ProductAllowedBalancesContractTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new ProductAllowedBalancesContractTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfInputIsNull()
        {
            List<ProductAllowedBalancesContract> input = null;

            var result = translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldGroupByProductId()
        {
            Guid productID = Guid.NewGuid();

            List<ProductAllowedBalancesContract> input = new List<ProductAllowedBalancesContract>
            {
              new ProductAllowedBalancesContract {Amount = 10, OutstandingAmount = 25, ProductId = productID, ServiceTypeCode = "DATA", UnitType = "MB" },
              new ProductAllowedBalancesContract {Amount = 10, OutstandingAmount = 2, ProductId = productID, ServiceTypeCode = "VOICE", UnitType = "MINUTES" },
              new ProductAllowedBalancesContract {Amount = 10, OutstandingAmount = 10, ProductId = productID, ServiceTypeCode = "DATA", UnitType = "MB" },
              new ProductAllowedBalancesContract {Amount = 10, OutstandingAmount = 25, ProductId = productID, ServiceTypeCode = "SMS", UnitType = "SMS" }
            };

            var result = translatorUnderTest.Translate(input);

            Assert.AreEqual(1, result.ProductAllowedBalances.Count());
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyList_IfInputEmptyList()
        {
            Guid productID = Guid.NewGuid();

            List<ProductAllowedBalancesContract> input = new List<ProductAllowedBalancesContract>();

            var result = translatorUnderTest.Translate(input);

            Assert.AreEqual(0, result.ProductAllowedBalances.Count());
        }
    }
}
