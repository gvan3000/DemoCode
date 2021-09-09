using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.AccessProvider.Configuration;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class BalanceControllerUT
    {
        private Mock<IBalanceProvider> balanceProviderMock;
        private SetBalanceModel setBalanceModel;
        private Mock<IAccessProviderConfiguration> accessProviderConfigurationMock;
        private Mock<IQuotaDistributionProvider> quotaDistributionProviderMock;
        private Mock<IJsonRestApiLogger> loggerMock;

        [TestInitialize]
        public void SetUp()
        {
            balanceProviderMock = new Mock<IBalanceProvider>();
            balanceProviderMock
                .Setup(x => x.SetBalanceAsync(It.IsAny<SetBalanceModel>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<object>.AcceptedResult());

            quotaDistributionProviderMock = new Mock<IQuotaDistributionProvider>();
            quotaDistributionProviderMock.Setup(x => x.GetSharedBalancesForProductAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<BalanceQuotasListModel>.OkResult(new BalanceQuotasListModel()));

            quotaDistributionProviderMock.Setup(x => x.GetAllSharedBalancesPerBusinessUnitAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<ProductAllowedBalanceList>.OkResult(new ProductAllowedBalanceList { ProductAllowedBalances = new List<ProductAllowedBalanceModel>()}));
        }

        private void SetUpHiddenControllerDependencies(BaseApiController controller)
        {
            loggerMock = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);
            accessProviderConfigurationMock = new Mock<IAccessProviderConfiguration>();
            accessProviderConfigurationMock.Setup(x => x.UseAccessProvider).Returns(false);

            controller.AccessProviderConfiguration = accessProviderConfigurationMock.Object;
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Logger = loggerMock.Object;
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties[RequestAndResponseIdConstants.RequestidHeaderName] = Guid.NewGuid();
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            setBalanceModel = new SetBalanceModel { Amount = 123.6M, UnitTypeValue = Domain.Models.Enums.BusinessUnitsEnums.UnitType.GB, ServiceTypeCode = BalanceConstants.ServiceType.DATA };
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            controllerUnderTest.ModelState.Clear();
            controllerUnderTest.ModelState.AddModelError("request", "error 121");

            var response = controllerUnderTest.Post(Guid.NewGuid(), Guid.NewGuid(), setBalanceModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Post_ShouldCall_BalanceProvider_SetBalanceAsync()
        {
            setBalanceModel = new SetBalanceModel { Amount = 123.6M, UnitTypeValue = Domain.Models.Enums.BusinessUnitsEnums.UnitType.GB, ServiceTypeCode = BalanceConstants.ServiceType.DATA };
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            SetUpHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.Post(Guid.NewGuid(), Guid.NewGuid(), setBalanceModel).ConfigureAwait(false).GetAwaiter().GetResult();

            balanceProviderMock.Verify(x => x.SetBalanceAsync(It.IsAny<SetBalanceModel>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Post_ShouldCallReturn_ProviderOperationresultAccepted()
        {
            setBalanceModel = new SetBalanceModel { Amount = 123.6M, UnitTypeValue = Domain.Models.Enums.BusinessUnitsEnums.UnitType.GB, ServiceTypeCode = BalanceConstants.ServiceType.DATA };
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            SetUpHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.Post(Guid.NewGuid(), Guid.NewGuid(), setBalanceModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<object>));
            Assert.AreEqual(HttpStatusCode.Accepted, ((NegotiatedContentResult<object>)response).StatusCode);
        }

        [TestMethod]
        public void GetSharedBalancesByProduct_ShouldReturnOkResult()
        {
            BalanceController controller = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            SetUpHiddenControllerDependencies(controller);

            var response = controller.Get(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<BalanceQuotasListModel>));
            Assert.AreEqual(HttpStatusCode.OK, ((NegotiatedContentResult<BalanceQuotasListModel>)response).StatusCode);
        }

        [TestMethod]
        public void GetAllShared_ShouldReturnBadrequest_IfModelStateError()
        {
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            controllerUnderTest.ModelState.AddModelError("someKey", "some error");

            var response = controllerUnderTest.GetAllShared(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetAllShared_Call_QuotaDistributionProvider_GetAllSharedBalancePerBalanceAsync()
        {
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);

            var response = controllerUnderTest.GetAllShared(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            quotaDistributionProviderMock.Verify(x => x.GetAllSharedBalancesPerBusinessUnitAsync(It.IsAny<Guid>()), Times.Once);
        }


        [TestMethod]
        public void Post_ShouldCall_BalanceProvider_SetBalanceAsync_If_Service_Quota_Unit_Monetary()
        {
            setBalanceModel = new SetBalanceModel { Amount = 12.3M, UnitTypeValue = Domain.Models.Enums.BusinessUnitsEnums.UnitType.MONETARY, ServiceTypeCode = BalanceConstants.ServiceType.QUOTA };
            var controllerUnderTest = new BalanceController(balanceProviderMock.Object, quotaDistributionProviderMock.Object);
            SetUpHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.Post(Guid.NewGuid(), Guid.NewGuid(), setBalanceModel).ConfigureAwait(false).GetAwaiter().GetResult();

            balanceProviderMock.Verify(x => x.SetBalanceAsync(It.IsAny<SetBalanceModel>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }
    }
}
