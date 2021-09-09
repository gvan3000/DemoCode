using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.AccessProvider.Cache;
using RestfulAPI.AccessProvider.Configuration;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class BusinessUnitControllerUT
    {
        private Guid exceptionInputID = Guid.Parse("2329183D-BB9E-4D8C-BB2D-95244C69833F");
        private BusinessUnitModel businessUnitModelResponse;
        private BusinessUnitCreateModel createInputModel;
        private CreateBusinessUnitResponseModel createResponseModel;
        private BusinessUnitPatchModel businessUnitPatchRequest;
        private ProviderOperationResult<object> addAllowedAddOnsResponse;
        private bool validateAddOnsResponse;
        private bool validateAddOnsResponseFalse;
        private Guid addOnFirst;
        private Guid addOnSecond;
        private List<Guid> addOnList;
        private APNsResponseModel apnsResponseModel;
        private BusinessUnitCreateModel defaultCreateBusinessUnitModel;
        private Mock<IAddOnProvider> mockAddAddOnProvider;
        private Mock<IBusinessUnitProvider> mockBusinessUnitProvider;
        private Mock<IAccessProviderConfiguration> mockAccessProviderConfiguration;
        private Mock<IJsonRestApiLogger> mockLogger;
        private Mock<IBusinessUnitCacheProvider> mockBusinessUnitCacheProvider;
        private Mock<IBusinessUnitForCompanyCacheProvider> mockBusinessUnitForCompanyCacheProvider;
        private Mock<IAPNProvider> mockApnProvider;
        private Mock<ISmscSettingProvider> mockSmscSettingsProvider;
        private Mock<IFeatureProvider> mockFeatureProvider;
        private Mock<IPreferredLanguageProvider> mockPreferredLanguageProvider;

        [TestInitialize]
        public void SetupEachTest()
        {
            businessUnitModelResponse = new BusinessUnitModel();
            createInputModel = new BusinessUnitCreateModel();
            createResponseModel = new CreateBusinessUnitResponseModel { Id = Guid.NewGuid(), Location = "spec/location/path" };
            businessUnitPatchRequest = new BusinessUnitPatchModel();
            addAllowedAddOnsResponse = ProviderOperationResult<object>.OkResult();
            validateAddOnsResponse = true;
            addOnFirst = Guid.NewGuid();
            addOnSecond = Guid.NewGuid();
            addOnList = new List<Guid> { addOnFirst, addOnSecond };
            validateAddOnsResponseFalse = false;
            apnsResponseModel = new APNsResponseModel()
            {
                Apns = new List<APNResponseDetail>()
                {
                  new APNResponseDetail() { Name = "apn1", Id = Guid.NewGuid() },
                  new APNResponseDetail() { Name = "apn2" , Id = Guid.NewGuid() }
                },
                DefaultApn = Guid.NewGuid()
            };

            defaultCreateBusinessUnitModel = new BusinessUnitCreateModel
            {
                UseOABasedSmsRouting = true,
                Name = "name",
                BillingCycleStartDay = 1,
                CustomerId = "1234",
                ParentId = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                AddOns = null,
                Propositions = new[]
                {
                    new Proposition
                    {
                        Id = Guid.NewGuid(),
                        BusinessUnitId = Guid.NewGuid(),
                        EndUserSubscription = true
                    },
                }

            };

            mockAccessProviderConfiguration = new Mock<IAccessProviderConfiguration>();

            mockAddAddOnProvider = new Mock<IAddOnProvider>();

            mockBusinessUnitProvider = new Mock<IBusinessUnitProvider>();

            mockBusinessUnitCacheProvider = new Mock<IBusinessUnitCacheProvider>();
            mockBusinessUnitForCompanyCacheProvider = new Mock<IBusinessUnitForCompanyCacheProvider>();

            mockApnProvider = new Mock<IAPNProvider>(MockBehavior.Strict);
            mockSmscSettingsProvider = new Mock<ISmscSettingProvider>();
            mockFeatureProvider = new Mock<IFeatureProvider>();

            mockPreferredLanguageProvider = new Mock<IPreferredLanguageProvider>();

            mockBusinessUnitProvider.Setup(x => x.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<BusinessUnitCreateModel>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<CreateBusinessUnitResponseModel>.AcceptedResult(createResponseModel));

            mockBusinessUnitProvider.Setup(x => x.UpdateBusinessUnitAsync(It.IsAny<Guid>(), It.IsAny<BusinessUnitPatchModel>()))
                .ReturnsAsync(ProviderOperationResult<object>.OkResult());

            mockBusinessUnitProvider.Setup(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()))
                .ReturnsAsync(new BusinessUnitListModel() { BusinessUnits = new List<BusinessUnitModel>() { new BusinessUnitModel() } });

            mockBusinessUnitProvider.Setup(x => x.UpdateBusinessUnitAsync(It.Is<Guid>(input => input == exceptionInputID), It.IsAny<BusinessUnitPatchModel>()))
                .Throws(new Exception());
            mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);

            mockApnProvider.Setup(x => x.GetCompanyAPNsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Domain.Models.APNs.APNSetList());
            mockApnProvider.Setup(x => x.UpdateBusinessUnitApnsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateBusinessUnitApnsModel>()))
                .ReturnsAsync(ProviderOperationResult<object>.OkResult());
            mockApnProvider.Setup(x => x.GetAPNsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<APNsResponseModel>.OkResult(apnsResponseModel));
            mockApnProvider.Setup(x => x.RemoveApnAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(ProviderOperationResult<object>.OkResult());
            mockApnProvider.Setup(x => x.UpdateBusinessUnitDefaultApnAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateBusinessUnitDefaultApnModel>()))
                .ReturnsAsync(ProviderOperationResult<object>.OkResult());

            mockAddAddOnProvider.Setup(x => x.AddAddOnAsync(It.IsAny<PurchaseAddOnModel>(), It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(ProviderOperationResult<object>.OkResult());
            mockAddAddOnProvider.Setup(x => x.AddAllowedAddOnsToBusinessUnit(It.IsAny<List<Guid>>(), It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(addAllowedAddOnsResponse);

            mockAddAddOnProvider.Setup(x => x.ValidateAddOns(It.IsAny<List<Guid>>())).ReturnsAsync(validateAddOnsResponse);
            mockAddAddOnProvider.Setup(x => x.ValidateAddOns(It.Is<List<Guid>>(a => a == addOnList))).ReturnsAsync(validateAddOnsResponseFalse);

            mockFeatureProvider.Setup(x => x.IsCompanyFeatureEnabled(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            mockSmscSettingsProvider.Setup(x => x.CreateOrUpdate(true, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(ProviderOperationResult<object>.AcceptedResult());

        }

        private static void SetupHiddenControllerDependencies(BaseApiController controller)
        {
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Route(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("some/route");
            mockUrlHelper.Setup(x => x.Content(It.IsAny<string>())).Returns("http://domain.com:3265/some/route");
            controller.Url = mockUrlHelper.Object;
            var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim("CrmCompanyId", Guid.NewGuid().ToString()),
                    new Claim("CrmAccountId", Guid.NewGuid().ToString())
                });
            controller.User = new ClaimsPrincipal(claimsIdentity);

            var mockAccessProviderConfiguration = new Mock<IAccessProviderConfiguration>();
            mockAccessProviderConfiguration.SetupGet(m => m.UseAccessProvider).Returns(false);
            controller.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            controller.Request.Properties[RequestAndResponseIdConstants.RequestidHeaderName] = Guid.NewGuid();
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestForInvalidInputData()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object);
            controllerUnderTest.ModelState.Clear();
            controllerUnderTest.ModelState.AddModelError("some_key", "some error message");
            var result = controllerUnderTest.Post(createInputModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Post_ShouldCallBusinessUnitProvider()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockBusinessUnitProvider.Verify(x => x.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.Is<BusinessUnitCreateModel>(model => model == createModel), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Post_WhenAddOnsAreProvided_ShouldCallAddOnProvider()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };
            createModel.AddOns = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockAddAddOnProvider.Verify(x => x.AddAllowedAddOnsToBusinessUnit(It.IsAny<List<Guid>>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Post_WhenAddOnsNotProvided_ShouldNotCallAddOnProvider_AddAllowedAddOnsToBusinessUnit_method()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockAddAddOnProvider.Verify(x => x.AddAllowedAddOnsToBusinessUnit(It.IsAny<List<Guid>>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Post_WhenAddOnsNotProvided_ShouldNotCallAddOnProvider_ValidateAddOns_method()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockAddAddOnProvider.Verify(x => x.ValidateAddOns(It.IsAny<List<Guid>>()), Times.Never);
        }

        [TestMethod]
        public void Post_WhenAddOnsProvidedAndValidateAddOnsReturnFalse_ShouldNotCallAddOnProvider_AddAllowedAddOnsToBusinessUnit_method()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };
            createModel.AddOns = addOnList;

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockAddAddOnProvider.Verify(x => x.AddAllowedAddOnsToBusinessUnit(It.IsAny<List<Guid>>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Post_WhenAddOnsProvidedAndValidateAddOnsReturnFalse_ShouldReturnBadRequest()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };
            createModel.AddOns = addOnList;

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void Post_WhenDuplicateAddOnsProvided_ShouldReturnBadRequest()
        {
            Guid addOn = Guid.NewGuid();

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };
            createModel.AddOns = new List<Guid> { addOn, addOn, Guid.NewGuid() };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsInstanceOfType(response, typeof(BadRequestErrorMessageResult));
        }

        [TestMethod]
        public void Post_ShouldReturnCreatedAndFillLocationInResponseModel()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            BusinessUnitCreateModel createModel = new BusinessUnitCreateModel();
            createModel.Propositions = new Proposition[] { new Proposition { Id = Guid.NewGuid() }, new Proposition { Id = Guid.NewGuid() } };

            var response = controllerUnderTest.Post(createModel).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<CreateBusinessUnitResponseModel>));
            Assert.AreEqual(System.Net.HttpStatusCode.Accepted, (response as NegotiatedContentResult<CreateBusinessUnitResponseModel>).StatusCode);
        }

        [TestMethod]
        public void GetById_ShouldCallBusinessUnitProvider()
        {
            Guid id = Guid.NewGuid();

            var businesUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businesUnitControllerMocked);

            var response = businesUnitControllerMocked.Get(id).ConfigureAwait(false).GetAwaiter().GetResult();

            mockBusinessUnitProvider.Verify(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetById_ShouldReturnResponseModel()
        {
            Guid id = Guid.NewGuid();
            var businesUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businesUnitControllerMocked);

            var response = businesUnitControllerMocked.Get(id).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<BusinessUnitModel>));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetById_ShouldReturnInternalServerError_IfProviderThrowsException()
        {
            Guid id = exceptionInputID;
            mockBusinessUnitProvider.Setup(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()))
                .ThrowsAsync(new Exception());
            var businesUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businesUnitControllerMocked);

            var response = businesUnitControllerMocked.Get(id).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetByName_ShouldCallBusinessUnitProvider_IfInputDataIsValid()
        {
            BusinessUnitSearchModel queryName = new BusinessUnitSearchModel() { Name = "bjdhs" };

            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            businessUnitControllerMocked.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;
            SetupHiddenControllerDependencies(businessUnitControllerMocked);

            var response = businessUnitControllerMocked.Get(queryName).ConfigureAwait(false).GetAwaiter().GetResult();

            mockBusinessUnitProvider.Verify(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<Domain.ApiModelProviders.Contracts.GetBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetByName_ShouldReturnBadRequestStatus_IfInputIsInvalid()
        {
            BusinessUnitSearchModel queryName = new BusinessUnitSearchModel();
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businessUnitControllerMocked);

            businessUnitControllerMocked.ModelState.AddModelError("bla", "bla");

            var response = businessUnitControllerMocked.Get(queryName).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetByName_ShouldReturnInternalServerError_IfProviderThrowsException()
        {
            BusinessUnitSearchModel queryName = new BusinessUnitSearchModel() { Name = "wwww" };
            mockBusinessUnitProvider.Setup(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()))
                .ThrowsAsync(new Exception());
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businessUnitControllerMocked);

            var response = businessUnitControllerMocked.Get(queryName).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetByName_ShouldReturnResponseModel_IfInputDataIsValid()
        {
            BusinessUnitSearchModel queryName = new BusinessUnitSearchModel() { Name = "wwww" };

            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businessUnitControllerMocked);
            businessUnitControllerMocked.AccessProviderConfiguration = mockAccessProviderConfiguration.Object;

            var response = businessUnitControllerMocked.Get(queryName).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<BusinessUnitListModel>));
        }

        [TestMethod]
        public void Patch_ShouldReturnHttpStatus200()
        {
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            var response = businessUnitControllerMocked.Patch(Guid.NewGuid(), businessUnitPatchRequest).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<object>));
            Assert.AreEqual(HttpStatusCode.OK, (response as NegotiatedContentResult<object>).StatusCode);
        }

        [TestMethod]
        public void Patch_ShouldRetunBadRequest()
        {
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            businessUnitControllerMocked.ModelState.Clear();
            businessUnitControllerMocked.ModelState.AddModelError("some_key", "some error message");

            var response = businessUnitControllerMocked.Patch(Guid.NewGuid(), businessUnitPatchRequest).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Put_ShouldReturnInternalServerError_IfProviderUpdateMethodThrowsException()
        {
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            var response = businessUnitControllerMocked.Patch(exceptionInputID, businessUnitPatchRequest).ConfigureAwait(false).GetAwaiter().GetResult();

            //Assert.IsInstanceOfType(response, typeof(ExceptionResult)); // this is handled by framework for us, no need to return 500 explicitly
        }

        [TestMethod]
        public void Get_ShouldReturnOkResult()
        {
            var buControllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(buControllerUnderTest);

            var response = buControllerUnderTest.Get(true).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<BusinessUnitListModel>));
        }

        [TestMethod]
        public void Get_ShouldCall_BusinessUnitProvider_GetBusinessUnitsWithFilteringAsync()
        {
            var buControllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(buControllerUnderTest);

            var response = buControllerUnderTest.Get(true).ConfigureAwait(false).GetAwaiter().GetResult();

            mockBusinessUnitProvider.Verify(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        public void Get_ShouldReturnEmptyArrayIfNoBusinessUnitFound()
        {
            mockBusinessUnitProvider.Setup(x => x.GetBusinessUnitsWithFilteringAsync(It.IsAny<GetBusinessUnitRequest>()))
                .ReturnsAsync(new BusinessUnitListModel() { BusinessUnits = new List<BusinessUnitModel>() });
            var businessUnitControllerMocked = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(businessUnitControllerMocked);

            var response = businessUnitControllerMocked.Get(false).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(OkNegotiatedContentResult<BusinessUnitListModel>));
            Assert.AreEqual(0, (response as OkNegotiatedContentResult<BusinessUnitListModel>).Content.BusinessUnits.Count);
        }

        [TestMethod]
        public void UpdateApns_ShouldReturnErrorWhenInputIsNotValid()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>(),
                DefaultApn = Guid.NewGuid()
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);
            controllerUnderTest.ModelState.AddModelError("bla", "error message");

            var result = controllerUnderTest.UpdateApns(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void UpdateApns_ShouldCallProviderToUpdateBusinessUnitApns()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>(),
                DefaultApn = Guid.NewGuid()
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.UpdateApns(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);

            mockApnProvider.Verify(x => x.UpdateBusinessUnitApnsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.Is<UpdateBusinessUnitApnsModel>(model => model == input)), Times.Once);
        }

        [TestMethod]
        public void UpdateApns_ShouldReturnOkWhenUpdateIsSuccessful()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>(),
                DefaultApn = Guid.NewGuid()
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.UpdateApns(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NegotiatedContentResult<object>));
            Assert.AreEqual(HttpStatusCode.OK, (result as NegotiatedContentResult<object>).StatusCode);
        }

        [TestMethod]
        public void UpdateApns_ShouldReturnErrorResponseWhenProviderIndicatesError()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>(),
                DefaultApn = Guid.NewGuid()
            };

            mockApnProvider.Setup(x => x.UpdateBusinessUnitApnsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateBusinessUnitApnsModel>()))
                .ReturnsAsync(Common.ProviderOperationResult<object>.InvalidInput("bla", "error"));

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.UpdateApns(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetAPNsByBusinessUnitId_ShouldCallProvider()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.GetAPNsByBusinessUnitId(Guid.NewGuid());

            Assert.IsNotNull(result);

            mockApnProvider.Verify(x => x.GetAPNsAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetAPNsByBusinessUnitId_ShouldReturnInternalServerError_IfProviderThrowsException()
        {
            Guid id = exceptionInputID;
            mockApnProvider.Setup(x => x.GetAPNsAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception());
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.GetAPNsByBusinessUnitId(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateDefaultApn_ShouldReturnErrorWhenInputIsNotValid()
        {
            var input = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);
            controllerUnderTest.ModelState.AddModelError("error", "error message");

            var result = controllerUnderTest.UpdateDefaultApn(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void UpdateDefaultApn_ShouldCallProviderToUpdateBusinessUnitApns()
        {
            Guid validAPNId = Guid.NewGuid();
            var input = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = validAPNId
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);
            var result = controllerUnderTest.UpdateDefaultApn(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);

            mockApnProvider.Verify(x => x.UpdateBusinessUnitDefaultApnAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.Is<UpdateBusinessUnitDefaultApnModel>(model => model == input)), Times.Once);
        }

        [TestMethod]
        public void UpdateDefaultApn_ShouldReturnErrorResponseWhenProviderReturnsError()
        {
            Guid invalidAPNName = Guid.NewGuid();
            var input = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = invalidAPNName
            };

            mockApnProvider.Setup(x => x.UpdateBusinessUnitDefaultApnAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), input))
                .ReturnsAsync(ProviderOperationResult<object>.InvalidInput("error target", "error message"));

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.UpdateDefaultApn(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void UpdateDefaultApn_ShouldReturnOkWhenUpdateIsSuccessful()
        {
            Guid validAPNName = Guid.NewGuid();
            var input = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = validAPNName
            };

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            SetupHiddenControllerDependencies(controllerUnderTest);

            var result = controllerUnderTest.UpdateDefaultApn(Guid.NewGuid(), input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NegotiatedContentResult<object>));
            Assert.AreEqual(HttpStatusCode.OK, (result as NegotiatedContentResult<object>).StatusCode);
        }

        [TestMethod]
        public void RemoveApn_Should_Call_ApnProvider_RemoveApnAsync()
        {
            var controllesUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            var response = controllesUnderTest.RemoveApn(Guid.NewGuid(), Guid.NewGuid());

            mockApnProvider.Verify(x => x.RemoveApnAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void RemoveApn_ShouldReturn_BadRequestForInvalidInputData()
        {
            var controllesUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            controllesUnderTest.ModelState.AddModelError("errorKey", "error 1 2 3 ");

            var response = controllesUnderTest.RemoveApn(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void RemoveApn_ShouldReturn_OkResult()
        {
            var controllesUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };

            var response = controllesUnderTest.RemoveApn(Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, (response as NegotiatedContentResult<object>).StatusCode);
        }

        [TestMethod]
        public void Post_ShouldReturn_NotCreateBUIfOABasedSmsRoutingFeatureDisabled()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            mockFeatureProvider.Setup(x => x.IsCompanyFeatureEnabled(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockSmscSettingsProvider.Setup(x => x.CreateOrUpdate(true, It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError, "updateBusinessUnit", "error"));

            var response = controllerUnderTest.Post(defaultCreateBusinessUnitModel).ConfigureAwait(false).GetAwaiter().GetResult();

            mockBusinessUnitProvider.Verify(x => x.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BusinessUnitCreateModel>(), It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void Post_ShouldReturn_CreateBUIfOABasedSmsRoutingFeatureEnabled()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.Post(defaultCreateBusinessUnitModel).ConfigureAwait(false).GetAwaiter().GetResult();

            mockBusinessUnitProvider.Verify(x => x.CreateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<BusinessUnitCreateModel>(), It.IsAny<Guid>()), Times.Once);
            mockSmscSettingsProvider.Verify(x => x.CreateOrUpdate(true, It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void GetBusinessUnitPreferredLanguages_ModelStateInvalid_ShouldReturnBadRequestForInvalidInputData()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            controllerUnderTest.ModelState.AddModelError("errorKey", "error 1 2 3 ");
            var accountId = Guid.NewGuid();


            var response = controllerUnderTest.GetBusinessUnitPreferredLanguages(accountId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetBusinessUnitPreferredLanguages_RequestedGuidEmpty_ShouldReturnBadRequestForInvalidInputData()
        {
            var businessUnitId = Guid.Empty;
            var responseResult = Task.Factory.StartNew(() => ProviderOperationResult<PreferredLanguageResponseModel>.TeleenaExceptionAsResult(2,
                nameof(businessUnitId), "InvalidInputValue - AccountId is Guid.Empty", Guid.NewGuid()
            ));

            mockPreferredLanguageProvider.Setup(p => p.GetAccountLanguagesAsync(It.IsAny<Guid>()))
            .Returns(responseResult);

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.GetBusinessUnitPreferredLanguages(businessUnitId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetBusinessUnitPreferredLanguages_RequestedBuIdWithEnabledPreferredLanguage_ShouldReturnOkResult()
        {
            var businessUnitId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var languageName = "English";
            var languageIsDefault = true;

            var preferredLanguage = new PreferredLanguageModel { LanguageId = languageId, Name = languageName, IsDefault = languageIsDefault };
            var result = new PreferredLanguageResponseModel
            {
                PreferredLanguages = new List<PreferredLanguageModel>() { preferredLanguage }
            };

            var responseResult = ProviderOperationResult<PreferredLanguageResponseModel>.OkResult(result);

            mockPreferredLanguageProvider.Setup(p => p.GetAccountLanguagesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(responseResult);

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.GetBusinessUnitPreferredLanguages(businessUnitId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<PreferredLanguageResponseModel>));
            var okResult = (NegotiatedContentResult<PreferredLanguageResponseModel>)response;
            Assert.IsTrue(okResult.Content.PreferredLanguages.Count > 0);
            var resultPreferredLanguage = okResult.Content.PreferredLanguages[0];
            Assert.AreEqual(languageId, resultPreferredLanguage.LanguageId);
            Assert.AreEqual(languageName, resultPreferredLanguage.Name);
            Assert.AreEqual(languageIsDefault, resultPreferredLanguage.IsDefault);
        }

        [TestMethod]
        public void GetBusinessUnitAvailableLanguages_ModelStateInvalid_ShouldReturnBadRequestForInvalidInputData()
        {
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            controllerUnderTest.ModelState.AddModelError("errorKey", "error 1 2 3 ");
            var accountId = Guid.NewGuid();

            var response = controllerUnderTest.GetBusinessUnitAvailableLanguages(accountId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetBusinessUnitAvailableLanguages_RequestedGuidEmpty_ShouldReturnBadRequestForInvalidInputData()
        {
            var businessUnitId = Guid.Empty;
            var responseResult = Task.Factory.StartNew(() => ProviderOperationResult<AvailableLanguagesResponseModel>.TeleenaExceptionAsResult(2,
                nameof(businessUnitId), "InvalidInputValue - CompanyId is Guid.Empty.", Guid.NewGuid()
            ));

            mockPreferredLanguageProvider.Setup(p => p.GetAvailableLanguagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(responseResult);

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.GetBusinessUnitAvailableLanguages(businessUnitId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void GetBusinessUnitAvailableLanguages_RequestedBuIdWithEnabledPreferredLanguage_ShouldReturnOkResult()
        {
            var businessUnitId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var languageName = "English";

            var availableLanguage = new AvailableLanguageModel { LanguageId = languageId, Name = languageName };
            var result = new AvailableLanguagesResponseModel
            {
                AvailableLanguages = new List<AvailableLanguageModel>() { availableLanguage }
            };

            var responseResult = ProviderOperationResult<AvailableLanguagesResponseModel>.OkResult(result);

            mockPreferredLanguageProvider.Setup(p => p.GetAvailableLanguagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(responseResult);

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.GetBusinessUnitAvailableLanguages(businessUnitId)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<AvailableLanguagesResponseModel>));
            var okResult = (NegotiatedContentResult<AvailableLanguagesResponseModel>)response;
            Assert.IsTrue(okResult.Content.AvailableLanguages.Count > 0);
            var resultAvailableLanguages = okResult.Content.AvailableLanguages[0];
            Assert.AreEqual(languageId, resultAvailableLanguages.LanguageId);
            Assert.AreEqual(languageName, resultAvailableLanguages.Name);

        }

        [TestMethod]
        public void UpdateBusinessUnitPreferredLanguages_ModelStateInvalid_ShouldReturnBadRequestForInvalidInputData()
        {
            var request = new UpdatePreferredLanguagesRequestModel();
            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);
            controllerUnderTest.ModelState.AddModelError("errorKey", "error 1 2 3 ");
            var accountId = Guid.NewGuid();

            var response = controllerUnderTest.UpdateBusinessUnitPreferredLanguages(accountId, request)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void UpdateBusinessUnitPreferredLanguages_ValidRequest_ShouldReturnOkResult()
        {
            var businessUnitId = Guid.NewGuid();
            var languageId = Guid.NewGuid();
            var languageIsDefault = true;

            var preferredLanguage = new UpdatePreferredLanguageModel { LanguageId = languageId, IsDefault = languageIsDefault };
            var request = new UpdatePreferredLanguagesRequestModel
            {
                PreferredLanguages = new List<UpdatePreferredLanguageModel>() { preferredLanguage }
            };

            var responseResult = ProviderOperationResult<AvailableLanguagesResponseModel>.OkResult();

            mockPreferredLanguageProvider.Setup(p => p.UpdateAccountLanguagesAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdatePreferredLanguagesRequestModel>()))
                .ReturnsAsync(responseResult);

            var controllerUnderTest = new BusinessUnitController(mockBusinessUnitProvider.Object, mockBusinessUnitCacheProvider.Object, mockBusinessUnitForCompanyCacheProvider.Object, mockAddAddOnProvider.Object, mockApnProvider.Object, mockSmscSettingsProvider.Object, mockFeatureProvider.Object, mockPreferredLanguageProvider.Object) { Logger = mockLogger.Object };
            SetupHiddenControllerDependencies(controllerUnderTest);

            var response = controllerUnderTest.UpdateBusinessUnitPreferredLanguages(businessUnitId, request)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<object>));
            var okResult = (NegotiatedContentResult<object>)response;
            Assert.IsTrue(okResult.StatusCode == HttpStatusCode.OK);
        }
    }
}
