using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BalancesContractBalanceQuotaListModelTranslatorUT
    {
        private Mock<IServiceTypeTranslator> mockServiceTypeTranslator;
        private Mock<IDataTypeCodeTranslator> mockDataTypeCodeTranslator;

        [TestInitialize]
        public void SetupEachTest()
        {
            mockServiceTypeTranslator = new Mock<IServiceTypeTranslator>();
            mockDataTypeCodeTranslator = new Mock<IDataTypeCodeTranslator>();

            mockServiceTypeTranslator.Setup(x => x.Translate(It.IsAny<String>()))
                .Returns(new List<BalanceConstants.ServiceType>() { BalanceConstants.ServiceType.DATA });

            mockDataTypeCodeTranslator.Setup(x => x.Translate(It.IsAny<String>(), It.IsAny<String>()))
                .Returns("MB");
        }

        [TestMethod]
        public void Translate_ShouldReturnResult()
        {
            var input = GetPreparedInputForException();

            var translatorUnderTest = new BalancesContractBalanceQuotaListModelTranslator(mockServiceTypeTranslator.Object, mockDataTypeCodeTranslator.Object);

            var translatedInput = translatorUnderTest.Translate(input);

            Assert.AreEqual(input[0].Buckets[0].InitialAmount, translatedInput.BalanceAllowances.First().Amount);
            Assert.AreEqual(input[0].DataTypeCode , Enum.GetName(typeof(BalanceConstants.ServiceType), translatedInput.BalanceAllowances.First().ServiceTypeCode));
            Assert.AreEqual(input[0].DataTypeName , translatedInput.BalanceAllowances.First().UnitType);
        }

        [TestMethod]
        public void Translate_ShoulReturnNull_If_InputIsNull()
        {
            var translatorUnderTest = new BalancesContractBalanceQuotaListModelTranslator(mockServiceTypeTranslator.Object, mockDataTypeCodeTranslator.Object);

            translatorUnderTest.Translate(null);

            Assert.IsNull(null);
        }
        
        [TestMethod]
        public void Translate_ShoulReturnNull_If_InputHasZeroElements()
        {
            var input = new AccountBalanceWithBucketsContract[0];

            var translatorUnderTest = new BalancesContractBalanceQuotaListModelTranslator(mockServiceTypeTranslator.Object, mockDataTypeCodeTranslator.Object);

            var translatedInput = translatorUnderTest.Translate(input);

            Assert.IsNull(translatedInput);
        }

        private AccountBalanceWithBucketsContract[] GetPreparedInputForException()
        {
            return GetPreparedInput("DATA");
        }

        private AccountBalanceWithBucketsContract[] GetPreparedInput(string dataTypeCode)
        {
            AccountBalanceWithBucketsContract[] accountBalanceWithBuckets = new AccountBalanceWithBucketsContract[1];
            CompanyBalanceBucketContract[] companyBalanceBuckets = new CompanyBalanceBucketContract[1];
            var bucket = new CompanyBalanceBucketContract()
            {
                InitialAmount = 11,
            };

            var accountBalanceWithBucketsContract = new AccountBalanceWithBucketsContract()
            {
                DataTypeName = "MB",
                DataTypeCode = dataTypeCode,
                Buckets = companyBalanceBuckets
            };

            companyBalanceBuckets[0] = bucket;

            accountBalanceWithBuckets[0] = accountBalanceWithBucketsContract;

            return accountBalanceWithBuckets;
        }
    }
}
