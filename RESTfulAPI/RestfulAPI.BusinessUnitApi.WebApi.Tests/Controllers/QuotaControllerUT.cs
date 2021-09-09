using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.Common;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class QuotaControllerUT
    {
        private Mock<IQuotaDistributionProvider> _mockProvider;
        private Guid _businessUnitClaim;
        private Guid _companyClaim;

        private QuotaController _controllerUnderTest;

        [TestInitialize]
        public void SetupEachTest()
        {
            _businessUnitClaim = Guid.NewGuid();
            _companyClaim = Guid.NewGuid();

            _mockProvider = new Mock<IQuotaDistributionProvider>(MockBehavior.Strict);
            _mockProvider.Setup(x => x.SetBusinessUnitQuota(It.IsAny<Guid>(), It.IsAny<SetBusinessUnitQuotaModel>(), It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(ProviderOperationResult<object>.AcceptedResult());

            _controllerUnderTest = new QuotaController(_mockProvider.Object);
            _controllerUnderTest.Configuration = new System.Web.Http.HttpConfiguration();
            var claims = new Claim[]
            {
                new Claim("CrmCompanyId", _companyClaim.ToString()),
                new Claim("CrmAccountId", _businessUnitClaim.ToString())
            };
            _controllerUnderTest.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        [TestMethod]
        public async Task Patch_ShouldReturnBadRequestIfModelIsNotValid()
        {
            _controllerUnderTest.ModelState.AddModelError("bla", "some error message");

            var result = await _controllerUnderTest.Patch(Guid.NewGuid(), new SetBusinessUnitQuotaModel());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task Patch_ShouldCallProviderAndReturnResult()
        {
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123
            };
            var businessUnitId = Guid.NewGuid();

            var result = await _controllerUnderTest.Patch(businessUnitId, input);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NegotiatedContentResult<object>));
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, (result as NegotiatedContentResult<object>).StatusCode);

            _mockProvider.Verify(x =>
                x.SetBusinessUnitQuota(
                    It.Is<Guid>(id => id == businessUnitId),
                    It.Is<SetBusinessUnitQuotaModel>(model => model == input),
                    It.IsAny<ClaimsPrincipal>()),
                Times.Once);
        }
    }
}
