using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.CommercialOfferService;
using RestfulAPI.TeleenaServiceReferences.ProductService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CommercialOfferDefinition = RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance.CommercialOfferDefinition;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class BalanceProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockTeleenaServices;
        private Mock<BalanceService> _mockBalanceService;
        private Mock<IBusinessUnitApiTranslators> _mockTranslators;
        private Mock<IServiceTypeConfigurationProvider> mockConfig;
        private Mock<IJsonRestApiLogger> mockLogger;
        private SaveQuotaDistributionContract _quoateDistributionContract;
        private Guid _businessUnitId;
        private Guid _productId;
        private PropositionsContract _propositionsContract;
        private AllowedPropositionContract _allowedPropositioContract;
        private SaveQuotaDistributionResultContract _saveQuoateDistributionresult;
        private Guid _requestID;
        private ProductContract _productContract;
        private List<PropositionInfoModel> _propositionsInfo;
        private CommercialOfferPropositionContract _commercialOfferPropositionContract;

        [TestInitialize]
        public void TestInit()
        {
            _mockTeleenaServices = new Mock<ITeleenaServiceUnitOfWork>();
            _mockTranslators = new Mock<IBusinessUnitApiTranslators>();
            mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);

            _businessUnitId = Guid.NewGuid();
            _productId = Guid.NewGuid();
            _allowedPropositioContract = new AllowedPropositionContract { EndUserSubscription = true, PropositionId = Guid.NewGuid() };
            _saveQuoateDistributionresult = new SaveQuotaDistributionResultContract { Success = true };

            _requestID = Guid.NewGuid();

            _propositionsContract = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract { Id = Guid.NewGuid() },
                    new PropositionContract { Id = Guid.NewGuid() },
                    new PropositionContract { Id = Guid.NewGuid() }
                }
            };

            _quoateDistributionContract = new SaveQuotaDistributionContract
            {
                AccountId = Guid.NewGuid(),
                ProductIds = new Guid[] { _productId },
                QuotaAmounts = new SaveQuotaDistributionAmountContract[]
                {
                    new SaveQuotaDistributionAmountContract
                    {
                        Amount = 101,
                        CommercialOfferDefinitionCode = "SMS",
                        Remove = false
                    }
                },
            };

            _propositionsInfo = new List<PropositionInfoModel>
            {
                new PropositionInfoModel
                {
                    CommercialOfferDefinitions = new List<Domain.ApiModelProviders.Internal.Balance.CommercialOfferDefinition>
                    {
                        new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB06", ServiceTypeCode = "DATA", SubscriptionTypeCode = "SHB" },
                        new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB05", ServiceTypeCode = "VOICE", SubscriptionTypeCode = "SHB" },
                        new CommercialOfferDefinition { CommercialOfferDefinitionCode = "SHB04", ServiceTypeCode = "SMS", SubscriptionTypeCode = "SHB" }
                    }
                }
            };

            _productContract = new ProductContract { AccountId = _businessUnitId };
            _commercialOfferPropositionContract = new CommercialOfferPropositionContract() { Code = "FakeCOPCode" };

            _mockBalanceService = new Mock<BalanceService>();
            _mockTeleenaServices.SetupGet(m => m.BalanceService).Returns(_mockBalanceService.Object);

            _mockTeleenaServices.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitAsync(It.IsAny<Guid>())).ReturnsAsync(_propositionsContract);
            _mockTeleenaServices.Setup(x => x.PropositionService.GetEndUserSubscriptionInfoAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(_allowedPropositioContract);
            _mockTeleenaServices.Setup(x => x.QuotaDistributionService.SaveQuotaDistributionAsync(It.IsAny<SaveQuotaDistributionContract>())).ReturnsAsync(_saveQuoateDistributionresult);
            _mockTeleenaServices.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>())).ReturnsAsync(_productContract);
            _mockTeleenaServices.Setup(x => x.CommercialOfferService.GetCommercailOfferPropositionForAsync(It.IsAny<string>())).ReturnsAsync(_commercialOfferPropositionContract);

            mockConfig = new Mock<IServiceTypeConfigurationProvider>();
            mockConfig.Setup(x => x.CurrencyUnitNames).Returns(new Dictionary<string, string>() { { "$", "DOLLARS" }, { "€", "EUROS" } });
            mockConfig.Setup(x => x.DataCodes).Returns(new List<string>() { "KB", "MB", "GB", "TB" });
            mockConfig.Setup(x => x.GeneralCacheCodes).Returns(new List<string>() { "EUROS", "POUNDS", "DOLLARS", "ZLOTYCH", "NONE" });
            mockConfig.Setup(x => x.SmsCodes).Returns(new List<string>() { "UNIT", "SMS", "MINUTESMS" });
            mockConfig.Setup(x => x.VoiceCodes).Returns(new List<string>() { "SECOND", "MINUTE", "HOUR", "DAY", "WEEK", "MONTH", "YEAR" });
            mockConfig.Setup(x => x.VoiceUnitNames).Returns(new Dictionary<string, string>() { { "min", "MINUTE" }, { "h", "HOUR" }, { "d", "DAY" } });

            _mockTranslators.Setup(x => x.SaveQuotaDistributionContactTranslator.Translate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<PropositionInfoModel>(), It.IsAny<SetBalanceModel>())).Returns(_quoateDistributionContract);
            _mockTranslators.Setup(x => x.PropositionInfoModelTranslator.Translate(It.IsAny<PropositionsContract>())).Returns(_propositionsInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void When_BusinessUnitId_Is_Empty_Should_Throw_Exception()
        {
            var provider = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);
            var balances = provider.GetBalancesAsync(Guid.Empty).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void When_BalanceService_Returns_Array_Should_Return_Success()
        {
            var mockBalanceTranslator = new Mock<IBalanceTranslator>();
            mockBalanceTranslator.Setup(m => m.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>())).Returns(new BalancesResponseModel { Balances = new System.Collections.Generic.List<BalanceModel> { new BalanceModel { Amount = 100 } } });

            _mockBalanceService.Setup(m => m.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>())).Returns(Task.FromResult(new AccountBalanceWithBucketsContract[] { new AccountBalanceWithBucketsContract { Amount = 100 } }));

            _mockTranslators.SetupGet(m => m.BalanceTranslator).Returns(mockBalanceTranslator.Object);

            var provider = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);
            var balances = provider.GetBalancesAsync(Guid.NewGuid()).GetAwaiter().GetResult();

            Assert.IsTrue(balances.Result.Balances.Count > 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetBalanceAsync_WhenRequestIsNull_ShouldThrowNullArgumentException()
        {
            SetBalanceModel model = null;

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void SetBalanceAsync_ShouldCall_PropositionService_GetActivePropositionsByBusinessUnitAsync()
        {
            SetBalanceModel model = new SetBalanceModel();

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.PropositionService.GetActivePropositionsByBusinessUnitAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void SetBalanceAsync_ShouldCall_PropositionService_GetEndUserSubscriptionInfoAsync()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.PropositionService.GetEndUserSubscriptionInfoAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void SetBalanceAsync_ShouldCall_SaveQuotaDistributionContactTranslator()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTranslators.Verify(x => x.SaveQuotaDistributionContactTranslator.Translate(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<PropositionInfoModel>(), It.IsAny<SetBalanceModel>()), Times.Once);
        }

        [TestMethod]
        public void SetBalanceAsync_ShouldCall_QuotaDistributionService_SaveQuotaDistributionAsync()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.QuotaDistributionService.SaveQuotaDistributionAsync(It.IsAny<SaveQuotaDistributionContract>()), Times.Once);
        }

        [TestMethod]
        public void SetSharedBalanceAsync_ShouldReturn_BadRequest_IfProductDoesNotBelongToBusinessUnit()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, Guid.NewGuid(), _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(response.HttpResponseCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void SetSharedBalance_ShouldCall_ProductService_GetPoductById()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, Guid.NewGuid(), _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void SetSharedBalance_ShouldCall_PropositionInfoModelTranslator()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTranslators.Verify(x => x.PropositionInfoModelTranslator.Translate(It.IsAny<PropositionsContract>()), Times.Once);
        }

        [TestMethod]
        public void SetSharedBalance_ShouldRetrunBaRequest_IfThereAreNoSHBpropositions()
        {
            _mockTranslators.Setup(x => x.PropositionInfoModelTranslator.Translate(It.IsAny<PropositionsContract>()))
                .Returns(new List<PropositionInfoModel>
                {
                    new PropositionInfoModel
                    {
                        CommercialOfferDefinitions = new List<CommercialOfferDefinition>
                        {
                            new CommercialOfferDefinition { SubscriptionTypeCode = "NOTSHB", CommercialOfferDefinitionCode = "PPU01", ServiceTypeCode = "DATA" },
                            new CommercialOfferDefinition { SubscriptionTypeCode = "EUS", CommercialOfferDefinitionCode = "EUS01", ServiceTypeCode = "SMS" }
                        }
                          
                    }
                });

            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
        }

        [TestMethod]
        public void SetSharedBalance_ShouldRetrunBaRequest_IfThereAreSHBpropositions_ButNotEusTrue()
        {
            AllowedPropositionContract eusPropositions = new AllowedPropositionContract { EndUserSubscription = false, PropositionId = Guid.NewGuid() };

            _mockTeleenaServices.Setup(x => x.PropositionService.GetEndUserSubscriptionInfoAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(eusPropositions);

            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.DATA
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
        }

        [TestMethod]
        public void SetBalanceAsync_ShouldCall_CommercialOfferService_SaveQuotaDistributionAsync()
        {
            SetBalanceModel model = new SetBalanceModel
            {
                ServiceTypeCode = BalanceConstants.ServiceType.QUOTA,
                UnitTypeValue = Domain.Models.Enums.BusinessUnitsEnums.UnitType.MONETARY,
                Amount = 40
            };

            var providerUnderTest = new BalanceProvider(_mockTeleenaServices.Object, _mockTranslators.Object, mockLogger.Object);

            var response = providerUnderTest.SetBalanceAsync(model, _businessUnitId, _productId, _requestID).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.CommercialOfferService.GetCommercailOfferPropositionForAsync(It.IsAny<String>()), Times.Once);
        }
    }
}
