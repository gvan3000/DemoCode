using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System.Net;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class APNProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> mockServices;
        private Mock<IJsonRestApiLogger> mockLogger;
        private Mock<IBusinessUnitApiTranslators> mockTranslators;
        private Mock<IBusinessUnitValidators> mockValidators;

        Guid apnNameValidationFalse;
        Guid apnNameValidationTrue;
        Guid businessUnitIdSetExc;

        [TestInitialize]
        public void SetupEachTest()
        {
            apnNameValidationFalse = Guid.NewGuid();
            apnNameValidationTrue = Guid.NewGuid();
            businessUnitIdSetExc = Guid.NewGuid();
            mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);

            mockServices = new Mock<ITeleenaServiceUnitOfWork>();
            mockServices.Setup(x => x.ApnService.GetApnSetsWithDetailsForCompanyAsync(It.IsAny<GetApnSetsWithDetailsForCompanyRequestContract>()))
                .ReturnsAsync(new ApnSetWithDetailsContract[0]);
            mockServices.Setup(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()))
                .Returns(Task.FromResult(0));
            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                .ReturnsAsync(new ApnDetailContract[0]);
            mockServices.Setup(x => x.ApnService.SetApnDetailsForAccountAsync(It.Is<SetApnDetailsForAccountRequestContract>(c => c.AccountId == businessUnitIdSetExc)))
                .Throws(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp()));

            mockTranslators = new Mock<IBusinessUnitApiTranslators>(MockBehavior.Strict);
            mockTranslators.Setup(x => x.UpdateApnsTranslator.Translate(It.IsAny<UpdateBusinessUnitApnsModel>(), It.IsAny<ApnSetWithDetailsContract[]>()))
                .Returns(new SetApnDetailsForAccountRequestContract());
            mockTranslators.Setup(x => x.UpdateDefaultApnTranslator.Translate(It.IsAny<UpdateBusinessUnitDefaultApnModel>(), It.IsAny<ApnDetailContract[]>()))
                .Returns(new SetApnDetailsForAccountRequestContract());
                

            mockTranslators.Setup(x => x.APNTranslator.Translate(It.IsAny<ApnSetWithDetailsContract[]>()))
                .Returns(new APNSetList());

            var defaultApn = Guid.NewGuid();

            var validApnResponseModel = new APNsResponseModel()
            {
                Apns = new List<APNResponseDetail>()
                { new APNResponseDetail() { Name = "apn1", Id = defaultApn },
                  new APNResponseDetail() { Name = "apn2", Id = Guid.NewGuid() }
                },
                DefaultApn = defaultApn
            };

            mockTranslators.Setup(x => x.ApnsResponseModelTranslator.Translate(It.IsAny<ApnDetailContract[]>()))
                .Returns(validApnResponseModel);

            mockTranslators.Setup(x => x.DeleteApnTranslator.Translate(It.IsAny<ApnDetailContract[]>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(new SetApnDetailsForAccountRequestContract
                {
                    AccountId = Guid.NewGuid(),
                    ApnDetails = new ApnDetailContract[]
                                 {
                                     new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = true, Name = "test_t_gprs" },
                                     new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = false, Name = "t_gprs.tst1" },
                                 }
                });

            mockTranslators.Setup(x => x.DeleteApnTranslator.Translate(It.IsAny<ApnDetailContract[]>(), It.IsAny<Guid>(), It.Is<Guid>(b => b == businessUnitIdSetExc)))
                .Returns(new SetApnDetailsForAccountRequestContract
                {
                    AccountId = businessUnitIdSetExc,
                    ApnDetails = new ApnDetailContract[]
                                 {
                                     new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = true, Name = "test.test111.com" }                                     
                                 }
                });

            mockValidators = new Mock<IBusinessUnitValidators>(MockBehavior.Strict);
            mockValidators.Setup(x => x.UpdateBusinessUnitApnsValidator.ValidateModel(It.IsAny<UpdateBusinessUnitApnsModel>(), It.IsAny<ApnSetWithDetailsContract[]>()))
                .Returns(Common.ProviderOperationResult<object>.OkResult());
            mockValidators.Setup(x => x.DeleteApnValidator.Validate(It.IsAny<List<ApnDetailContract>>(), It.IsAny<Guid>()))
                .Returns(ProviderOperationResult<object>.OkResult());
            mockValidators.Setup(x => x.DeleteApnValidator.Validate(It.IsAny<List<ApnDetailContract>>(), It.Is<Guid>(n => n == apnNameValidationFalse)))
                .Returns(ProviderOperationResult<object>.InvalidInput("businessUnitId", "validation failed"));
            mockValidators.Setup(x => x.DeleteApnValidator.Validate(It.IsAny<List<ApnDetailContract>>(), It.Is<Guid>(n => n == apnNameValidationTrue)))
                .Returns(ProviderOperationResult<object>.OkResult());
            mockValidators.Setup(x => x.UpdateBusinessUnitDefaultApnValidator.ValidateModel(It.IsAny<UpdateBusinessUnitDefaultApnModel>(), It.IsAny<ApnDetailContract[]>()))
                .Returns(ProviderOperationResult<object>.OkResult());
        }

        [TestMethod]
        public void GetAvailableAPNsForCompany_ShouldReturnNull_WhenServiceReturnsNull()
        {
            var BusinessUnitId = Guid.NewGuid();
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);
            var response = providerUnderTest.GetCompanyAPNsAsync(BusinessUnitId).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(response.APNSets);
        }

        [TestMethod]
        public void GetAvailableAPNsForCompany_ShouldCallServiceMethodAndThenTranslatorAndSucceedForValidInput()
        {
            var BusinessUnitId = Guid.NewGuid();
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);
            var response = providerUnderTest.GetCompanyAPNsAsync(BusinessUnitId).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);

            mockServices.Verify(x => x.ApnService.GetApnSetsWithDetailsForCompanyAsync(It.IsAny<GetApnSetsWithDetailsForCompanyRequestContract>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateBusinessUnitApnsAsync_ShouldThrowWhenInputIsNull()
        {
            var updateModel = default(UpdateBusinessUnitApnsModel);
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitApnsAsync_ShouldGetAllApnsForCompanyAndValidateInputList()
        {
            var updateModel = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);

            mockServices.Verify(x => x.ApnService.GetApnSetsWithDetailsForCompanyAsync(It.IsAny<GetApnSetsWithDetailsForCompanyRequestContract>()), Times.Once);
            mockValidators.Verify(x => x.UpdateBusinessUnitApnsValidator.ValidateModel(It.IsAny<UpdateBusinessUnitApnsModel>(), It.IsAny<ApnSetWithDetailsContract[]>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitApnsAsync_ShouldReturnValidationErrorWhenInputIsNotValid()
        {
            var updateModel = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            mockValidators.Setup(x => x.UpdateBusinessUnitApnsValidator.ValidateModel(It.IsAny<UpdateBusinessUnitApnsModel>(), It.IsAny<ApnSetWithDetailsContract[]>()))
                .Returns(Common.ProviderOperationResult<object>.InvalidInput("bla", "some error"));

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);

            mockServices.Verify(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitApnsAsync_ShouldCallTranslatorAndUpdateServiceMethod()
        {
            var updateModel = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);

            mockTranslators.Verify(x => x.UpdateApnsTranslator.Translate(It.IsAny<UpdateBusinessUnitApnsModel>(), It.IsAny<ApnSetWithDetailsContract[]>()), Times.Once);
            mockServices.Verify(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitApnsAsync_ShouldReturnValidationErrorWhenServiceFaultsWithAppropriateErrorCode()
        {
            var updateModel = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            mockServices.Setup(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()))
                .Throws(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp() { ErrorCode = (int)ErrorCode.InvalidInputValue }));

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitApnsAsync_ShouldReturnSuccessWhenprocessFinishedWithoutErrors()
        {
            var updateModel = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitApnsAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void GetListOfBusinessUnitAPNs_ShouldReturnResult()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = providerUnderTest.GetAPNsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void GetListOfBusinessUnitAPNs_ShouldCallTranslatorAndApnServiceOnce()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = providerUnderTest.GetAPNsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);

            mockTranslators.Verify(x => x.ApnsResponseModelTranslator.Translate(It.IsAny<ApnDetailContract[]>()), Times.Once);

            mockServices.Verify(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public void GetListOfBusinessUnitAPNs_ShouldReturnNull_WhenServiceReturnsNull()
        {
            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                    .ReturnsAsync(default(ApnDetailContract[]));
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);
            var result = providerUnderTest.GetAPNsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetListOfBusinessUnitAPNs_ShouldNotSuccess_WhenServiceReturnsError()
        {
            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                .Throws(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp()));
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);
            var result = providerUnderTest.GetAPNsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldThrowWhenInputIsNull()
        {
            var updateModel = default(UpdateBusinessUnitDefaultApnModel);
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel)
                .ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldCallGetApnDetailsForAccountAsync()
        {
            var updateModel = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);

            mockServices.Verify(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldCallTranslatorAndUpdateServiceMethod()
        {
            string validApnName = "validAPNName";
            var updateModel = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            var apnDetailsContract = new ApnDetailContract[] { new ApnDetailContract() { Name = validApnName } };

            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                 .ReturnsAsync(apnDetailsContract);

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);

            mockTranslators.Verify(x => x.UpdateDefaultApnTranslator.Translate(It.IsAny<UpdateBusinessUnitDefaultApnModel>(), It.IsAny<ApnDetailContract[]>()), Times.Once);
            mockServices.Verify(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldReturnValidationErrorWhenServiceFaultsWithAppropriateErrorCode()
        {
            var updateModel = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            mockServices.Setup(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()))
                .Throws(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp() { ErrorCode = (int)ErrorCode.InvalidInputValue }));

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }


        [TestMethod]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldReturnSuccessWhenFinishedWithoutErrors()
        {
            string validApnName = "validAPNName";
            var updateModel = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            var apnDetailsContract = new ApnDetailContract[] { new ApnDetailContract() { Name = validApnName } };

            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                 .ReturnsAsync(apnDetailsContract);

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task UpdateBusinessUnitDefaultApnAsync_ShouldReturnFailureIfValidatorReturnsFailure()
        {
            string validApnName = "validAPNName";
            var updateModel = new UpdateBusinessUnitDefaultApnModel()
            {
                Id = Guid.NewGuid()
            };

            var apnDetailsContract = new ApnDetailContract[] { new ApnDetailContract() { Name = validApnName } };

            mockValidators.Setup(x => x.UpdateBusinessUnitDefaultApnValidator.ValidateModel(It.IsAny<UpdateBusinessUnitDefaultApnModel>(), It.IsAny<ApnDetailContract[]>()))
                .Returns(ProviderOperationResult<object>.InvalidInput("bla", "some error"));

            mockServices.Setup(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()))
                 .ReturnsAsync(apnDetailsContract);

            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.UpdateBusinessUnitDefaultApnAsync(Guid.NewGuid(), Guid.NewGuid(), updateModel).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Call_ApnService_GetApnDetailsForAccountAsync()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationTrue).ConfigureAwait(false);

            mockServices.Verify(x => x.ApnService.GetApnDetailsForAccountAsync(It.IsAny<GetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Call_DeleteApnValidator()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationTrue).ConfigureAwait(false);

            mockValidators.Verify(x => x.DeleteApnValidator.Validate(It.IsAny<List<ApnDetailContract>>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_RetrunBadRequest_If_ValidationReturnsNotSuccess()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationFalse).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Call_DeleteApnTranslator()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationTrue).ConfigureAwait(false);

            mockTranslators.Verify(x => x.DeleteApnTranslator.Translate(It.IsAny<ApnDetailContract[]>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Call_SetApnDetailsForAccountAsync()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationTrue).ConfigureAwait(false);

            mockServices.Verify(x => x.ApnService.SetApnDetailsForAccountAsync(It.IsAny<SetApnDetailsForAccountRequestContract>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Call_Logger_Log_IfServiceThrowsException()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(businessUnitIdSetExc, apnNameValidationTrue).ConfigureAwait(false);

            mockLogger.Verify(x => x.LogException(It.IsAny<LogSeverity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveApnAsync_Should_Return_()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(businessUnitIdSetExc, apnNameValidationTrue).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.InternalServerError, result.HttpResponseCode);
        }

        [TestMethod]
        public async Task RemoveApnAsync_ShouldReturn_Okresult()
        {
            var providerUnderTest = new APNProvider(mockLogger.Object, mockServices.Object, mockTranslators.Object, mockValidators.Object);

            var result = await providerUnderTest.RemoveApnAsync(Guid.NewGuid(), apnNameValidationTrue).ConfigureAwait(false);

            Assert.AreEqual(HttpStatusCode.OK, HttpStatusCode.OK);
        }
    }
}
