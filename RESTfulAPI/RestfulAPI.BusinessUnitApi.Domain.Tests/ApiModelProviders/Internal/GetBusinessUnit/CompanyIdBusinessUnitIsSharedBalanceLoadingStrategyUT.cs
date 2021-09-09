using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.GetBusinessUnit
{
    [TestClass]
    public class CompanyIdBusinessUnitIsSharedBalanceLoadingStrategyUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockServiceUnitOfWork;
        private List<AccountContract> _returnedBusinessUnits;

        private CompanyIdBusinessUnitIsSharedBalanceLoadingStrategy _strategyUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            _returnedBusinessUnits = new List<AccountContract>();
            _mockServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();
            _mockServiceUnitOfWork.Setup(x => x.AccountService.GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(It.IsAny<AccountRequest>()))
                .ReturnsAsync(_returnedBusinessUnits);

            _strategyUnderTest = new CompanyIdBusinessUnitIsSharedBalanceLoadingStrategy();
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturn_True_WhenUserCompanyIdAndFilterHasSharedWalletIsSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid(),
                FilterHasSharedWallet = "true"
            };

            var result = _strategyUnderTest.CanHandleRequest(request);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturn_False_WhenUserCompanyIdIsSetAndFilterHasSharedWalletNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid()
            };

            var result = _strategyUnderTest.CanHandleRequest(request);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadBusinessUnitsAsync_ShouldThrowForNullRequest()
        {
            _strategyUnderTest.LoadBusinessUnitsAsync(null, null).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void LoadBusinessUnitsAsync_ShouldCallAccountService_Once()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterHasSharedWallet = "true",
                UserBusinessUnitId = Guid.NewGuid()
            };

            var result = _strategyUnderTest.LoadBusinessUnitsAsync(request, _mockServiceUnitOfWork.Object)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(result);
            _mockServiceUnitOfWork.Verify(x => x.AccountService.GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(It.IsAny<AccountRequest>()), Times.Once);
        }
    }
}
