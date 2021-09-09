using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.Common;
using System;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class BalanceProfileControllerUT
    {
        private Mock<IBalanceProfileProvider> mockProvider;

        [TestInitialize]
        public void SetupEachTest()
        {
            mockProvider = new Mock<IBalanceProfileProvider>(MockBehavior.Strict);
            mockProvider.Setup(x => x.GetBalanceProfilesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<BalanceProfileListModel>.OkResult(new BalanceProfileListModel()));
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequestIfModelStatisNotValid()
        {
            var controllerUnderTest = new BalanceProfileController(mockProvider.Object);

            controllerUnderTest.ModelState.AddModelError("bla", "some error");

            var result = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_ShouldCallProviderAndReturnResult()
        {
            var controllerUnderTest = new BalanceProfileController(mockProvider.Object);

            var result = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NegotiatedContentResult<BalanceProfileListModel>));
            Assert.IsNotNull((result as NegotiatedContentResult<BalanceProfileListModel>).Content);
            Assert.AreEqual(System.Net.HttpStatusCode.OK, (result as NegotiatedContentResult<BalanceProfileListModel>).StatusCode);

            mockProvider.Verify(x => x.GetBalanceProfilesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Get_ShouldThrowWhenProviderThrows()
        {
            mockProvider.Setup(x => x.GetBalanceProfilesAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("bla"));

            var controllerUnderTest = new BalanceProfileController(mockProvider.Object);

            var result = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
