using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels;
using RestfulAPI.Common;
using System;
using System.Net;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{


    [TestClass]
    public class AvailableSIMControllerUnitTests
    {
        private Mock<ISimProvider> _simProviderMock;

        [TestInitialize]
        public void Setup()
        {
            _simProviderMock = new Mock<ISimProvider>();
            _simProviderMock.Setup(x => x.GetAvailableSIMsAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(ProviderOperationResult<AvailableSIMResponseModel>.OkResult(new AvailableSIMResponseModel()));
            _simProviderMock.Setup(x => x.GetAvailableSIMsAsync(It.IsAny<AvailableSimProviderRequest>()))
                .ReturnsAsync(ProviderOperationResult<AvailableSimResponseV2Model>.OkResult(new AvailableSimResponseV2Model()));
        }

        [TestMethod]
        public void Get_ForValidIncomeParams_ShouldReturnResult()
        {
            var controller = new AvailableSIMController(_simProviderMock.Object);

            var response = controller.Get(Guid.NewGuid(), "status:twtwe").ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableSIMResponseModel>));
        }

        [TestMethod]
        public void Get_ForEmptyStatusParam_ShouldReturnModelStateError()
        {
            var controller = new AvailableSIMController(_simProviderMock.Object);

            var response = controller.Get(Guid.NewGuid(), string.Empty).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_ForIncorrectStatus_ShouldReturnStatusNotFound()
        {
            _simProviderMock.Setup(x => x.GetAvailableSIMsAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(ProviderOperationResult<AvailableSIMResponseModel>.NotFoundResult(string.Empty));
            var controller = new AvailableSIMController(_simProviderMock.Object);

            var response = controller.Get(Guid.NewGuid(), "status:sdfsd").ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<StatusMessageModel>));
            Assert.AreEqual(HttpStatusCode.NotFound, ((NegotiatedContentResult<StatusMessageModel>)response).StatusCode);
        }

        [TestMethod]
        public void GetV2_ForValidIncomeParams_ShouldReturnResult()
        {
            var controller = new AvailableSIMController(_simProviderMock.Object);

            var response = controller.GetV2(Guid.NewGuid(), "status", 10, 15).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableSimResponseV2Model>));
        }

        [TestMethod]
        public void GetV2_ForGetAvailabeSIMsAsyncReturnedNotFoundResult_ShouldReturnStatusNotFound()
        {
            _simProviderMock.Setup(x => x.GetAvailableSIMsAsync(It.IsAny<AvailableSimProviderRequest>()))
                .ReturnsAsync(ProviderOperationResult<AvailableSimResponseV2Model>.NotFoundResult(string.Empty));
            var controller = new AvailableSIMController(_simProviderMock.Object);

            var response = controller.GetV2(Guid.NewGuid(), "status", 10, 15).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<StatusMessageModel>));
            Assert.AreEqual(HttpStatusCode.NotFound, ((NegotiatedContentResult<StatusMessageModel>)response).StatusCode);
        }
    }
}
