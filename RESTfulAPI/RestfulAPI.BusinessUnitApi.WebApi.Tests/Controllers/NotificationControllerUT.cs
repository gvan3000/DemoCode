using System;
using System.Collections.Generic;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Controllers;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    [TestClass]
    public class NotificationControllerUT
    {
        NotificationController _controllerUnderTest;

        Mock<INotificationConfigurationProvider> _notificationProviderMock;

        [TestInitialize]
        public void Setup()
        {
            _notificationProviderMock = new Mock<INotificationConfigurationProvider>();

            _notificationProviderMock.Setup(x => x.GetBusinessUnitNotificationsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProviderOperationResult<Domain.Models.NotificationModels.GetNotificationListDataModel>());

            _notificationProviderMock.Setup(x => x.DeleteBusinessUnitNotificationAsync(It.IsAny<Guid>(), It.IsAny<TeleenaServiceReferences.Constants.NotificationType>()))
                .ReturnsAsync(new ProviderOperationResult<object>());
            _notificationProviderMock.Setup(x => x.UpdateBusinessUnitNotificationAsync(It.IsAny<UpdateNotificationModel>()))
                .ReturnsAsync(new Common.ProviderOperationResult<object>());
            _notificationProviderMock.Setup(x => x.CreateNotificationAsync(It.IsAny<Guid>(), It.IsAny<CreateNotificationModel>()))
                .ReturnsAsync(Common.ProviderOperationResult<CreateNotificationModelResponse>.OkResult(new CreateNotificationModelResponse()));

            _controllerUnderTest = new NotificationController(_notificationProviderMock.Object);
        }

        [TestMethod]
        public void Get_ShouldCall_NotificationConfigurationProvider_GetBusinessUnitNotificationsAsync()
        {
            var response = _controllerUnderTest.GetBusinesUnitConfigurations(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _notificationProviderMock.Verify(x => x.GetBusinessUnitNotificationsAsync(It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod]
        public void Get_ShouldReturn_InvalidModelStateResult_When_ModelStateIsInvalid()
        {
            _controllerUnderTest.ModelState.AddModelError("some key", "some error");

            var response = _controllerUnderTest.GetBusinesUnitConfigurations(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void UpdateNotification_ShouldReturn_InvalidModelStateResult_When_ModelStateIsInvalid()
        {
            _controllerUnderTest.ModelState.AddModelError("some key", "some error");

            var response = _controllerUnderTest.UpdateNotification(
                                Guid.NewGuid(),
                                "type notificaiton",
                                new UpdateNotificationModel { Deliveries = new List<Domain.Models.NotificationModels.Delivery> { new Domain.Models.NotificationModels.Delivery { DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.Email, DeliveryValue = "test@test.com" } } })
                                .ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Delete_ShouldCall_NotificationConfigurationProvider_DeleteBusinessUnit()
        {
            var response = _controllerUnderTest.DeleteBusinessUnitConfigurations(Guid.NewGuid(), TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE).ConfigureAwait(false).GetAwaiter().GetResult();

            _notificationProviderMock.Verify(x => x.DeleteBusinessUnitNotificationAsync(It.IsAny<Guid>(), It.IsAny<TeleenaServiceReferences.Constants.NotificationType>()), Times.Once());
        }

        [TestMethod]
        public void Delete_ShouldReturn_InvalidModelStateResult_When_ModelStateIsInvalid()
        {
            _controllerUnderTest.ModelState.AddModelError("key1", "random validation error");

            var response = _controllerUnderTest.DeleteBusinessUnitConfigurations(Guid.NewGuid(), TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void CreateNotification_ShouldReturnBadRequestIfInputIsNotValid()
        {
            _controllerUnderTest.ModelState.AddModelError("some key", "some error");

            var response = _controllerUnderTest.CreateNotification(Guid.NewGuid(), null)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void CreateNotification_ShouldCallProviderAndReturnResult()
        {
            var businessUnitId = Guid.NewGuid();
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
            };
            var response = _controllerUnderTest.CreateNotification(businessUnitId, input)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(NegotiatedContentResult<CreateNotificationModelResponse>));

            _notificationProviderMock.Verify(x => x.CreateNotificationAsync(It.Is<Guid>(buId => buId == businessUnitId), It.Is<CreateNotificationModel>(contract => contract == input)), Times.Once);
        }
    }
}
