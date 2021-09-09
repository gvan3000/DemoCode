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
    public class ParentBusinessUnitLoadingStrategyUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockServiceUnitOfWork;
        private List<AccountContract> _serviceResponse;

        private ParentBusinessUnitLoadingStrategy _strategyUnderTest = new ParentBusinessUnitLoadingStrategy();

        [TestInitialize]
        public void SetupEachTest()
        {
            _serviceResponse = new List<AccountContract>();

            _mockServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();
            _mockServiceUnitOfWork.Setup(x => 
                x.AccountService.GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(It.IsAny<AccountRequest>()))
                .ReturnsAsync(_serviceResponse);

            _strategyUnderTest = new ParentBusinessUnitLoadingStrategy();
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturnTrueWhenUserAccountIdIsSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserBusinessUnitId = Guid.NewGuid()
            };

            Assert.IsTrue(_strategyUnderTest.CanHandleRequest(request));
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturnFalseWhenUserAccountIdIsNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserBusinessUnitId = Guid.Empty
            };

            Assert.IsFalse(_strategyUnderTest.CanHandleRequest(request));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadBusinessUnitsAsync_ShouldThrowWhenInputIsNull()
        {
            _strategyUnderTest.LoadBusinessUnitsAsync(null, null).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void LoadBusinessUnitsAsync_ShouldSupplyRequestUserAccountIdToService()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserBusinessUnitId = Guid.NewGuid()
            };

            var result = _strategyUnderTest.LoadBusinessUnitsAsync(request, _mockServiceUnitOfWork.Object)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(result);
            _mockServiceUnitOfWork.Verify(x =>
                x.AccountService.GetAccountWithChildAccountsIsSharedWalletAndEndUserSubscriptionAsync(It.Is<AccountRequest>(
                    accountRequest => accountRequest.AccountId == request.UserBusinessUnitId)), Times.Once);
        }
    }
}
