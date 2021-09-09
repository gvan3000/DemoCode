using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;
using System.Collections.Generic;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class AddOnProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> _mockTeleenaServices;
        private Mock<AddOnService> _mockAddOnService;
        private Mock<IBusinessUnitApiTranslators> _mockTranslators;
        private AddAddOnResponseModel transaltorAddOnResponseMessageNull;
        private PurchaseAddOnResultContract _addOnResultContract;
        private Mock<IJsonRestApiLogger> _mockLogger;

        private PurchaseAddOnResultContract purchaseAddOnResultContract;
        private AddAddOnResponseModel addOnResponseNull;
        private AllowedAddOnsContract allowedAddOnContractResponse;
        private List<AddOnContract> addOnsContractResponse;
        private List<AddOnContract> addOnsContractResponseOneAddOn;
        private BusinessUnitAddOnsContract allowedAddOnsTranslate;
        private List<Guid> addOnIdsRequest;

        private int resourceId = 12;

        [TestInitialize]
        public void TestInitilization()
        {
            addOnResponseNull = new AddAddOnResponseModel { Fail = true, Message = null };
            purchaseAddOnResultContract = new PurchaseAddOnResultContract { Success = false, Message = null };
            _addOnResultContract = new PurchaseAddOnResultContract { Success = true, Message = "error message" };
            transaltorAddOnResponseMessageNull = new AddAddOnResponseModel { Fail = false, Message = string.Empty };
            allowedAddOnContractResponse = new AllowedAddOnsContract { AddOn = new AddOnContract { Id = Guid.NewGuid() } };
            addOnsContractResponse = new List<AddOnContract> { new AddOnContract { Id = Guid.NewGuid() } };
            allowedAddOnsTranslate = new BusinessUnitAddOnsContract { AllowedAddOns = new List<Guid>() { Guid.NewGuid() }, AccountId = Guid.NewGuid() };
            addOnIdsRequest = new List<Guid> { Guid.Empty, Guid.NewGuid() };
            addOnsContractResponseOneAddOn = new List<AddOnContract> { new AddOnContract { Id = Guid.NewGuid() } };

            _mockTeleenaServices = new Mock<ITeleenaServiceUnitOfWork>();
            _mockTranslators = new Mock<IBusinessUnitApiTranslators>();
            _mockAddOnService = new Mock<AddOnService>();

            _mockTeleenaServices.Setup(a => a.AddOnService).Returns(_mockAddOnService.Object);
            _mockTeleenaServices.Setup(x => x.AddOnService.GetAddOnByAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AddOnContract());
            _mockTeleenaServices.Setup(x => x.AddOnService.PurchaseGroupAddOnAsync(It.IsAny<PurchaseGroupAddOnContract>())).ReturnsAsync(_addOnResultContract);
            //_mockTeleenaServices.Setup(x => x.AddOnService.SaveAllowedBusinessUnitsForAsync(It.IsAny<AllowedAddOnsContract>())).ReturnsAsync(allowedAddOnContractResponse); // not used any longer
            _mockTeleenaServices.Setup(x => x.AddOnService.SaveAllowedAddOnsForBusinessUnitAsync(It.IsAny<BusinessUnitAddOnsContract>()))
                .ReturnsAsync(new AddOnSaveResultContract() { Success = true });
            _mockTeleenaServices.Setup(x => x.AddOnService.GetAddOnsAsync(It.IsAny<List<Guid>>())).ReturnsAsync(addOnsContractResponse);
            _mockTeleenaServices.Setup(x => x.AddOnService.GetAddOnsAsync(It.Is<List<Guid>>(a => a == addOnIdsRequest))).ReturnsAsync(addOnsContractResponseOneAddOn);
            _mockTeleenaServices.Setup(x => x.AddOnService.GetPurchasedAddOnsForAccountAsync(It.IsAny<Guid>())).ReturnsAsync(new AddOnsContract() { AddOnContracts = new List<AddOnContract>() });
            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<BusinessUnitAddOnMatrixxResourceContract>()
                {
                    new BusinessUnitAddOnMatrixxResourceContract()
                    {
                        ResourceId = resourceId
                    }
                });
            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitPurchasedAddOnContractAsync(It.IsAny<GetPurchasedAddonsBusinessUnitContract>()))
                .ReturnsAsync(new BusinessUnitPurchasedAddOnListContract()
                {
                    BusinessUnitPurchasedAddOns = new List<BusinessUnitPurchasedAddOnContract>()
                });

            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync
                (
                    new BusinessUnitAddOnCoupleContract()
                    {
                        Cancelled = false
                    }
                );
            _mockTeleenaServices.Setup(x => x.AddOnService.CancelAddOnAsync(It.IsAny<AddOnCancelParam>()))
                .ReturnsAsync
                (
                    new AddOnSaveResultContract()
                    {
                        Success = true
                    }
                );

            _mockTranslators.Setup(x => x.PurchaseAddOnResultTranslator.Translate(It.IsAny<PurchaseAddOnResultContract>())).Returns(transaltorAddOnResponseMessageNull);
            _mockTranslators.Setup(x => x.PurchaseAddOnResultTranslator.Translate(It.Is<PurchaseAddOnResultContract>(t => t.Message == purchaseAddOnResultContract.Message)))
                .Returns(addOnResponseNull);
            _mockTranslators.Setup(x => x.AllowedAddOnsContractTranslator.Translate(It.IsAny<List<Guid>>(), It.IsAny<Guid>())).Returns(allowedAddOnsTranslate);
            _mockTranslators.Setup(x => x.AddOnContractTranslator.Translate(It.IsAny<AddOnsContract>(), It.IsAny<List<BusinessUnitAddOnMatrixxResourceContract>>())).Returns(new AddOnListModel()
            {
                AddOns = new List<AddOnModel>()
            });

            _mockTranslators.Setup(x => x.DeleteAddOnModelTranslator.Translate(It.IsAny<DeleteAddOnModel>(), It.IsAny<Guid>()))
                .Returns(new AddOnCancelParam());
            _mockTranslators.Setup(x => x.BusinessUnitPurchasedAddOnsCumulativeTranslator.Translate(It.IsAny<BusinessUnitPurchasedAddOnListContract>()))
                .Returns(new AddOnCumulativeListModel { AddOns = new List<AddOnCumulativeModel> { new AddOnCumulativeModel { Id = Guid.NewGuid(), Name = "name1" } } });

            _mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);
        }

        [TestMethod]
        public void AddAddOn_WhenAddOnId_IsEmpty_ShouldReturnBadRequest()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var result = providerUnderTest.AddAddOnAsync(new PurchaseAddOnModel { AddOnId = Guid.Empty }, Guid.NewGuid(), Guid.NewGuid()).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
        }

        [TestMethod]
        public void AddAddOn_WhenAddOn_IsNull_ShouldReturnBadRequest()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var result = providerUnderTest.AddAddOnAsync(null, Guid.NewGuid(), Guid.NewGuid()).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
        }

        [TestMethod]
        public void AddAddOn_ShouldCall_PurchaseAddOnResultTranslator()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var result = providerUnderTest.AddAddOnAsync(new PurchaseAddOnModel { AddOnId = Guid.NewGuid() }, Guid.NewGuid(), Guid.NewGuid()).GetAwaiter().GetResult();

            _mockTranslators.Verify(t => t.PurchaseAddOnResultTranslator.Translate(It.IsAny<PurchaseAddOnResultContract>()), Times.Once);
        }

        [TestMethod]
        public void AddAddOn_ShouldCall_AddOnService_PurchaseGroupAddOnAsync()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.AddAddOnAsync(new PurchaseAddOnModel { AddOnId = Guid.NewGuid() }, Guid.NewGuid(), Guid.NewGuid()).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.AddOnService.PurchaseGroupAddOnAsync(It.IsAny<PurchaseGroupAddOnContract>()), Times.Once);
        }

        [TestMethod]
        public void AddAllowedAddOnsToBusinessUnit_WhenListOfAddOnsIsEmpty_ShouldReturnOkAndNotCallService()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.AddAllowedAddOnsToBusinessUnit(new List<Guid> { }, Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsTrue(response.IsSuccess);
            _mockTeleenaServices.Verify(x => x.AddOnService.SaveAllowedAddOnsForBusinessUnitAsync(It.IsAny<BusinessUnitAddOnsContract>()), Times.Never);
        }

        [TestMethod]
        public void AddAllowedAddOnsToBusinessUnit_WhenListOfAddOnsIsNull_ShouldReturnOkANdNotCallService()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.AddAllowedAddOnsToBusinessUnit(new List<Guid> { }, Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsTrue(response.IsSuccess);
            _mockTeleenaServices.Verify(x => x.AddOnService.SaveAllowedAddOnsForBusinessUnitAsync(It.IsAny<BusinessUnitAddOnsContract>()), Times.Never);
        }

        [TestMethod]
        public void AddAllowedAddOnsToBusinessUnit_WhenListOfAddOnsIsProvided_ShouldCall_AllowedAddOnsContractTranslator()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.AddAllowedAddOnsToBusinessUnit(new List<Guid> { Guid.NewGuid() }, Guid.NewGuid(), Guid.NewGuid());

            _mockTranslators.Verify(x => x.AllowedAddOnsContractTranslator.Translate(It.IsAny<List<Guid>>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void AddAllowedAddOnsToBusinessUnit_WhenListOfAddOnsIsProvided_ShouldCall_AddOnService_SaveAllowedAddOnsForBusinessUnitAsync()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.AddAllowedAddOnsToBusinessUnit(new List<Guid> { Guid.NewGuid() }, Guid.NewGuid(), Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.AddOnService.SaveAllowedAddOnsForBusinessUnitAsync(It.IsAny<BusinessUnitAddOnsContract>()), Times.Once);
        }

        [TestMethod]
        public void ValidateAddOns_ShouldCall_AddOnService_GetAddOnsAsync()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.ValidateAddOns(new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

            _mockTeleenaServices.Verify(x => x.AddOnService.GetAddOnsAsync(It.IsAny<List<Guid>>()), Times.Once);
        }

        [TestMethod]
        public void ValidateAddOns_WhenProvidedAddOnIdsAndFetchedAddOns_AreNotTheSameNumber_ShouldReturnFalse()
        {

            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var response = providerUnderTest.ValidateAddOns(addOnIdsRequest);

            Assert.IsFalse(response.Result);
        }

        [TestMethod]
        public void DeleteAddOnAsync_ShouldReturnAcceptedResult()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);
            DeleteAddOnModel model = new DeleteAddOnModel()
            {
                ResourceId = resourceId
            };

            var result = providerUnderTest.DeleteAddOnAsync(model, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Accepted, result.HttpResponseCode);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.CancelAddOnAsync(It.IsAny<AddOnCancelParam>()), Times.Once);
        }

        [TestMethod]
        public void DeleteAddOnAsync_ShouldReturnInvalidInputIfNoMatrixData()
        {
            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<BusinessUnitAddOnMatrixxResourceContract>());
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);
            DeleteAddOnModel model = new DeleteAddOnModel()
            {
                ResourceId = resourceId
            };

            var result = providerUnderTest.DeleteAddOnAsync(model, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockTeleenaServices.Verify(x => x.AddOnService.CancelAddOnAsync(It.IsAny<AddOnCancelParam>()), Times.Never);
        }

        [TestMethod]
        public void DeleteAddOnAsync_ShouldReturnInvalidInputIfResourceIdDoesNotExists()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);
            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<BusinessUnitAddOnMatrixxResourceContract>()
                { new BusinessUnitAddOnMatrixxResourceContract()
                    {
                        ResourceId = resourceId + 5
                    }
                });

            DeleteAddOnModel model = new DeleteAddOnModel()
            {
                ResourceId = resourceId
            };

            var result = providerUnderTest.DeleteAddOnAsync(model, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockTeleenaServices.Verify(x => x.AddOnService.CancelAddOnAsync(It.IsAny<AddOnCancelParam>()), Times.Never);
        }

        [TestMethod]
        public void DeleteAddOnAsync_ShouldReturnForbiddenIfCoupleIsCanceld()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);
            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync
                (new BusinessUnitAddOnCoupleContract()
                {
                    Cancelled = true
                });

            DeleteAddOnModel model = new DeleteAddOnModel()
            {
                ResourceId = resourceId
            };

            var result = providerUnderTest.DeleteAddOnAsync(model, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Forbidden, result.HttpResponseCode);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitAddOnCoupleByIdAsync(It.IsAny<Guid>()), Times.Once);
            _mockTeleenaServices.Verify(x => x.AddOnService.CancelAddOnAsync(It.IsAny<AddOnCancelParam>()), Times.Never);
        }

        [TestMethod]
        public void GetGetCumulativeAddOnsAsync_ShouldCallAddOnService_GetBusinessUnitPurchasedAddOnContractAsync()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var getPurchasedAddonsRequest = new GetPurchasedAddonsBusinessUnitRequest()
            {
                BusinessUnitId = Guid.NewGuid(), IncludeExpired = false
            };

            var response = providerUnderTest.GetAddOnsAsync(getPurchasedAddonsRequest).GetAwaiter().GetResult();

            _mockTeleenaServices.Verify(x => x.AddOnService.GetBusinessUnitPurchasedAddOnContractAsync(It.IsAny<GetPurchasedAddonsBusinessUnitContract>()), Times.Once);
        }


        [TestMethod]
        public void GetGetCumulativeAddOnsAsync_ShouldreturnNull_IfServiceReturnsNull()
        {
            BusinessUnitPurchasedAddOnListContract result = null;

            _mockTeleenaServices.Setup(x => x.AddOnService.GetBusinessUnitPurchasedAddOnContractAsync(It.IsAny<GetPurchasedAddonsBusinessUnitContract>())).ReturnsAsync(result);
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var getPurchasedAddonsRequest = new GetPurchasedAddonsBusinessUnitRequest()
            {
                BusinessUnitId = Guid.NewGuid(),
                IncludeExpired = false
            };

            var response = providerUnderTest.GetAddOnsAsync(getPurchasedAddonsRequest).GetAwaiter().GetResult();

            Assert.IsNull(response);
        }

        [TestMethod]
        public void GetCumulativeAddOnsAsync_ShpuldCall_BusinessUnitPurchasedAddOnsCumulativeTranslator()
        {
            var providerUnderTest = new AddOnProvider(_mockLogger.Object, _mockTeleenaServices.Object, _mockTranslators.Object);

            var getPurchasedAddonsRequest = new GetPurchasedAddonsBusinessUnitRequest()
            {
                BusinessUnitId = Guid.NewGuid(),
                IncludeExpired = false
            };

            var response = providerUnderTest.GetAddOnsAsync(getPurchasedAddonsRequest).GetAwaiter().GetResult();

            _mockTranslators.Verify(x => x.BusinessUnitPurchasedAddOnsCumulativeTranslator.Translate(It.IsAny<BusinessUnitPurchasedAddOnListContract>()), Times.Once);
        }
    }
}
