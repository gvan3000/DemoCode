using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using TeleenaLogging.Abstraction;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.NotificationsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class NotificationConfigurationProviderUT
    {
        NotificationConfigurationProvider _providerUnderTest;

        Mock<IJsonRestApiLogger> _loggerMock;
        Mock<ITeleenaServiceUnitOfWork> _serviceUnitOfWorkMock;
        Mock<IBusinessUnitApiTranslators> _businessUnitTranslatorsMock;
        Mock<IBusinessUnitValidators> _businessUnitValidatorsMock;

        Guid _businessUnitIdSuccess;
        Guid _businessUnitIdNotSuccess;
        Guid _businessUnitIdException;


        [TestInitialize]
        public void Setup()
        {
            _businessUnitIdException = Guid.NewGuid();
            _businessUnitIdNotSuccess = Guid.NewGuid();
            _businessUnitIdSuccess = Guid.NewGuid();

            _loggerMock = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);

            _serviceUnitOfWorkMock = new Mock<ITeleenaServiceUnitOfWork>();
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.GetBusinessUnitNotificationConfigurationAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetNotificationConfigurationListResponse
                {
                    Success = true,
                    NotificationsConfiguration = new GetNotificationConfigurationResponse[]
                    {
                        new GetNotificationConfigurationResponse
                        {
                            Id = Guid.NewGuid(),
                            MvnoId = "mvnoiD-12",
                            Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                            {
                                new TeleenaServiceReferences.NotificationConfigurationService.Delivery { DeliveryMethod = DeliveryMethodEnum.Email, DeliveryValue = "asdadsa" }
                            }
                        }
                    }
                });

            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.DeleteBusinessUnitNotificationConfigurationAsync(It.Is<Guid>(b => b == _businessUnitIdNotSuccess), It.IsAny<string>()))
                .ReturnsAsync(new DeleteNotificationConfigurationResponse { Success = false, ErrorMessage = "Validation random error message" });
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.DeleteBusinessUnitNotificationConfigurationAsync(It.Is<Guid>(b => b == _businessUnitIdSuccess), It.IsAny<string>()))
                .ReturnsAsync(new DeleteNotificationConfigurationResponse { Success = true, ErrorMessage = string.Empty });
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.DeleteBusinessUnitNotificationConfigurationAsync(It.Is<Guid>(b => b == _businessUnitIdException), It.IsAny<string>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp { ErrorCode = 1, ErrorDescription = "error desc", TicketId = Guid.NewGuid() }));

            _businessUnitTranslatorsMock = new Mock<IBusinessUnitApiTranslators>();

            _businessUnitTranslatorsMock.Setup(x => x.NotificationListDataModelTranslator.Translate(It.IsAny<GetNotificationConfigurationListResponse>()))
                .Returns(new Domain.Models.NotificationModels.GetNotificationListDataModel
                {
                    Notifications = new List<Domain.Models.NotificationModels.GetNotificationDataModel>
                    {
                        new Domain.Models.NotificationModels.GetNotificationDataModel
                        {
                            Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE, Deliveries = new List<Domain.Models.NotificationModels.Delivery>
                            {
                                new Domain.Models.NotificationModels.Delivery { DeliveryValue = "123", DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.HTTP }
                            }
                        }
                    }
                });
            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationResponseTranslator.Translate(It.IsAny<CreateBusinessUnitNotificationConfigurationResult>()))
                .Returns(new CreateNotificationModelResponse());

            _businessUnitTranslatorsMock.Setup(x => x.NotificationTypeTranslator)
                .Returns(new TeleenaServiceReferences.Translators.NotificationTypeTranslator());

            TeleenaServiceReferences.NotificationConfigurationService.Delivery[] mockDeliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
            {
                new TeleenaServiceReferences.NotificationConfigurationService.Delivery { DeliveryMethod = DeliveryMethodEnum.Email, DeliveryValue = "test@test.com" },
                new TeleenaServiceReferences.NotificationConfigurationService.Delivery { DeliveryMethod = DeliveryMethodEnum.HTTPS, DeliveryValue = "www.teleena.com" }
            };

            _businessUnitTranslatorsMock.Setup(x => x.UpdateNotificationConfigurationTranslator.Translate(It.IsAny<UpdateNotificationModel>()))
                .Returns(new UpdateBusinessUnitNotificationConfigurationContract
                {
                    Type = "type",
                    BusinessUnitId = Guid.NewGuid(),
                    Deliveries = mockDeliveries
                });

            _businessUnitValidatorsMock = new Mock<IBusinessUnitValidators>();
            _businessUnitValidatorsMock.Setup(x => x.UpdateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<UpdateNotificationModel>()))
                .Returns(Common.ProviderOperationResult<object>.AcceptedResult());

            _providerUnderTest = new NotificationConfigurationProvider(_loggerMock.Object,
                                                                       _serviceUnitOfWorkMock.Object,
                                                                       _businessUnitTranslatorsMock.Object,
                                                                       _businessUnitValidatorsMock.Object);
        }

        [TestMethod]
        public void GetBusinessUnitNotificationsAsync_ShouldCall_NotificationConfigurationService_GetBusinessUnitNotificationConfigurationAsync()
        {
            var response = _providerUnderTest.GetBusinessUnitNotificationsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _serviceUnitOfWorkMock.Verify(x => x.NotificationConfigurationService.GetBusinessUnitNotificationConfigurationAsync(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void GetBusinessUnitNotificationsAsync_ShouldCallLogger_Log_When_ServiceThworsAnException()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.GetBusinessUnitNotificationConfigurationAsync(It.IsAny<Guid>()))
                 .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp() { ErrorCode = 1, TicketId = Guid.NewGuid() }));

            var response = _providerUnderTest.GetBusinessUnitNotificationsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _loggerMock.Verify(x => x.LogException(It.IsAny<LogSeverity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Once());
        }

        [TestMethod]
        public void GetBusinessUnitNotificationsAsync_ShouldReturn_BadRequest_When_ResponseReturns_Success_False()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.GetBusinessUnitNotificationConfigurationAsync(It.IsAny<Guid>()))
                 .ReturnsAsync(new GetNotificationConfigurationListResponse { Success = false, ErrorMessage = "error" });

            var response = _providerUnderTest.GetBusinessUnitNotificationsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(response.HttpResponseCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void GetBusinessUnitNotificationsAsync_ShouldCall_NotificationListDataModelTranslator_Translate()
        {
            var response = _providerUnderTest.GetBusinessUnitNotificationsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _businessUnitTranslatorsMock.Verify(x => x.NotificationListDataModelTranslator.Translate(It.IsAny<GetNotificationConfigurationListResponse>()), Times.Once());
        }

        [TestMethod]
        public void GetBusinessUnitNotificationsAsync_ShouldReturn_OkResult()
        {
            var response = _providerUnderTest.GetBusinessUnitNotificationsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, response.HttpResponseCode);
        }

        [TestMethod]
        public void DeleteMethod_ShouldCall_NotificationConfigurationService_DeleteBusinessUnitNotificationConfigurationAsync()
        {
            var response = _providerUnderTest.DeleteBusinessUnitNotificationAsync(_businessUnitIdSuccess, TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE_PCRF).ConfigureAwait(false).GetAwaiter().GetResult();

            _serviceUnitOfWorkMock.Verify(x => x.NotificationConfigurationService
                        .DeleteBusinessUnitNotificationConfigurationAsync(It.Is<Guid>(b => b == _businessUnitIdSuccess), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void DeleteMethod_ShouldCallLogger_Log_When_NotificationConfigurationService_ThrowsAnException()
        {
            var response = _providerUnderTest.DeleteBusinessUnitNotificationAsync(_businessUnitIdException, TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE_DATA).ConfigureAwait(false).GetAwaiter().GetResult();

            _loggerMock.Verify(x => x.LogException(It.IsAny<LogSeverity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Once());
        }

        [TestMethod]
        public void DeleteMethod_ShouldCall_Log_When_NotificationConfigurationService_ReturnResponse_NotSuccess()
        {
            var response = _providerUnderTest.DeleteBusinessUnitNotificationAsync(_businessUnitIdException, TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE_DATA).ConfigureAwait(false).GetAwaiter().GetResult();

            _loggerMock.Verify(x => x.LogException(It.IsAny<LogSeverity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>()), Times.Once());
        }

        [TestMethod]
        public void DeleteMethod_ShouldReturn_Accepted()
        {
            var response = _providerUnderTest.DeleteBusinessUnitNotificationAsync(_businessUnitIdSuccess, TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE_DATA).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Accepted, response.HttpResponseCode);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateBusinessUnitNotificationAsync_ShouldNotAcceptNullArgument()
        {
            UpdateNotificationModel input = null;
            var result = _providerUnderTest.UpdateBusinessUnitNotificationAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateBusinessUnitNotifiactionAsync_ShouldCallTranslatorAndService()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(It.IsAny<UpdateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(new UpdateNotificationConfigurationResultContract() { IsSuccess = true });

            var input = new UpdateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod  = DeliveryMethod.Email,
                        DeliveryValue = "bla"
                    }
                }
            };

            var result = _providerUnderTest.UpdateBusinessUnitNotificationAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);

            _serviceUnitOfWorkMock.Verify(x => x.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(It.IsAny<UpdateBusinessUnitNotificationConfigurationContract>()), Times.Once);
            _businessUnitTranslatorsMock.Verify(x => x.UpdateNotificationConfigurationTranslator.Translate(It.IsAny<UpdateNotificationModel>()), Times.Once);
        }

        [TestMethod]
        public void UpdateBusinessUnitNotifiactionAsync_ShouldReturnErrorWhenServiceFaults()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(It.IsAny<UpdateBusinessUnitNotificationConfigurationContract>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp() { TicketId = Guid.NewGuid() }));

            var input = new UpdateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery
                    {
                        DeliveryMethod  = DeliveryMethod.Email,
                        DeliveryValue = "bla"
                    }
                }
            };

            var result = _providerUnderTest.UpdateBusinessUnitNotificationAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateBusinessUnitNotifiactionAsync_ShouldThrowWhenServiceReturnsNull()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(It.IsAny<UpdateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(default(UpdateNotificationConfigurationResultContract));

            var input = new UpdateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod  = DeliveryMethod.Email,
                        DeliveryValue = "bla"
                    }
                }
            };

            var result = _providerUnderTest.UpdateBusinessUnitNotificationAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateBusinessUnitNotifiactionAsync_ShouldPropagateValidationErrorFromService()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(It.IsAny<UpdateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(new UpdateNotificationConfigurationResultContract() { IsSuccess = false, ErrorMessage = "error" });

            var input = new UpdateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery
                    {
                        DeliveryMethod  = DeliveryMethod.Email,
                        DeliveryValue = "bla"
                    }
                }
            };

            var result = _providerUnderTest.UpdateBusinessUnitNotificationAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(result.ErrorMessage, "error");
            Assert.AreEqual(result.HttpResponseCode, System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldCallTranslatorToGetServiceContract()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.IsAny<CreateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(new CreateBusinessUnitNotificationConfigurationResult { CreationSucceeded = true });

            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationTranslator.Translate(It.IsAny<CreateNotificationModel>()))
                .Returns(new CreateBusinessUnitNotificationConfigurationContract()
                {
                    ProcessId = "bla",
                    Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                    {
                        new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                        {
                            DeliveryMethod = DeliveryMethodEnum.HTTP,
                            DeliveryValue = "truc",
                            DeliveryOptions = new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                            {
                                Type = DeliveryOptionsEnumDeliveryOptionsType.Basis,
                                Username = "456123",
                                Password = "5647321"
                            }
                        }
                    }
                });
            _businessUnitValidatorsMock.Setup(x => x.CreateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<CreateNotificationModel>()))
                .Returns(ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse()));

            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.LOWBALANCE_SMS,
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod = DeliveryMethod.HTTP,
                        DeliveryValue = "test.me",
                        DeliveryOptions = new Domain.Models.NotificationModels.DeliveryOption()
                        {
                            Username = "123456",
                            Password = "987654",
                            Type = DeliveryOptionsType.Basic
                        }
                    }
                }
            };

            var buId = Guid.NewGuid();

            var result = await _providerUnderTest.CreateNotificationAsync(buId, input);

            Assert.IsNotNull(result);

            _businessUnitTranslatorsMock.Verify(x => x.CreateNotificationTranslator.Translate(It.Is<CreateNotificationModel>(contract => contract == input)), Times.Once);
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldCallServiceToCreateNotificationIncludeBusinessUnitIdInContractAndReturnOkResult()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.IsAny<CreateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(new CreateBusinessUnitNotificationConfigurationResult { CreationSucceeded = true });

            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationTranslator.Translate(It.IsAny<CreateNotificationModel>()))
                .Returns(new CreateBusinessUnitNotificationConfigurationContract()
                {
                    ProcessId = "bla",
                    Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                    {
                        new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                        {
                            DeliveryMethod = DeliveryMethodEnum.HTTP,
                            DeliveryValue = "truc",
                            DeliveryOptions = new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                            {
                                Type = DeliveryOptionsEnumDeliveryOptionsType.Basis,
                                Username = "456123",
                                Password = "5647321"
                            }
                        }
                    }
                });
            _businessUnitValidatorsMock.Setup(x => x.CreateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<CreateNotificationModel>()))
                .Returns(ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse()));

            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.LOWBALANCE_SMS,
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod = DeliveryMethod.HTTP,
                        DeliveryValue = "test.me",
                        DeliveryOptions = new Domain.Models.NotificationModels.DeliveryOption()
                        {
                            Username = "123456",
                            Password = "987654",
                            Type = DeliveryOptionsType.Basic
                        }
                    }
                }
            };

            var buId = Guid.NewGuid();

            var result = await _providerUnderTest.CreateNotificationAsync(buId, input);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseCode);

            _serviceUnitOfWorkMock.Verify(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.Is<CreateBusinessUnitNotificationConfigurationContract>(contract => contract.AccountId == buId && contract.ProcessId == "bla")), Times.Once);
            _businessUnitTranslatorsMock.Verify(x => x.CreateNotificationResponseTranslator.Translate(It.IsAny<CreateBusinessUnitNotificationConfigurationResult>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldReturnErrorIfServiceReturnsError()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.IsAny<CreateBusinessUnitNotificationConfigurationContract>()))
                .ReturnsAsync(new CreateBusinessUnitNotificationConfigurationResult { CreationSucceeded = false, ErrorMessage = "bla error" });

            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationTranslator.Translate(It.IsAny<CreateNotificationModel>()))
                .Returns(new CreateBusinessUnitNotificationConfigurationContract()
                {
                    ProcessId = "bla",
                    Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                    {
                        new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                        {
                            DeliveryMethod = DeliveryMethodEnum.HTTP,
                            DeliveryValue = "truc",
                            DeliveryOptions = new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                            {
                                Type = DeliveryOptionsEnumDeliveryOptionsType.Basis,
                                Username = "456123",
                                Password = "5647321"
                            }
                        }
                    }
                });
            _businessUnitValidatorsMock.Setup(x => x.CreateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<CreateNotificationModel>()))
                .Returns(ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse()));

            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.LOWBALANCE_SMS,
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod = DeliveryMethod.HTTP,
                        DeliveryValue = "test.me",
                        DeliveryOptions = new Domain.Models.NotificationModels.DeliveryOption()
                        {
                            Username = "123456",
                            Password = "987654",
                            Type = DeliveryOptionsType.Basic
                        }
                    }
                }
            };

            var buId = Guid.NewGuid();

            var result = await _providerUnderTest.CreateNotificationAsync(buId, input);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldReturnErrorIfServiceFaults()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.IsAny<CreateBusinessUnitNotificationConfigurationContract>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp()));

            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationTranslator.Translate(It.IsAny<CreateNotificationModel>()))
                .Returns(new CreateBusinessUnitNotificationConfigurationContract()
                {
                    ProcessId = "bla",
                    Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                    {
                        new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                        {
                            DeliveryMethod = DeliveryMethodEnum.HTTP,
                            DeliveryValue = "truc",
                            DeliveryOptions = new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                            {
                                Type = DeliveryOptionsEnumDeliveryOptionsType.Basis,
                                Username = "456123",
                                Password = "5647321"
                            }
                        }
                    }
                });
            _businessUnitValidatorsMock.Setup(x => x.CreateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<CreateNotificationModel>()))
                .Returns(ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse()));

            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.LOWBALANCE_SMS,
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod = DeliveryMethod.HTTP,
                        DeliveryValue = "test.me",
                        DeliveryOptions = new Domain.Models.NotificationModels.DeliveryOption()
                        {
                            Username = "123456",
                            Password = "987654",
                            Type = DeliveryOptionsType.Basic
                        }
                    }
                }
            };

            var buId = Guid.NewGuid();

            var result = await _providerUnderTest.CreateNotificationAsync(buId, input);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public async Task CreateNotificationAsync_ShouldValidateModelANdReturnErrorWhenInputIsNotValid()
        {
            _serviceUnitOfWorkMock.Setup(x => x.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(It.IsAny<CreateBusinessUnitNotificationConfigurationContract>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(new TeleenaInnerExp()));

            _businessUnitTranslatorsMock.Setup(x => x.CreateNotificationTranslator.Translate(It.IsAny<CreateNotificationModel>()))
                .Returns(new CreateBusinessUnitNotificationConfigurationContract()
                {
                    ProcessId = "bla",
                    Deliveries = new TeleenaServiceReferences.NotificationConfigurationService.Delivery[]
                    {
                        new TeleenaServiceReferences.NotificationConfigurationService.Delivery()
                        {
                            DeliveryMethod = DeliveryMethodEnum.HTTP,
                            DeliveryValue = "truc",
                            DeliveryOptions = new TeleenaServiceReferences.NotificationConfigurationService.DeliveryOption()
                            {
                                Type = DeliveryOptionsEnumDeliveryOptionsType.Basis,
                                Username = "456123",
                                Password = "5647321"
                            }
                        }
                    }
                });
            _businessUnitValidatorsMock.Setup(x => x.CreateBusinessUnitNotificationValidator.ValidateModel(It.IsAny<CreateNotificationModel>()))
                .Returns(ProviderOperationResult<CreateNotificationModelResponse>.InvalidInput("bla", "truc"));

            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.LOWBALANCE_SMS,
                Deliveries = new List<Domain.Models.NotificationModels.Delivery>()
                {
                    new Domain.Models.NotificationModels.Delivery()
                    {
                        DeliveryMethod = DeliveryMethod.HTTP,
                        DeliveryValue = "test.me",
                        DeliveryOptions = new Domain.Models.NotificationModels.DeliveryOption()
                        {
                            Username = "123456",
                            Password = "987654",
                            Type = DeliveryOptionsType.Basic
                        }
                    }
                }
            };

            var buId = Guid.NewGuid();

            var result = await _providerUnderTest.CreateNotificationAsync(buId, input);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }
    }
}