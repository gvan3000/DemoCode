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
    public class CompanyIdBusinessUnitLoadingStrategyUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockServiceUnitOfWork;
        private List<AccountContract> _returnedBusinessUnits;

        private CompanyIdBusinessUnitLoadingStrategy _strategyUnderTest;

        [TestInitialize]
        public void SetupEachTest()
        {
            _returnedBusinessUnits = new List<AccountContract>();

            _mockServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();
            _mockServiceUnitOfWork.Setup(x => x.AccountService.GetAccountsByCompanyAsync(It.IsAny<GetAccountsByCompanyContract>()))
                .ReturnsAsync(_returnedBusinessUnits);

            _strategyUnderTest = new CompanyIdBusinessUnitLoadingStrategy();
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturnTrueWhenUserCompanyIdIsSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid()
            };

            Assert.IsTrue(_strategyUnderTest.CanHandleRequest(request));
        }

        [TestMethod]
        public void CanHandleRequest_ShouldReturnFalseWhenUserCompanyIdIsNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.Empty
            };

            Assert.IsFalse(_strategyUnderTest.CanHandleRequest(request));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadBusinessUnitsAsync_ShouldThrowForNullRequest()
        {
            _strategyUnderTest.LoadBusinessUnitsAsync(null, null).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void LoadBusinessUnitsAsync_ShouldCallAppropriateAccountServiceOnce()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid()
            };

            var result = _strategyUnderTest.LoadBusinessUnitsAsync(request, _mockServiceUnitOfWork.Object)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(result);
            _mockServiceUnitOfWork.Verify(x => x.AccountService.GetAccountsByCompanyAsync(It.IsAny<GetAccountsByCompanyContract>()), Times.Once);
        }

        [TestMethod]
        public void LoadBusinessUnitAsync_ShouldPassSuppliedCompanyIdToAccountService()
        {
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid()
            };

            var result = _strategyUnderTest.LoadBusinessUnitsAsync(request, _mockServiceUnitOfWork.Object)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(result);
            _mockServiceUnitOfWork.Verify(x => x.AccountService.GetAccountsByCompanyAsync(It.Is<GetAccountsByCompanyContract>(contract => contract.CompanyId == request.UserCompanyId)));
        }
    }
}
