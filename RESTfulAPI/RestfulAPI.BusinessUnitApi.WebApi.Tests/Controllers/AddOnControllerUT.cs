using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.AccessProvider.Configuration;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class AddOnControllerUT
    {
        private Mock<IAddOnProvider> mockAddOnProvider;
        private ProviderOperationResult<object> addAllowedAddOnsResponse;
        private Guid addOnIdNull = Guid.NewGuid();
        private AddOnCumulativeListModel addOnsResponse = null;

        [TestInitialize]
        public void SetupEachTest()
        {
            addAllowedAddOnsResponse = ProviderOperationResult<object>.OkResult();

            mockAddOnProvider = new Mock<IAddOnProvider>();
            mockAddOnProvider.Setup(x => x.AddAddOnAsync(It.IsAny<PurchaseAddOnModel>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<object>.OkResult());
            mockAddOnProvider.Setup(x => x.AddAllowedAddOnsToBusinessUnit(It.IsAny<List<Guid>>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(addAllowedAddOnsResponse);
            mockAddOnProvider.Setup(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>()))
                .ReturnsAsync(new  AddOnCumulativeListModel() { AddOns = new List<AddOnCumulativeModel>() });
            mockAddOnProvider.Setup(x => x.DeleteAddOnAsync(It.IsAny<DeleteAddOnModel>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<object>.AcceptedResult());
            mockAddOnProvider.Setup(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>()))
                .ReturnsAsync(new AddOnCumulativeListModel { AddOns = new List<AddOnCumulativeModel>() });
            mockAddOnProvider.Setup(x => x.GetAddOnsAsync(It.Is<GetPurchasedAddonsBusinessUnitRequest>(b => b.BusinessUnitId == addOnIdNull)))
                .ReturnsAsync(addOnsResponse);
        }

        private static void SetupHiddenControllerDependencies(BaseApiController controller)
        {
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Route(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("some/route");
            mockUrlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns("http://domain.com:3265/some/route");
            controller.Url = mockUrlHelper.Object;
            var claimsIdentity = new ClaimsIdentity(new Claim[] { new Claim("CrmCompanyId", Guid.NewGuid().ToString()) });
            controller.User = new ClaimsPrincipal(claimsIdentity);

            var mockAccessProviderConfiguration = new Mock<IAccessProviderConfiguration>();
            var mockLogger = new Mock<IJsonRestApiLogger>();
            mockAccessProviderConfiguration.SetupGet(m => m.UseAccessProvider).Returns(false);
            controller.Logger = mockLogger.Object;
            controller.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties[RequestAndResponseIdConstants.RequestidHeaderName] = Guid.NewGuid();
        }

        [TestMethod]
        public void PostAddOn_ShouldCallAddOnProvider()
        {
            var controllerMocked = new AddOnController(mockAddOnProvider.Object);
            SetupHiddenControllerDependencies(controllerMocked);

            var response = controllerMocked.Post(new PurchaseAddOnModel { AddOnId = Guid.NewGuid() }, Guid.NewGuid()).GetAwaiter().GetResult();

            mockAddOnProvider.Verify(x => x.AddAddOnAsync(It.IsAny<PurchaseAddOnModel>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void PostAddOn_ShouldNotCallAddOnProvider()
        {
            var controllerMocked = new AddOnController(mockAddOnProvider.Object);
            controllerMocked.ModelState.AddModelError("error", "some error");

            var response = controllerMocked.Post(new PurchaseAddOnModel { AddOnId = Guid.NewGuid() }, Guid.NewGuid()).GetAwaiter().GetResult();

            mockAddOnProvider.Verify(x => x.AddAddOnAsync(It.IsAny<PurchaseAddOnModel>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Get_ShouldReturnOkResult()
        {
            var controller = new AddOnController(mockAddOnProvider.Object);

            var response = controller.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<AddOnCumulativeListModel>));
            mockAddOnProvider.Verify(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        public void Get_ShouldReturnNotFoundResult()
        {
            AddOnCumulativeListModel providerResponse = null;
            mockAddOnProvider.Setup(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>())).ReturnsAsync(providerResponse);
            var controller = new AddOnController(mockAddOnProvider.Object);

            var response = controller.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<StatusMessageModel>));
            mockAddOnProvider.Verify(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        public void Get_ShouldModelStateError()
        {
            var controller = new AddOnController(mockAddOnProvider.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("error", "error");

            var response = controller.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
            mockAddOnProvider.Verify(x => x.GetAddOnsAsync(It.IsAny<GetPurchasedAddonsBusinessUnitRequest>()), Times.Never);
        }

        [TestMethod]
        public void Delete_ShouldReturnAcceptedIfSuccessful()
        {
            var controller = new AddOnController(mockAddOnProvider.Object);
            DeleteAddOnModel request = new DeleteAddOnModel();
            SetupHiddenControllerDependencies(controller);

            var response = controller.Delete(request, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<object>));
            mockAddOnProvider.Verify(x => x.DeleteAddOnAsync(It.IsAny<DeleteAddOnModel>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Delete_ShouldReturnInvalidModelStateError()
        {
            var controller = new AddOnController(mockAddOnProvider.Object);
            controller.ModelState.Clear();
            controller.ModelState.AddModelError("error", "error");
            DeleteAddOnModel request = new DeleteAddOnModel();

            var response = controller.Delete(request, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
            mockAddOnProvider.Verify(x => x.DeleteAddOnAsync(It.IsAny<DeleteAddOnModel>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}
