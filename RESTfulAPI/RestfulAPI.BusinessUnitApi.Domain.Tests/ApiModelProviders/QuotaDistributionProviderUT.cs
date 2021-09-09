using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class QuotaDistributionProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockTeleenaServiceUnitOfWork;
        private Mock<IBusinessUnitApiTranslators> _mockTranslators;
        private Mock<IJsonRestApiLogger> _mockLogger;
        private BalanceQuotasListModel _balanceQuotaListModel;

        private Guid _businessUnitIdShared;
        private Guid _businessUnitIdNotShared;
        private Guid _businessUnitIdSharedNoData;
        private BalanceQuotasListModel _overridenBalances;

        [TestInitialize]
        public void Setup()
        {
            _mockTeleenaServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();

            _mockTranslators = new Mock<IBusinessUnitApiTranslators>();

            _mockLogger = new Mock<IJsonRestApiLogger>();

            _businessUnitIdNotShared = Guid.NewGuid();
            _businessUnitIdShared = Guid.NewGuid();
            _businessUnitIdSharedNoData = Guid.NewGuid();

            _balanceQuotaListModel = GetBalanceQuotasListModelMockResult();
            _overridenBalances = GetBalanceQuotasListModelMockResult();
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_ShouldReturnResult()
        {
            Guid accountId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductQuotaDistributionContract());
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    IsSharedWallet = true
                });

            var mockedABWBCResultArray = GetAccountBalanceWithBucketsContractMockResult();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()))
                .ReturnsAsync(mockedABWBCResultArray);

            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(new Domain.Models.BalanceModels.BalanceQuotasListModel());
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);
            _mockTranslators.Setup(x => x.BalancesContractBalanceQuotaListModelTranslator.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>()))
                .Returns(_balanceQuotaListModel);

            var response = provider.GetSharedBalancesForProductAsync(accountId, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_ShouldReturnNotFoundResult()
        {
            Guid accountId = Guid.NewGuid();

            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProductQuotaDistributionContract)null);
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    IsSharedWallet = true
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()))
                .ReturnsAsync(default(AccountBalanceWithBucketsContract[]));

            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(new Domain.Models.BalanceModels.BalanceQuotasListModel());
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);
            _mockTranslators.Setup(x => x.BalancesContractBalanceQuotaListModelTranslator.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>()))
                .Returns(default(BalanceQuotasListModel));

            var response = provider.GetSharedBalancesForProductAsync(accountId, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpResponseCode);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_ShouldReturnInvalidInput()
        {
            Guid accountId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ProductQuotaDistributionContract)null);
            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(new Domain.Models.BalanceModels.BalanceQuotasListModel());
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = provider.GetSharedBalancesForProductAsync(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Never);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockTranslators.Verify(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()), Times.Never);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Never);
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_ShouldReturnInvalidInputIfBusinessUnitIsNotShared()
        {
            Guid accountId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductQuotaDistributionContract());
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    IsSharedWallet = false
                });
            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(new Domain.Models.BalanceModels.BalanceQuotasListModel());
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = provider.GetSharedBalancesForProductAsync(accountId, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Never);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockTranslators.Verify(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()), Times.Never);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Never);
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_IsCappedShouldBeTrue()
        {
            Guid accountId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductQuotaDistributionContract());
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    IsSharedWallet = true
                });

            var mockedABWBCResultArray = GetAccountBalanceWithBucketsContractMockResult();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()))
                .ReturnsAsync(mockedABWBCResultArray);

            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(_overridenBalances);
            _mockTranslators.Setup(x => x.BalancesContractBalanceQuotaListModelTranslator.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>()))
                .Returns(_balanceQuotaListModel);
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = provider.GetSharedBalancesForProductAsync(accountId, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response, "Response is null.");
            Assert.IsTrue(response.IsSuccess, "Response is not success.");
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode, $"HttpResponseCode is {response.HttpResponseCode}");
            Assert.IsTrue(response.Result.BalanceAllowances.Any(x => x.IsCapped), "IsCapped is false");
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockTranslators.Verify(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetSharedBalancesForProductAsync_IsCappedShouldBeFalse()
        {
            Guid accountId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.ProductService.GetProductByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.ProductService.ProductContract()
                {
                    AccountId = accountId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProductQuotaDistributionContract());
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    IsSharedWallet = true
                });

            var mockedABWBCResultArray = GetAccountBalanceWithBucketsContractMockResult();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()))
                .ReturnsAsync(mockedABWBCResultArray);

            _mockTranslators.Setup(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()))
                .Returns(new BalanceQuotasListModel());
            _mockTranslators.Setup(x => x.BalancesContractBalanceQuotaListModelTranslator.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>()))
                .Returns(_balanceQuotaListModel);
            QuotaDistributionProvider provider = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = provider.GetSharedBalancesForProductAsync(accountId, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response, "Response is null.");
            Assert.IsTrue(response.IsSuccess, "Response is not success.");
            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode, $"HttpResponseCode is {response.HttpResponseCode}");
            Assert.IsTrue(response.Result.BalanceAllowances.All(x => !x.IsCapped), "IsCapped is true");
            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForProductAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockTranslators.Verify(x => x.ProductQuotaDistributionContractTranslator.Translate(It.IsAny<ProductQuotaDistributionContract>()), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.BalanceService.GetAccountBalancesAsync(It.IsAny<AccountBalanceRequest>()), Times.Once);
        }

        private AccountBalanceWithBucketsContract[] GetAccountBalanceWithBucketsContractMockResult()
        {
            var oneBucket = new CompanyBalanceBucketContract()
            {
                InitialAmount = 1.1M
            };
            var bucketsArray = new CompanyBalanceBucketContract[1];
            bucketsArray[0] = oneBucket;

            var oneAccountBalanceWithBucketsContract = new AccountBalanceWithBucketsContract()
            {
                Buckets = bucketsArray,
                BalanceType = "DATA",
                BalanceTypeId = Guid.NewGuid()
            };

            var mockedABWBCResultArray = new AccountBalanceWithBucketsContract[1];
            mockedABWBCResultArray[0] = oneAccountBalanceWithBucketsContract;

            return mockedABWBCResultArray;
        }

        private BalanceQuotasListModel GetBalanceQuotasListModelMockResult()
        {
            var balanceQuotaModel = new BalanceQuotaModel() { Amount = 1, ServiceTypeCode = BalanceConstants.ServiceType.DATA, UnitType = "MB" };
            var balanceQuotaListModel = new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() };
            balanceQuotaListModel.BalanceAllowances.Add(balanceQuotaModel);

            return balanceQuotaListModel;
        }

        private void SetUpMock()
        {
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(b => b == _businessUnitIdShared)))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract { Id = Guid.NewGuid(), IsSharedWallet = true });

            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(b => b == _businessUnitIdNotShared)))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract { Id = Guid.NewGuid(), IsSharedWallet = false });

            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(b => b == _businessUnitIdSharedNoData)))
               .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract { Id = Guid.NewGuid(), IsSharedWallet = true });

            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForAccountAsync(It.Is<Guid>(a => a == _businessUnitIdShared)))
                .ReturnsAsync(new ProductAllowedBalancesContract[]
                {
                    new ProductAllowedBalancesContract {Amount = 100, OutstandingAmount = 1, ProductId = Guid.NewGuid(), ServiceTypeCode = "DATA", UnitType = "MB" },
                    new ProductAllowedBalancesContract {Amount = 50, OutstandingAmount = 2, ProductId = Guid.NewGuid(), ServiceTypeCode = "DATA", UnitType = "MB" }
                });

            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForAccountAsync(It.Is<Guid>(a => a == _businessUnitIdShared)))
                .ReturnsAsync(new ProductAllowedBalancesContract[]
                {
                    new ProductAllowedBalancesContract {Amount = 100, OutstandingAmount = 1, ProductId = Guid.NewGuid(), ServiceTypeCode = "DATA", UnitType = "MB" },
                    new ProductAllowedBalancesContract {Amount = 50, OutstandingAmount = 2, ProductId = Guid.NewGuid(), ServiceTypeCode = "DATA", UnitType = "MB" }
                });

            _mockTeleenaServiceUnitOfWork.Setup(x => x.QuotaDistributionService.GetSharedBalanceForAccountAsync(It.Is<Guid>(a => a == _businessUnitIdSharedNoData)))
                .ReturnsAsync(new ProductAllowedBalancesContract[] { });

            _mockTranslators.Setup(x => x.ProductAllowedBalancesTranslator.Translate(It.IsAny<List<ProductAllowedBalancesContract>>()))
                .Returns(new ProductAllowedBalanceList { ProductAllowedBalances = new List<ProductAllowedBalanceModel>() });

            _mockTranslators.Setup(x => x.BalancesContractBalanceQuotaListModelTranslator.Translate(It.IsAny<AccountBalanceWithBucketsContract[]>()))
                .Returns(new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() });
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldCall_AccountService_GetAccountByIdAsync()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdNotShared).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(acc => acc == _businessUnitIdNotShared)), Times.Once);
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldreturnBadRequest_IfAccountIsNotSharedwallet()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdNotShared).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldCallQuotaDistributionService_GetSharedBalanceForAccountAsync()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdShared).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServiceUnitOfWork.Verify(x => x.QuotaDistributionService.GetSharedBalanceForAccountAsync(It.Is<Guid>(a => a == _businessUnitIdShared)), Times.Once);
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldReturnNotFound_IfServicereturnsNullOrEmptyResult()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdSharedNoData).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpResponseCode);
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldCall_ProductAllowedBalancesTranslator()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdShared).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTranslators.Verify(x => x.ProductAllowedBalancesTranslator.Translate(It.IsAny<List<ProductAllowedBalancesContract>>()), Times.Once);
        }

        [TestMethod]
        public void GetAllSharedBalancePerBalance_ShouldReturnOkResult()
        {
            SetUpMock();
            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var response = providerUnderTest.GetAllSharedBalancesPerBusinessUnitAsync(_businessUnitIdShared).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SetBusinessUnitQuota_ShouldThrowWhenAccountIsNotSet()
        {
            var accountId = Guid.Empty;
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SetBusinessUnitQuota_ShouldThrowWhenInputModelIsNull()
        {
            var accountId = Guid.NewGuid();
            var input = default(SetBusinessUnitQuotaModel);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SetBusinessUnitQuota_ShouldThrowWhenUserIsNull()
        {
            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = default(ClaimsPrincipal);

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldCallServiceToGetAccountAndThenCompanyBalanceTypes()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "bla"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);

            _mockTeleenaServiceUnitOfWork.Verify(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(id => id == accountId)), Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(x =>
                x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.Is<GetCompanyBalanceTypeTopUpSettingByCompanyContract>(
                    contract => contract.CompanyId == companyId)),
                Times.Once);
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldReturnErrorWhenAccountIsNotFound()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(default(TeleenaServiceReferences.AccountService.AccountContract));
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "bla"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NotFound, result.HttpResponseCode);
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldReturnErrorWhenNoBalanceTypesFoundForCompany()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(default(CompanyBalanceTypeTopUpSettingContract[]));
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.HttpResponseCode);
            StringAssert.Contains(result.ErrorMessage, "not find balance types");
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldSucceedWhenMoreThanOneCompanyBalanceTypeFound()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "General Cash"
                    },
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "truc"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Accepted, result.HttpResponseCode);
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldFailWhenNoCompanyBalanceTypesIsGeneralCash()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "bla"
                    },
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "truc"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
            StringAssert.Contains(result.ErrorMessage, "not configured for company");
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldCallTranslatorToProduceActionServiceContractAndCallServiceToSetQuotaAndReturnSuccess()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            var companyBalanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "General Cash"
            };
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    companyBalanceType
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(Task.FromResult(0));

            var translatorReturnContract = new TopUpAccountRequestContract();
            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(translatorReturnContract);

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Accepted, result.HttpResponseCode);

            _mockTranslators.Verify(x => x.SetQuotaTranslator.Translate(
                                        It.Is<Guid>(id => id == accountId),
                                        It.Is<SetBusinessUnitQuotaModel>(model => model == input),
                                        It.Is<ClaimsPrincipal>(u => u == user),
                                        It.Is<CompanyBalanceTypeTopUpSettingContract>(balanceType => balanceType == companyBalanceType)),
                                    Times.Once);
            _mockTeleenaServiceUnitOfWork.Verify(
                x => x.BalanceService.CustomTopUpOnAccountAsync(
                    It.Is<TopUpAccountRequestContract>(contract => contract == translatorReturnContract)),
                Times.Once);
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldFetchPropositionsForBusinessUnitANdReturnErrorWhenNoSharedBalancePropositionUsed()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = false
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "General Cash"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(async () =>
                {
                    throw new System.ServiceModel.FaultException<TeleenaServiceReferences.BalanceService.TeleenaInnerExp>(new TeleenaServiceReferences.BalanceService.TeleenaInnerExp()
                    {
                        TicketId = Guid.NewGuid(),
                        ErrorCode = 123,
                        ErrorDescription = "bla error"
                    });
                });

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
            StringAssert.Contains(result.ErrorMessage, "shared balances");
        }

        [TestMethod]
        public async Task SetBusinessUnitQuota_ShouldReturnErrorWhenSettingQuotaFailsOnService()
        {
            var companyId = Guid.NewGuid();
            _mockTeleenaServiceUnitOfWork.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract()
                {
                    CompanyId = companyId
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.PropositionService.PropositionsContract()
                {
                    PropositionContracts = new List<TeleenaServiceReferences.PropositionService.PropositionContract>()
                    {
                        new TeleenaServiceReferences.PropositionService.PropositionContract()
                        {
                            CommercialOfferConfigurationsContract = new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationsContract()
                            {
                                CommercialOfferConfigurationContracts = new List<TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract>()
                                {
                                    new TeleenaServiceReferences.PropositionService.CommercialOfferConfigurationContract()
                                    {
                                        IsSharedWallet = true
                                    }
                                }
                            }
                        }
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(It.IsAny<GetCompanyBalanceTypeTopUpSettingByCompanyContract>()))
                .ReturnsAsync(new CompanyBalanceTypeTopUpSettingContract[]
                {
                    new CompanyBalanceTypeTopUpSettingContract()
                    {
                        BalanceTypeName = "General Cash"
                    }
                });
            _mockTeleenaServiceUnitOfWork.Setup(x => x.BalanceService.CustomTopUpOnAccountAsync(It.IsAny<TopUpAccountRequestContract>()))
                .Returns(async () =>
                {
                    throw new System.ServiceModel.FaultException<TeleenaServiceReferences.BalanceService.TeleenaInnerExp>(new TeleenaServiceReferences.BalanceService.TeleenaInnerExp()
                    {
                        TicketId = Guid.NewGuid(),
                        ErrorCode = 123,
                        ErrorDescription = "bla error"
                    });
                });

            _mockTranslators.Setup(x => x.SetQuotaTranslator.Translate(It.IsAny<Guid>(),
                                                                    It.IsAny<SetBusinessUnitQuotaModel>(),
                                                                    It.IsAny<ClaimsPrincipal>(),
                                                                    It.IsAny<CompanyBalanceTypeTopUpSettingContract>()))
                .Returns(new TopUpAccountRequestContract());

            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel() { Amount = 123 };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("blaClaimType", "blaValue") }));

            var providerUnderTest = new QuotaDistributionProvider(_mockTeleenaServiceUnitOfWork.Object, _mockTranslators.Object, _mockLogger.Object);

            var result = await providerUnderTest.SetBusinessUnitQuota(accountId, input, user);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.HttpResponseCode);
        }
    }
}


