using System;
using System.Net;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class AvailableMSISDNControllerUnitTest
    {
        private Mock<IMobileProvider> mockMobileProvider;

        [TestInitialize]
        public void Setup()
        {
            mockMobileProvider = new Mock<IMobileProvider>();
            mockMobileProvider.Setup(x => x.GetAvailableMsisdnsAsync(It.IsAny<AvailableMsisdnProviderRequest>()))
                .ReturnsAsync(ProviderOperationResult<AvailableMSISDNResponseModel>.OkResult(new AvailableMSISDNResponseModel()));
        }

        [TestMethod]
        public void Get_ShouldReturnResult()
        {
            var controller = new AvailableMSISDNController(mockMobileProvider.Object);

            var response = controller.Get(Guid.NewGuid(), null, "twtwe").ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableMSISDNResponseModel>));
        }

        [TestMethod]
        public void Get_ShouldReturnResultWithQueryParamAndIncludeChildren()
        {
            var controller = new AvailableMSISDNController(mockMobileProvider.Object);
            var queryModel = new AvailableMSISDNSearchModel() { Status = "available" };
            var response = controller.Get(Guid.NewGuid(), queryModel, null, true).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableMSISDNResponseModel>));
        }


        [TestMethod]
        public void Get_ShouldReturnResultWithAllParams()
        {
            var controller = new AvailableMSISDNController(mockMobileProvider.Object);

            var response = controller.Get(Guid.NewGuid(), null, "available", true).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableMSISDNResponseModel>));
        }


        [TestMethod]
        public void Get_ShouldReturnModelStateErrorIfProviderReturnsInvalidInput()
        {
            var controller = new AvailableMSISDNController(mockMobileProvider.Object);
            mockMobileProvider.Setup(x => x.GetAvailableMsisdnsAsync(It.IsAny<AvailableMsisdnProviderRequest>()))
                .ReturnsAsync(ProviderOperationResult<AvailableMSISDNResponseModel>.InvalidInput("Target", "Msisdns empty string"));

            var response = controller.Get(Guid.NewGuid(), null, "").ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_Should_Return_Status_Not_Found()
        {
            mockMobileProvider.Setup(x => x.GetAvailableMsisdnsAsync(It.IsAny<AvailableMsisdnProviderRequest>()))
                .ReturnsAsync(ProviderOperationResult<AvailableMSISDNResponseModel>.NotFoundResult(""));
            var controller = new AvailableMSISDNController(mockMobileProvider.Object);

            var response = controller.Get(Guid.NewGuid(), null, "sdfsd").ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<StatusMessageModel>));
            Assert.AreEqual(HttpStatusCode.NotFound, ((NegotiatedContentResult<StatusMessageModel>)response).StatusCode);
        }
    }
}
