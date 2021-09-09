using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using RestfulAPI.TeleenaServiceReferences.ProductService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class ProductProviderUT
    {
        private PagedListInputForSearchProductsByAccountIdWithPaginationContract serviceRequest;
        private GetProductResponse[] serviceResponse;
        private GetProductResponse[] serviceResponseEmpty;
        private GetProductWithPagingResponse contractResponse;
        private GetProductWithPagingResponse contractResponseEmpty;
        private List<ProductModel> response;
        private AccountContract accountResponse;

        private ProviderOperationResult<ProductImeiListModel> productImeiResponse;
        private ProductImeiByBusinessUnitDataResultContract productImeiServiceResponse;

        private Mock<IBusinessUnitApiTranslators> mockTranslators;
        private Mock<ITeleenaServiceUnitOfWork> mockService;
        private Mock<ICustomAppConfiguration> mockAppConfiguration;
        private string appCofigurationRetVal;
        private int pageSize = 250;
        private Guid accoutIdEmptyResult;

        [TestInitialize]
        public void SetupEachTest()
        {
            accoutIdEmptyResult = Guid.NewGuid();
            accountResponse = new AccountContract();
            serviceRequest = new PagedListInputForSearchProductsByAccountIdWithPaginationContract();
            serviceResponse = new List<GetProductResponse> { new GetProductResponse { IccId = "11"}, new GetProductResponse { IccId = "555"}}.ToArray();
            serviceResponseEmpty = new List<GetProductResponse>().ToArray();
            contractResponse = new GetProductWithPagingResponse() { Products = serviceResponse, TotalResults = 1555 };
            contractResponseEmpty = new GetProductWithPagingResponse() { Products = serviceResponseEmpty, TotalResults = 0 };
            response = new List<ProductModel>
            {
                new ProductModel { Id = Guid.NewGuid(), Iccid = "123123"},
                new ProductModel { Id = Guid.NewGuid(), Iccid = "1266663"},
                new ProductModel { Id = Guid.NewGuid(), Iccid = "12214"},
            };
            appCofigurationRetVal = string.Empty;
            productImeiServiceResponse = new ProductImeiByBusinessUnitDataResultContract
            {
                Model = new[]
                {
                    new ProductImeiByBusinessUnitDataContract
                    {
                        CreationDate = DateTimeOffset.MinValue,
                        Imei = "1",
                        Iccid = "2",
                        ProductId = Guid.Empty
                    }
                }
            };

            mockTranslators = new Mock<IBusinessUnitApiTranslators>(MockBehavior.Strict);
            mockTranslators.Setup(x => x.ProductListTranslator.Translate(It.IsAny<GetProductResponse[]>())).Returns(response);
            mockTranslators.Setup(x => x.ProductImeiListTranslator.Translate(It.IsAny<ProductImeiByBusinessUnitDataContract[]>())).Returns(new ProductImeiListModel());
            mockTranslators.Setup(x => x.PageSizeInfoTranslator.Translate(It.Is<int?>(p => p == null))).Returns(new PageSizeInfo { IsPaged = false, PageSize = int.MaxValue });
            mockTranslators.Setup(x => x.PageSizeInfoTranslator.Translate(It.IsInRange(1000, int.MaxValue, Range.Inclusive))).Returns(new PageSizeInfo { IsPaged = true, PageSize = 1000 });
            mockTranslators.Setup(x => x.PageSizeInfoTranslator.Translate(It.IsInRange(1, 999, Range.Inclusive))).Returns(new PageSizeInfo { IsPaged = true, PageSize = pageSize });

            mockService = new Mock<ITeleenaServiceUnitOfWork>(MockBehavior.Strict);
            mockService.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>())).ReturnsAsync(accountResponse);
            //mockService.Setup(x => x.ProductService.SearchProductsByAccountIdWithPaginationAsync(It.IsAny<PagedListInputForSearchProductsByAccountIdWithPaginationContract>()))
            //    .ReturnsAsync(serviceResponse);
            mockService.Setup(x => x.ProductService.GetProductsByAccountIdAsync(It.IsAny<GetProductsByAccountRequest>()))
                .ReturnsAsync(serviceResponse);
            mockService.Setup(x => x.ProductService.GetProductsSearchByAccountIdAsync(It.IsAny<GetProductsByAccountRequest>()))
                .ReturnsAsync(contractResponse);
            mockService.Setup(x => x.ProductService.GetProductsSearchByAccountIdAsync(It.Is<GetProductsByAccountRequest>(p => p.AccountId == accoutIdEmptyResult)))
                .ReturnsAsync(contractResponseEmpty);
            mockService.Setup(x => x.ProductImeiService.GetProductImeiByBusinessUnitAsync(It.IsAny<GetProductImeiByBusinessUnitDataContract>()))
                .ReturnsAsync(productImeiServiceResponse);

            mockAppConfiguration = new Mock<ICustomAppConfiguration>(MockBehavior.Strict);
            mockAppConfiguration.Setup(x => x.GetConfigurationValue(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldCallServiceMethodToFetchListOfProducts()
        {
            var businessUnitId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnitId, companyId, null, null, 1, 1000)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            Assert.IsNotNull(response);
            mockService.Verify(x => x.ProductService.GetProductsSearchByAccountIdAsync(It.IsAny<GetProductsByAccountRequest>()), Times.Once);
        }


        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldSupplyIncludeChildrenIfSet()
        {
            var businessUnitId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            mockService.Setup(x => x.ProductService.GetProductsSearchByAccountIdAsync(
                    It.Is<GetProductsByAccountRequest>(request => request.IncludeChildAccounts == true)))
                .ReturnsAsync(contractResponse)
                .Verifiable();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnitId, companyId, null, true, 1, 1000)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            Assert.IsNotNull(response);
            mockService.Verify();
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldUseTranslator()
        {
            var businessUnit = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            contractResponse.TotalResults = 1;
            contractResponse.Products = new GetProductResponse[] { new GetProductResponse() };
            serviceResponse = new GetProductResponse[] { new GetProductResponse() }; // needs at least one to trigger translator
            mockService.Setup(x => x.ProductService.GetProductsSearchByAccountIdAsync(It.IsAny<GetProductsByAccountRequest>())) 
                .ReturnsAsync(contractResponse);
            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnit, companyId, null, null, 1, 1000)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            mockTranslators.Verify(x => x.ProductListTranslator.Translate(It.IsAny<GetProductResponse[]>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldReturnEmptyListIfNoMatchesFoundAndNotCallTranslator()
        {
            var companyId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(accoutIdEmptyResult, companyId, null, null, 1, 1000)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            Assert.AreEqual(0, response.Result.Products.Count);
            mockTranslators.Verify(x => x.ProductListTranslator.Translate(It.IsAny<GetProductResponse[]>()), Times.Never);
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldSendPagingInformationAndReturnTotalCount()
        {
            var businessUnitId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnitId, companyId, null, null, 33, pageSize)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            mockService.Verify(x => x.ProductService.GetProductsSearchByAccountIdAsync(It.Is<GetProductsByAccountRequest>(input => input.PageSize == pageSize && input.PageNumber == 33)), Times.Once);
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldCall_PageSizeInfoTranslator()
        {
            var businessUnitId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            int? pageSize = 35;

            var providerunderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerunderTest.GetProductsForBusinessUnitAsync(businessUnitId, companyId, null, null, 1, pageSize).ConfigureAwait(false).GetAwaiter().GetResult();

            mockTranslators.Verify(t => t.PageSizeInfoTranslator.Translate(It.Is<int?>(p => p == pageSize)), Times.Once);
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_ShouldReturnPageInfoSize_IfRequestIsPaged()
        {
            var businessUnitID = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnitID, companyId, null, null, 2, 100).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response.Result.PagingInfo);
        }

        [TestMethod]
        public void GetProductsForBusinessUnitAsync_pageInfoSizeShouldBeNull_IfRequestIsNotPaged()
        {
            var businessUnitId = Guid.NewGuid();
            var companyId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            var response = providerUnderTest.GetProductsForBusinessUnitAsync(businessUnitId, companyId, null, null, 1, null).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(response.Result.PagingInfo);
        }

        [TestMethod]
        public void GetProductImeisForBusinessUnitAsync_ShouldCallServiceMethodToFetchListOfProductImeiOnce()
        {
            var businessUnitId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            productImeiResponse = providerUnderTest.GetProductImeisForBusinessUnitAsync(new GetProductImeiByBusinessUnitDataContract { BusinessUnitId = businessUnitId} )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            
            mockService.Verify(x => x.ProductImeiService.GetProductImeiByBusinessUnitAsync(It.IsAny<GetProductImeiByBusinessUnitDataContract>()), Times.Once);
        }

        [TestMethod]
        public void GetProductImeisForBusinessUnitAsync_ShouldCallTranslatorOnce()
        {
            var businessUnitId = Guid.NewGuid();

            var providerUnderTest = new ProductProvider(mockService.Object, mockTranslators.Object, mockAppConfiguration.Object);

            productImeiResponse = providerUnderTest.GetProductImeisForBusinessUnitAsync(new GetProductImeiByBusinessUnitDataContract { BusinessUnitId = businessUnitId} )
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            
            mockTranslators.Verify(x => x.ProductImeiListTranslator.Translate(It.IsAny<ProductImeiByBusinessUnitDataContract[]>()), Times.Once);
        }
    }
}
