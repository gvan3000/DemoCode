using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.ProductTypeService;
using System;
using System.Collections.Generic;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class ProductTypesProviderUT
    {
        #region Private Fields

        private ProductTypeProvider providerUnderTest;

        private Mock<ITeleenaServiceUnitOfWork> teleenaServiceMock;
        private Mock<IBusinessUnitApiTranslators> translatorMock;
        private Mock<IJsonRestApiLogger> loggerMock;

        private Guid companyId;
        private AccountContract accountContract;
        private AccountContract nullAccountContract;
        private GetProductTypesByCompanyContract getProdTypesRequest;
        private ProductTypeContract[] productTypes;
        private ProductTypeListResponseModel productTypesTranslated;
        private string prodTypeName1 = "name1";
        private string prodTypeName2 = "name2";
        private Guid prodTypeId1 = Guid.NewGuid();
        private Guid prodTypeId2 = Guid.NewGuid();
        private Guid nullBuId = Guid.NewGuid();

        #endregion

        #region SetUp Mock
       
        [TestInitialize]
        public void SetUp()
        {
            companyId = Guid.NewGuid();
            accountContract = new AccountContract { Id = Guid.NewGuid(), CompanyId = companyId };
            nullAccountContract = null;
            getProdTypesRequest = new GetProductTypesByCompanyContract { CompanyId = companyId };
            productTypes = new ProductTypeContract[] { new ProductTypeContract { Name = prodTypeName1, Id = Guid.NewGuid() }, new ProductTypeContract { Name = prodTypeName2, Id = Guid.NewGuid() } };
            productTypesTranslated = new ProductTypeListResponseModel
            {
                ProductTypes = new List<ProductTypeModel>
                {
                    new ProductTypeModel { Id = prodTypeId1, Name = prodTypeName1 },
                    new ProductTypeModel { Id = prodTypeId2, Name = prodTypeName2 },
                }
            };

            teleenaServiceMock = new Mock<ITeleenaServiceUnitOfWork>();
            teleenaServiceMock.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>())).ReturnsAsync(accountContract);
            teleenaServiceMock.Setup(x => x.AccountService.GetAccountByIdAsync(It.Is<Guid>(b => b == nullBuId))).ReturnsAsync(nullAccountContract);
            teleenaServiceMock.Setup(x => x.ProductTypeService.GetProductTypesByCompanyAsync(It.IsAny<GetProductTypesByCompanyContract>())).ReturnsAsync(productTypes);

            translatorMock = new Mock<IBusinessUnitApiTranslators>();
            translatorMock.Setup(x => x.ProductTypeContractTranslator.Translate(It.IsAny<ProductTypeContract[]>())).Returns(productTypesTranslated);

            loggerMock = new Mock<IJsonRestApiLogger>();

            providerUnderTest = new ProductTypeProvider(teleenaServiceMock.Object, translatorMock.Object, loggerMock.Object);
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProductTypesAsync_ShouldThrow_IfBusinessUnitIsGuidEmpty()
        {
            var response = providerUnderTest.GetProductTypesAsync(Guid.Empty).ConfigureAwait(false).GetAwaiter().GetResult();

            // not returning bad request, throwing now
            //Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
            //Assert.IsTrue(!string.IsNullOrEmpty(response.ErrorMessage));
            //Assert.IsTrue(!response.IsSuccess);
        }

        [TestMethod]
        public void GetProductTypesAsync_ShouldCall_AccountSevice_GetAccountByIdAsync()
        {
            var response = providerUnderTest.GetProductTypesAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            teleenaServiceMock.Verify(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetProductTypesAsync_ShouldReturnNotFound_IfBusinessUnitNotFoundById()
        {
            var response = providerUnderTest.GetProductTypesAsync(nullBuId).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpResponseCode);
            Assert.IsTrue(!response.IsSuccess);
        }

        [TestMethod]
        public void GetProductTypesAsync_ShouldCall_ProductTypeService_GetProductTypesByCompanyAsync()
        {
            var response = providerUnderTest.GetProductTypesAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            teleenaServiceMock.Verify(x => x.ProductTypeService.GetProductTypesByCompanyAsync(It.IsAny<GetProductTypesByCompanyContract>()), Times.Once);
        }

        [TestMethod]
        public void GetProductTypesAsync_ShouldCall_ProductContractTranslator()
        {
            var response = providerUnderTest.GetProductTypesAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            translatorMock.Verify(x => x.ProductTypeContractTranslator.Translate(It.IsAny<ProductTypeContract[]>()), Times.Once);
        }

        [TestMethod]
        public void GetProductTypeAsync_ShouldReturnOkresult()
        {
            var response = providerUnderTest.GetProductTypesAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
            Assert.IsTrue(response.IsSuccess);
        }       
    }
}


