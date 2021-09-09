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
    public class SingleBusinessUnitLoadingStrategyUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockServiceUnitOfWork;
        private List<AccountContract> _serviceResponse;
        private GetBusinessUnitRequest _request;

        private SingleBusinessUnitLoadingStrategy _strategyUnderTest;

        [TestInitialize]
        public void SetupEachTest()
        {
            _serviceResponse = new List<AccountContract>();
            _request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = Guid.NewGuid(),
                IncludeChildren = false
            };

            _mockServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();
            _mockServiceUnitOfWork.Setup(x =>
                x.AccountService.GetAccountsExtendedDataByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_serviceResponse);

            _strategyUnderTest = new SingleBusinessUnitLoadingStrategy();
        }

        [TestMethod]
        public void CanHandleRequest_FilterBusinessUnitIdAndIncludeChildrenAreSet_ReturnsTrue()
        {
            Assert.IsTrue(_strategyUnderTest.CanHandleRequest(_request));
        }

        [TestMethod]
        public void CanHandleRequest_FilterBusinessUnitIdIsNotSet_ReturnsFalse()
        {
            _request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = Guid.Empty
            };

            Assert.IsFalse(_strategyUnderTest.CanHandleRequest(_request));
        }

        [TestMethod]
        public void CanHandleRequest_IncludeChildrenTrue_ReturnsFalse()
        {
            _request = new GetBusinessUnitRequest()
            {
                IncludeChildren = true
            };

            Assert.IsFalse(_strategyUnderTest.CanHandleRequest(_request));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadBusinessUnitsAsync_InputIsNull_ThrowsException()
        {
            _strategyUnderTest.LoadBusinessUnitsAsync(null, null).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void LoadBusinessUnitsAsync_DependenciesCalledCorrectly()
        {
            var result = _strategyUnderTest.LoadBusinessUnitsAsync(_request, _mockServiceUnitOfWork.Object)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(result);
            _mockServiceUnitOfWork.Verify(x =>
                x.AccountService.GetAccountsExtendedDataByIdAsync(It.Is<Guid>(
                    accountId => accountId == _request.FilterBusinessUnitId)), Times.Once);
        }
    }
}
