using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class ProductTypeControllerUT
    {
        private ProductTypeController controllerUnderTest;
        private Mock<IProductTypeProvider> productTypeProviderMock;
        private ProviderOperationResult<ProductTypeListResponseModel> productTypesResponse;

        [TestInitialize]
        public void SetUp()
        {
            productTypesResponse = ProviderOperationResult<ProductTypeListResponseModel>
                .OkResult(new ProductTypeListResponseModel
                    {
                        ProductTypes = new List<ProductTypeModel>
                            {
                                new ProductTypeModel { Id = Guid.NewGuid(), Name = "name1" }
                            }
                    });

            productTypeProviderMock = new Mock<IProductTypeProvider>();
            productTypeProviderMock.Setup(x => x.GetProductTypesAsync(It.IsAny<Guid>())).ReturnsAsync(productTypesResponse);

            controllerUnderTest = new ProductTypeController(productTypeProviderMock.Object);
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequest_IfModelStateIsInvalid()
        {
            controllerUnderTest.ModelState.AddModelError("some key", "error");

            var respone = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(respone, typeof(InvalidModelStateResult));
            productTypeProviderMock.Verify(x => x.GetProductTypesAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Get_ShoudCall_ProductTypeProvider_GetProductTypesAsync()
        {
            var respone = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            productTypeProviderMock.Verify(x => x.GetProductTypesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Get_ShouldReturnOkResult()
        {
            var respone = controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(respone, typeof(NegotiatedContentResult<ProductTypeListResponseModel>));

            Assert.AreEqual(((NegotiatedContentResult<ProductTypeListResponseModel>)respone).StatusCode, HttpStatusCode.OK);
        }
    }
}
