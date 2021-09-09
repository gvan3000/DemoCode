using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System;
using System.Globalization;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ProductQuotaDistributionContractTranslatorUT
    {
        private ProductQuotaDistributionContractTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new ProductQuotaDistributionContractTranslator();
        }

        [TestMethod]
        public void Translate_SholdReturnEmptyBalanceAllowances_WhenQuotasAreEmptyList()
        {
            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract { Quotas = new QuotaDistributionAmountContract[] { } };

            var result = translatorUnderTest.Translate(contract);

            Assert.AreEqual(0, result.BalanceAllowances.Count());
        }

        [TestMethod]
        public void Translate_ShouldReturnModelWithEmptyBalanceAllowances_WhenInputIsNull()
        {
            ProductQuotaDistributionContract contract = null;

            var result = translatorUnderTest.Translate(contract);

            Assert.AreEqual(0, result.BalanceAllowances.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Translate_ShouldThowsArgumentException_WhenServiceTypeCodeIsNotrecognized()
        {
            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract
            {
                Quotas = new QuotaDistributionAmountContract[] { new QuotaDistributionAmountContract { ServiceLevelType = "NotRecognized"} }
            };

            var result = translatorUnderTest.Translate(contract);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_ContractServiceLevelType_ToServiceTypeCode()
        {
            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract
            {
                Quotas = new QuotaDistributionAmountContract[] 
                {
                    new QuotaDistributionAmountContract { Amount = 100, ServiceLevelType = "VOICE" },
                    new QuotaDistributionAmountContract { Amount = 500, ServiceLevelType = "SMS" },
                    new QuotaDistributionAmountContract { Amount = 50, ServiceLevelType = "DATA" },
                    new QuotaDistributionAmountContract { Amount = 160, ServiceLevelType = "QUOTA" }
                }
            };

            var result = translatorUnderTest.Translate(contract);

            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.VOICE));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.DATA));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.SMS));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.QUOTA));
            Assert.AreEqual(contract.Quotas.Length, result.BalanceAllowances.Count);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_ContractServiceLevelType_ToServiceTypeCode_IgnoreCase()
        {
            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract
            {
                Quotas = new QuotaDistributionAmountContract[]
                {
                    new QuotaDistributionAmountContract { Amount = 100, ServiceLevelType = "voice" },
                    new QuotaDistributionAmountContract { Amount = 500, ServiceLevelType = "sMs" },
                    new QuotaDistributionAmountContract { Amount = 50, ServiceLevelType = "Data" },
                    new QuotaDistributionAmountContract { Amount = 160, ServiceLevelType = "QUOTA" }
                }
            };

            var result = translatorUnderTest.Translate(contract);

            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.VOICE));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.DATA));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.SMS));
            Assert.IsTrue(result.BalanceAllowances.Any(b => b.ServiceTypeCode == BalanceConstants.ServiceType.QUOTA));
            Assert.AreEqual(contract.Quotas.Length, result.BalanceAllowances.Count);
        }

        [TestMethod]
        public void Translate_ShouldTranslateUnitTypeToMONETARY_WhenServiceLevelTypeIs_Quota()
        {
            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract
            {
                Quotas = new QuotaDistributionAmountContract[]
                {
                    new QuotaDistributionAmountContract { Amount = 160, ServiceLevelType = "QUOTA" }
                }
            };

            var result = translatorUnderTest.Translate(contract);

            Assert.IsTrue(result.BalanceAllowances.Any(b => b.UnitType == "MONETARY"));
        }

        [TestMethod]
        public void Translate_ShouldReturnAmountRoundedOn2DigitsAfterDecimalPoint()
        {
            int numberOfDigits = 2;

            ProductQuotaDistributionContract contract = new ProductQuotaDistributionContract
            {
                Quotas = new QuotaDistributionAmountContract[]
                {
                    new QuotaDistributionAmountContract { Amount = 200.555577777777M, ServiceLevelType = "DATA" }
                }
            };

            var result = translatorUnderTest.Translate(contract);

            var amount = result.BalanceAllowances[0].Amount.ToString(CultureInfo.InvariantCulture);

            var lengthOfDigitsAfterDecimalPoint = amount.Substring(amount.IndexOf(".", StringComparison.Ordinal) +1).Length;            

            Assert.AreEqual(numberOfDigits, lengthOfDigitsAfterDecimalPoint);
        }       
    }
}
