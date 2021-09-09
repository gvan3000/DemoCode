using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.Common;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Results;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class ProductControllerUT
    {
        private ProductsListModel _returnedProducts;
        private ProductImeiListModel _returnedProductImei;
        private ProviderOperationResult<ProductsListModel> _providerOperationResult;
        private ProviderOperationResult<ProductImeiListModel> _providerProductImeiOperationResult;
        private Mock<IProductProvider> _productProvider;

        private const string AllowedImei = "12345678901234";

        private ProductController _controllerUnderTest;

        [TestInitialize]
        public void SetupEachTest()
        {
            _returnedProducts = new ProductsListModel();
            _returnedProductImei = new ProductImeiListModel();
            _providerOperationResult = ProviderOperationResult<ProductsListModel>.OkResult(_returnedProducts);
            _providerProductImeiOperationResult = ProviderOperationResult<ProductImeiListModel>.OkResult(_returnedProductImei);
            _productProvider = new Mock<IProductProvider>();
            _productProvider.Setup(x => x.GetProductsForBusinessUnitAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime?>(), It.IsAny<bool?>(), It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(_providerOperationResult);
            _productProvider.Setup(x => x.GetProductImeisForBusinessUnitAsync(It.IsAny<GetProductImeiByBusinessUnitDataContract>()))
                .ReturnsAsync(_providerProductImeiOperationResult);

            _controllerUnderTest = new ProductController(_productProvider.Object);
        }

        private void SetProviderDependencies(ApiController controller)
        {
            var request = new Mock<HttpRequestMessage>();

            Uri locationUrl;
            Uri.TryCreate("http://localhost/test/products/1", UriKind.RelativeOrAbsolute, out locationUrl);

            request.Object.RequestUri = locationUrl;

            controller.Request = request.Object;
            var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim("CrmCompanyId", Guid.NewGuid().ToString()),
                    new Claim("CrmAccountId", Guid.NewGuid().ToString())
                });
            controller.User = new ClaimsPrincipal(claimsIdentity);
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequestIfInputDataIsNotValid()
        {
            _controllerUnderTest.ModelState.Clear();
            _controllerUnderTest.ModelState.AddModelError("some_key", "some error");

            var response = _controllerUnderTest.Get(Guid.NewGuid(), new DateTime(2010, 10, 10), true) // invalid data combination should be ignored here
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_ShouldReturnBadRequestForDateTimeBelowMinRange()
        {
            var response = _controllerUnderTest.Get(Guid.NewGuid(), new DateTime(1500, 10, 10), true);

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response.Result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Get_ShouldReturn_ProductListModelResponseType()
        {
            string hostName = "http://localhost";

            _returnedProducts.Products.Add(new ProductModel() { Location = $"{hostName}" });

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.Get(Guid.NewGuid(), new DateTime(2012, 12, 12), true)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<ProductsListModel>));
        }

        [TestMethod]
        public void Get_LocationPropertyShouldBeFilled()
        {
            Guid id = Guid.Parse("10000000-0000-0000-0000-000000000000");
            string host = "http://localhost";

            _returnedProducts.Products.Add(new ProductModel { Id = id, Location = $"{host}/products/{id}" });

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.Get(Guid.NewGuid(), new DateTime(2015, 11, 11), true)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var convertedResult = response as NegotiatedContentResult<ProductsListModel>;

            Assert.IsTrue(convertedResult.Content.Products.Exists(x => x.Location == $"{host}/products/{id}"));
        }

        [TestMethod]
        public void GetProductImei_IfInputDataIsNotValid_ShouldReturnBadRequest()
        {
            _controllerUnderTest.ModelState.Clear();
            _controllerUnderTest.ModelState.AddModelError("some_key", "some error");

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetProductImei_ShouldReturn_ProductImeiListModelResponseType()
        {
            _returnedProductImei.Products.Add(new ProductImeiModel());

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<ProductImeiListModel>));
        }

        [TestMethod]
        public void GetProductImei_ImeiLengthIsLessThan14Digits_ReturnsBadRequest()
        {
            _returnedProductImei.Products.Add(new ProductImeiModel());

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid(), "123456")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var invalidResponse = response as InvalidModelStateResult;
            Assert.IsNotNull(invalidResponse);
            Assert.AreEqual(1, invalidResponse.ModelState.Values.First().Errors.Count);
            Assert.IsTrue(invalidResponse.ModelState.Values.First().Errors.First().ErrorMessage.Contains(
                $"imei must have at least {ImeiConstants.MinLength} digits"));
        }

        [TestMethod]
        public void GetProductImei_ImeiLengthIsMoreThan14Digits_CallsProductProviderWithTruncatedImei()
        {
            _returnedProductImei.Products.Add(new ProductImeiModel());

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid(), $"{AllowedImei}56")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            _productProvider.Verify(p => p.GetProductImeisForBusinessUnitAsync(
                It.Is<GetProductImeiByBusinessUnitDataContract>(x => x.Imei == AllowedImei)), Times.Once());
        }

        [TestMethod]
        public void GetProductImei_ImeiLengthEquals14Digits_CallsProductProviderWithOriginalImei()
        {
            _returnedProductImei.Products.Add(new ProductImeiModel());

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid(), AllowedImei)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            _productProvider.Verify(p => p.GetProductImeisForBusinessUnitAsync(
                It.Is<GetProductImeiByBusinessUnitDataContract>(x => x.Imei == AllowedImei)), Times.Once());
        }

        [TestMethod]
        public void GetProductImei_ImeiIsNotSpecified_CallsProductProviderWithImeiEqualsNull()
        {
            _returnedProductImei.Products.Add(new ProductImeiModel());

            SetProviderDependencies(_controllerUnderTest);

            var response = _controllerUnderTest.GetProductImei(Guid.NewGuid())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            _productProvider.Verify(p => p.GetProductImeisForBusinessUnitAsync(
                It.Is<GetProductImeiByBusinessUnitDataContract>(x => string.IsNullOrEmpty(x.Imei))), Times.Once());
        }
    }
}
