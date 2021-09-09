using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class SaveQuotaDistributionContactTranslatorUT
    {
        private Guid productId = Guid.NewGuid();
        private Guid businessUnitId = Guid.NewGuid();
        private PropositionInfoModel propositionInfo;

        [TestInitialize]
        public void TestInitialize()
        {
            propositionInfo = new PropositionInfoModel
            {
                PropositionId = Guid.NewGuid(),
                EndUserSubscription = true,
                CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                {
                    new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB06", ServiceTypeCode = "DATA", SubscriptionTypeCode = "SHB" }
                }
            };
        }
        
        [TestMethod]
        public void Translate_UnitType_kB_to_MB()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                Amount = 12300.5M,
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.kB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.IsNotNull(transalatedValue);
            Assert.IsNotNull(transalatedValue.QuotaAmounts);
            Assert.IsNotNull(transalatedValue.QuotaAmounts.Select(x => x.Amount));
        }

        [TestMethod]
        public void Translate_UnitType_kB_to_MB_round()
        {
            decimal amount = 22330.5M;
            SetBalanceModel model = new SetBalanceModel
            {
                Amount = amount,
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.kB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            int expectedCalculatedAmount = (int)Math.Round(amount / 1024, MidpointRounding.AwayFromZero);
            var transaltedAmount = transalatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault();

            Assert.AreEqual(expectedCalculatedAmount, transaltedAmount);
        }

        [TestMethod]
        public void Transalte_UnitType_GB_to_MB()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                Amount = 56.7M,
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.GB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.IsNotNull(transalatedValue);
            Assert.IsNotNull(transalatedValue.QuotaAmounts);
            Assert.IsNotNull(transalatedValue.QuotaAmounts.Select(x => x.Amount));
        }

        [TestMethod]
        public void Translate_UnitType_GB_to_MB_round()
        {
            decimal amount = 101.4M;

            SetBalanceModel model = new SetBalanceModel
            {
                Amount = amount,
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.GB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            int expectedCalculatedAmount = (int)Math.Round(amount * 1024, MidpointRounding.AwayFromZero);
            var transaltedAmountValue = transalatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault();

            Assert.AreEqual(expectedCalculatedAmount, transaltedAmountValue);
        }

        [TestMethod]
        public void Translate_ShouldMappAllFields_ServiceType_SMS()
        {
            string commercialOfferDefCode = "SHB06";

            propositionInfo = new PropositionInfoModel
            {
                PropositionId = Guid.NewGuid(),
                EndUserSubscription = true,
                CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                {
                    new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB06", ServiceTypeCode = "SMS", SubscriptionTypeCode = "SMS" }
                }
            };
            decimal amount = 55M;

            SetBalanceModel model = new SetBalanceModel
            {
                Amount = amount,
                ServiceTypeCode = BalanceConstants.ServiceType.SMS,
                UnitTypeValue = BusinessUnitsEnums.UnitType.SMS
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.AreEqual(transalatedValue.AccountId, businessUnitId);
            Assert.AreEqual(transalatedValue.ProductIds.First(), productId);
            Assert.AreEqual(transalatedValue.QuotaAmounts.Select(x => x.CommercialOfferDefinitionCode).FirstOrDefault(), commercialOfferDefCode);
            Assert.IsFalse(transalatedValue.QuotaAmounts.Select(x => x.Remove).FirstOrDefault());
        }

        [TestMethod]
        public void Translate_ShouldMappAllFields_ServiceType_Voice()
        {
            string commercialOfferDefCode = "SHB06";
            propositionInfo = new PropositionInfoModel
            {
                PropositionId = Guid.NewGuid(),
                EndUserSubscription = true,
                CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                {
                    new CommercialOfferDefinition { CommercialOfferDefinitionCode = commercialOfferDefCode, ServiceTypeCode = "VOICE", SubscriptionTypeCode = "SHB" }
                }
            };
            decimal amount = 125.4M;

            SetBalanceModel model = new SetBalanceModel
            {
                Amount = amount,
                ServiceTypeCode = BalanceConstants.ServiceType.VOICE,
                UnitTypeValue = BusinessUnitsEnums.UnitType.MINUTES
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.AreEqual(transalatedValue.AccountId, businessUnitId);
            Assert.AreEqual(transalatedValue.ProductIds.First(), productId);
            Assert.AreEqual(transalatedValue.QuotaAmounts.Select(x => x.CommercialOfferDefinitionCode).FirstOrDefault(), commercialOfferDefCode);
            Assert.IsFalse(transalatedValue.QuotaAmounts.Select(x => x.Remove).FirstOrDefault());
        }

        [TestMethod]
        public void Translate_ShouldreturnNull_IfInputIsNull()
        {
            SetBalanceModel model = null;

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.IsNull(transalatedValue);
        }

        [TestMethod]
        public void Translate_UnitType_kB_to_MB_round_Amount_Zero()
        {
            decimal amount = 0M;
            SetBalanceModel model = new SetBalanceModel
            {
                Amount = 0,
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.kB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            int expectedCalculatedAmount = (int)Math.Round(amount / 1024, MidpointRounding.AwayFromZero);
            var transaltedAmount = transalatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault();

            Assert.AreEqual(expectedCalculatedAmount, transaltedAmount);
        }

        [TestMethod]
        public void Translate_UnitType_kB_to_MB_ShouldReturnAmount_0_IfAmountNull()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.kB
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var transalatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.IsNotNull(transalatedValue);
            Assert.AreEqual(0, transalatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault());
        }

        [TestMethod]
        public void Translate_UnitType_SMS_ShouldReturnAmount_0_IfAmountNull()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.SMS,
                UnitTypeValue = BusinessUnitsEnums.UnitType.SMS
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var translatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfo, model);

            Assert.IsNotNull(translatedValue);
            Assert.AreEqual(0, translatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault());
        }


        [TestMethod]
        public void Translate_ServiceType_Quota_ShouldReturnAmount_0_IfAmountNull()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.QUOTA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.MONETARY
            };

            var propositionInfoLocal = new PropositionInfoModel
            {
                PropositionId = Guid.NewGuid(),
                EndUserSubscription = true,
                
                CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                {
                    new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB06", ServiceTypeCode = "QUOTA", SubscriptionTypeCode = "SHB" }
                }
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var translatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfoLocal, model);

            Assert.IsNotNull(translatedValue);
            Assert.AreEqual(0, translatedValue.QuotaAmounts.Select(x => x.Amount).FirstOrDefault());
        }

        [TestMethod]
        public void Translate_ServiceType_Quota_Should_Set_CommercialOfferDefinitionCode_To_QuotaOfferCode()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.QUOTA,
                UnitTypeValue = BusinessUnitsEnums.UnitType.MONETARY,
                Amount = 50
            };

            string QuotaOfferCodeString = "FakeQuotaOfferCode";

            var propositionInfoLocal = new PropositionInfoModel
            {
                PropositionId = Guid.NewGuid(),
                EndUserSubscription = true,
                QuotaOfferCode = QuotaOfferCodeString,

                CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                {
                    new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB06", ServiceTypeCode = "QUOTA", SubscriptionTypeCode = "SHB" }
                }
            };

            var translatorUnderTest = new SaveQuotaDistributionContractTranslator();

            var translatedValue = translatorUnderTest.Translate(businessUnitId, productId, propositionInfoLocal, model);
             
            Assert.IsNotNull(translatedValue);
            Assert.AreEqual(QuotaOfferCodeString, translatedValue.QuotaAmounts.FirstOrDefault().CommercialOfferDefinitionCode);
        }
    }
}
