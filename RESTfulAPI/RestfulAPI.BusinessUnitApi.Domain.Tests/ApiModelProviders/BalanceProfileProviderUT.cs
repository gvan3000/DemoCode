using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;
using System.ServiceModel;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class BalanceProfileProviderUT
    {
        private Mock<IJsonRestApiLogger> mockLogger;
        private Mock<ITeleenaServiceUnitOfWork> mockServices;
        private Mock<IBusinessUnitApiTranslators> mockTranslators;

        [TestInitialize]
        public void SetupEachTest()
        {
            mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);

            mockTranslators = new Mock<IBusinessUnitApiTranslators>(MockBehavior.Strict);
            mockTranslators.Setup(x => x.BalanceProfileListTranslator.Translate(It.IsAny<SysCodeContract[]>()))
                .Returns(new Domain.Models.BalanceProfileModels.BalanceProfileListModel());

            mockServices = new Mock<ITeleenaServiceUnitOfWork>();
            mockServices.Setup(x => x.BalanceService.GetBalanceProfilesPerCompanyAsync(It.IsAny<GetBalanceProfilesPerCompanyContract>()))
                .ReturnsAsync(new SysCodeContract[0]);
            mockServices.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new TeleenaServiceReferences.AccountService.AccountContract());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetBalanceProfilesAsync_ShouldThrowWhenBusinessUnitIdIsEmptyGuid()
        {
            var providerUnderTest = new BalanceProfileProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object);

            var input = Guid.Empty;

            var result = providerUnderTest.GetBalanceProfilesAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetBalanceProfilesAsync_ShouldReturnFailureWhenAccountServicecDoesNotFindBusinessUnitById()
        {
            mockServices.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(default(TeleenaServiceReferences.AccountService.AccountContract));

            var providerUnderTest = new BalanceProfileProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object);

            var input = Guid.NewGuid();

            var result = providerUnderTest.GetBalanceProfilesAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void GetBalanceProfilesAsync_ShouldReturnFailureWhenBalanceServiceFaults()
        {
            mockServices.Setup(x => x.BalanceService.GetBalanceProfilesPerCompanyAsync(It.IsAny<GetBalanceProfilesPerCompanyContract>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp()));

            var providerUnderTest = new BalanceProfileProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object);

            var input = Guid.NewGuid();

            var result = providerUnderTest.GetBalanceProfilesAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void GetBalanceProfilesAsync_ShouldCallTwoServiceMethodsAndTranslatorAndReturnSuccess()
        {
            var providerUnderTest = new BalanceProfileProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object);

            var input = Guid.NewGuid();

            var result = providerUnderTest.GetBalanceProfilesAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Result);

            mockServices.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Once);
            mockServices.Verify(x => x.BalanceService.GetBalanceProfilesPerCompanyAsync(It.IsAny<GetBalanceProfilesPerCompanyContract>()), Times.Once);

            mockTranslators.Verify(x => x.BalanceProfileListTranslator.Translate(It.IsAny<SysCodeContract[]>()), Times.Once);
        }
    }
}
