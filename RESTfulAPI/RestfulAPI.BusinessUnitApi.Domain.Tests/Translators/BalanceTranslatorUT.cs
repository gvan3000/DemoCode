using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;
using System.Collections.Generic;
using System.Linq;
using static RestfulAPI.Constants.BalanceConstants;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BalanceTranslatorUT
    {
        private Mock<IServiceTypeConfigurationProvider> mockConfig;

        [TestInitialize]
        public void SetupEachTest()
        {
            mockConfig = new Mock<IServiceTypeConfigurationProvider>();
            mockConfig.Setup(x => x.CurrencyUnitNames).Returns(new Dictionary<string, string>() { { "$", "DOLLARS" }, { "€", "EUROS" } });
            mockConfig.Setup(x => x.DataCodes).Returns(new List<string>() { "KB", "MB", "GB", "TB" });
            mockConfig.Setup(x => x.GeneralCacheCodes).Returns(new List<string>() { "EUROS", "POUNDS", "DOLLARS", "ZLOTYCH", "NONE" });
            mockConfig.Setup(x => x.SmsCodes).Returns(new List<string>() { "UNIT", "SMS", "MINUTESMS" });
            mockConfig.Setup(x => x.VoiceCodes).Returns(new List<string>() { "SECOND", "MINUTE", "HOUR", "DAY", "WEEK", "MONTH", "YEAR" });
            mockConfig.Setup(x => x.VoiceUnitNames).Returns(new Dictionary<string, string>() { { "min", "MINUTE" }, { "h", "HOUR" }, { "d", "DAY" } });
        }

        [TestMethod]
        public void When_Input_Is_Null_Should_Return_Null()
        {
            var translator = new BalanceTranslator(null, null);
            var result = translator.Translate(null);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void When_Input_Is_Empty_Should_Return_Null()
        {
            var translator = new BalanceTranslator(null, null);
            var result = translator.Translate(new AccountBalanceWithBucketsContract[] { });
            Assert.IsNull(result);
        }

        [TestMethod]
        public void When_Input_Buckets_Is_Null_Should_Return_Empty()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract { Amount = 100, BalanceType = "test balance 1", DataTypeCode = "KB", Buckets = null },
                new AccountBalanceWithBucketsContract { Amount = 200, BalanceType = "test balance 2", DataTypeCode = "KB", Buckets = null }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);
            Assert.IsTrue(result.Balances.Count < 1);
        }

        [TestMethod]
        public void When_Input_With_Buckets_Should_Return_Same_Count_Of_Buckets()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    Amount = 100,
                    BalanceType = "test balance 1",
                    DataTypeCode = "KB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount=101 },
                        new CompanyBalanceBucketContract { Amount=102 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "KB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);

            int expectedBalancesCount = dummyBalances.Sum(b => b.Buckets.Length);
            Assert.IsTrue(result.Balances.Count == expectedBalancesCount);
        }

        [TestMethod]
        public void When_Input_With_DataTypeCode_Data_Should_Return_ServiceType_Data()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    Amount = 100,
                    BalanceType = "test balance 1",
                    DataTypeCode = "KB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount=101 },
                        new CompanyBalanceBucketContract { Amount=102 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "MB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "MINUTE",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);

            int countSTData = result.Balances.Count(c => c.ServiceType.Contains(ServiceType.DATA));
            int countExpected = dummyBalances.Sum(d => d.Buckets.Length) - 1; //Minus one cause of MINUTE
            Assert.IsTrue(countSTData == countExpected);
        }

        [TestMethod]
        public void When_Input_With_DataTypeCode_Voice_Should_Return_ServiceType_Voice()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    Amount = 100,
                    BalanceType = "test balance 1",
                    DataTypeCode = "KB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount=101 },
                        new CompanyBalanceBucketContract { Amount=102 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "DAY",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "MINUTE",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);

            int countSTData = result.Balances.Count(c => c.ServiceType.Contains(ServiceType.VOICE));
            int countExpected = dummyBalances.Sum(d => d.Buckets.Length) - 2; //Minus two cause of KB
            Assert.AreEqual(countExpected, countSTData);
        }

        [TestMethod]
        public void When_Input_With_DataTypeCode_SMS_Should_Return_ServiceType_SMS()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    Amount = 100,
                    BalanceType = "test balance 1",
                    DataTypeCode = "SMS",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount=101 }
                    }
                },
                new AccountBalanceWithBucketsContract
                {
                    Amount = 200,
                    BalanceType = "test balance 2",
                    DataTypeCode = "UNIT",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount = 123 }
                    }
                }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);

            int countSTData = result.Balances.Count(c => c.ServiceType.Contains(ServiceType.SMS));
            int countExpected = dummyBalances.Sum(d => d.Buckets.Length);
            Assert.IsTrue(countSTData == countExpected);
        }

        [TestMethod]
        public void When_Input_With_DataTypeCode_GeneralCash_Should_Return_All_ServiceType()
        {
            var dummyBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    Amount = 100,
                    BalanceType = "test balance 1",
                    DataTypeCode = "EUROS",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract { Amount=101 },
                    }
                }
            };

            var translator = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translator.Translate(dummyBalances);

            var enumNames = Enum.GetValues(typeof(ServiceType)).Cast<ServiceType>().Where(x => x != ServiceType.QUOTA).ToList();
            int countST = result.Balances[0].ServiceType.Count;

            Assert.IsTrue(countST == enumNames.Count);
        }

        [TestMethod]
        public void Translate_ShouldUseDataTypeNameIfCodeIsNotAvailable()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test balance 1",
                    DataTypeCode = null,
                    DataTypeName = "$",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 100,
                            ExpirationDate = DateTime.Now,
                            StartDate = DateTime.Now.AddDays(-5),
                            InitialAmount = 150
                        }
                    },
                }
            };
            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Balances.Count);
            Assert.IsNotNull(result.Balances[0].ServiceType);
            Assert.IsNotNull(result.Balances[0].UnitType);
        }

        [TestMethod]
        public void Translate_AddOnShouldBeFalseIfIsOneOffIsNull()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test balance 1",
                    DataTypeCode = null,
                    DataTypeName = "$",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 100,
                            ExpirationDate = DateTime.Now,
                            StartDate = DateTime.Now.AddDays(-5),
                            InitialAmount = 150,
                            IsOneOff = null
                        }
                    },
                }
            };
            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsFalse(result.Balances.First().IsAddOn);
        }

        [TestMethod]
        public void Translate_ShouldFillInAddOnIdAndCommercialOffer()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test balance 1",
                    DataTypeCode = null,
                    DataTypeName = "$",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 100,
                            ExpirationDate = DateTime.Now,
                            StartDate = DateTime.Now.AddDays(-5),
                            InitialAmount = 150,
                            IsOneOff = null,
                            AddOnId = Guid.NewGuid(),
                            CommercialOfferId = Guid.NewGuid() // it is up to the source to make usre only one of those is set
                        }
                    },
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Balances);
            Assert.IsNotNull(result.Balances[0]);
            Assert.IsNotNull(result.Balances[0].Origin);
            Assert.IsNotNull(result.Balances[0].Origin.AddOnId);
            Assert.IsNotNull(result.Balances[0].Origin.CommercialOfferId);
        }

        [TestMethod]
        public void Translate_ShouldHandleNullsForAddOnAndCommercialOfferAndLeaveSubContractEmpty()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test balance 1",
                    DataTypeCode = null,
                    DataTypeName = "$",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 100,
                            ExpirationDate = DateTime.Now,
                            StartDate = DateTime.Now.AddDays(-5),
                            InitialAmount = 150,
                            IsOneOff = null,
                            AddOnId = null,
                            CommercialOfferId = null
                        }
                    },
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Balances);
            Assert.IsNotNull(result.Balances[0]);
            Assert.IsNotNull(result.Balances[0].Origin);
            Assert.IsNull(result.Balances[0].Origin.AddOnId);
            Assert.IsNull(result.Balances[0].Origin.CommercialOfferId);
        }

        [TestMethod]
        public void Translate_ShouldRoundAmountAndInitialAmountToZeroWhenTheyAreTooSmall()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test test 123",
                    DataTypeCode = null,
                    DataTypeName = "MB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 9.5367431640625e-7M,
                            InitialAmount = 9.5367431640625e-7M,
                            ExpirationDate = DateTime.Now.AddDays(7),
                            StartDate = DateTime.Now.AddDays(-7)
                        }
                    }
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Balances);
            Assert.IsNotNull(result.Balances[0]);
            Assert.AreEqual(0M, result.Balances[0].Amount);
            Assert.AreEqual(0M, result.Balances[0].InitialAmount);
        }

        [TestMethod]
        public void Translate_ShouldRoundAmountAndInitialAmountToTwoDecimalPlaces()
        {
            var testBalances = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract()
                {
                    BalanceType = "test test 123",
                    DataTypeCode = null,
                    DataTypeName = "MB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract()
                        {
                            Amount = 1.23456789M,
                            InitialAmount = 1.23456789M,
                            ExpirationDate = DateTime.Now.AddDays(7),
                            StartDate = DateTime.Now.AddDays(-7)
                        }
                    }
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));
            var result = translatorUnderTest.Translate(testBalances);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Balances);
            Assert.IsNotNull(result.Balances[0]);
            Assert.AreEqual(1.23M, result.Balances[0].Amount);
            Assert.AreEqual(1.23M, result.Balances[0].InitialAmount);
        }

        [TestMethod]
        public void Translate_ShouldMapp_AddOnResourceIdContract_to_AddOnResourceIdResponse()
        {
            int? addOnResourceId1 = 11;
            int? addOnResourceId2 = 13;

            var contract = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    BalanceType = "blnc_test_1",
                    DataTypeCode = "MB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract
                        {
                            Amount = 1.5454545M,
                            InitialAmount = 1.5454545M,
                            StartDate = DateTime.Now.AddDays(-3),
                            IsOneOff = true,
                            IsTransferBalance = true,
                            AddOnResourceId = addOnResourceId1,                                                   
                        },
                        new CompanyBalanceBucketContract
                        {
                            Amount = 1.5454545M,
                            InitialAmount = 1.5454545M,
                            StartDate = DateTime.Now.AddDays(-3),
                            IsOneOff = true,
                            IsTransferBalance = true,
                            AddOnResourceId = addOnResourceId2,
                        }
                    }                    
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));

            var result = translatorUnderTest.Translate(contract);

            var addOnResourceIds = result.Balances.Select(x => x.Origin.AddOnResourceId).ToList();

            Assert.IsTrue(addOnResourceIds.Contains(addOnResourceId1));
            Assert.IsTrue(addOnResourceIds.Contains(addOnResourceId2));
        }

        [TestMethod]
        public void Translate_IfContractAddOnResourceIdIsNull_ModelAddOnResourceIdIsNull()
        {
            var contract = new AccountBalanceWithBucketsContract[]
            {
                new AccountBalanceWithBucketsContract
                {
                    BalanceType = "blnc_test_1",
                    DataTypeCode = "MB",
                    Buckets = new CompanyBalanceBucketContract[]
                    {
                        new CompanyBalanceBucketContract
                        {
                            Amount = 1.5454545M,
                            InitialAmount = 1.5454545M,
                            StartDate = DateTime.Now.AddDays(-3),
                            IsOneOff = true,
                            AddOnResourceId = null,
                        },
                        new CompanyBalanceBucketContract
                        {
                            Amount = 1.5454545M,
                            InitialAmount = 1.5454545M,
                            StartDate = DateTime.Now.AddDays(-3),
                            AddOnResourceId = null,
                        }
                    }
                }
            };

            var translatorUnderTest = new BalanceTranslator(new TeleenaServiceReferences.Translators.ServiceTypeTranslator(mockConfig.Object), new TeleenaServiceReferences.Translators.DataTypeCodeTranslator(mockConfig.Object));

            var result = translatorUnderTest.Translate(contract);

            var addOnResourceIds = result.Balances.Select(x => x.Origin.AddOnResourceId).ToList();

            Assert.IsTrue(addOnResourceIds.Contains(null));
        }
    }
}