using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.AccessProvider.Configuration;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.Common;
using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class PropositionsControllerUT
    {
        private Mock<IPropositionsProvider> propositionsProviderMocked;
        private Mock<IAccessProviderConfiguration> mockAccessProviderConfiguration;

        [TestInitialize]
        public void Setup()
        {
            propositionsProviderMocked = new Mock<IPropositionsProvider>();
            propositionsProviderMocked.Setup(x => x.GetPropositionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new PropositionsResponseModel());

            mockAccessProviderConfiguration = new Mock<IAccessProviderConfiguration>();
        }

        private static void SetupHiddenControllerDependencies(ApiController controller)
        {
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Route(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("some/route");
            mockUrlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns("http://domain.com:3265/some/route");
            controller.Url = mockUrlHelper.Object;
        }

        [TestMethod]
        public void Get_ShouldReturnPropositionsResponseModel()
        {
            var propositionsController = new PropositionsController(propositionsProviderMocked.Object);
            propositionsController.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;

            var response = propositionsController.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<PropositionsResponseModel>));
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequest()
        {
            var propositionsController = new PropositionsController(propositionsProviderMocked.Object);
            propositionsController.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;
            propositionsController.ModelState.Clear();
            propositionsController.ModelState.AddModelError("some_key", "some error message");

            var response = propositionsController.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_ShouldReturnNotFound()
        {
            var propositionsController = new PropositionsController(propositionsProviderMocked.Object);
            propositionsController.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;
            PropositionsResponseModel providerResponse = null;
            propositionsProviderMocked.Setup(x => x.GetPropositionsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(providerResponse);

            var response = propositionsController.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<StatusMessageModel>));
            Assert.AreEqual(HttpStatusCode.NotFound, ((NegotiatedContentResult<StatusMessageModel>)response).StatusCode);
        }
    }
}
