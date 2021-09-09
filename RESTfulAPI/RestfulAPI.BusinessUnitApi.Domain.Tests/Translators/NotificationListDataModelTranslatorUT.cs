using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class NotificationListDataModelTranslatorUT
    {
        NotificationListDataModelTranslator _notificationListDataModelTranslator;

        GetNotificationConfigurationListResponse _response;
        GetNotificationConfigurationListResponse _responseEmpty;
        GetNotificationConfigurationListResponse _responseNull;

        Mock<INotificationTypeTranslator> _notificationTypeTranslatorMock;

        string notificationType1 = "EMPTYBALANCE_DATALIMIT";
        //NotificationType notificationType1 = NotificationType.EMPTYBALANCE_DATALIMIT;
        string notificationType2 = "adddd";
        string mvnoId = "mvnoId-123";

        [TestInitialize]
        public void Setup()
        {
            _notificationTypeTranslatorMock = new Mock<INotificationTypeTranslator>();
            _notificationTypeTranslatorMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(NotificationType.EMPTYBALANCE_DATALIMIT);

            _notificationListDataModelTranslator = new NotificationListDataModelTranslator(_notificationTypeTranslatorMock.Object);

            _responseNull = new GetNotificationConfigurationListResponse();

            _responseEmpty = new GetNotificationConfigurationListResponse { NotificationsConfiguration = new GetNotificationConfigurationResponse[] { } };

            _response = new GetNotificationConfigurationListResponse
            {
                NotificationsConfiguration = new GetNotificationConfigurationResponse[]
                {
                    new GetNotificationConfigurationResponse
                    {
                        Id = Guid.NewGuid(),
                        MvnoId = mvnoId,
                        Type = notificationType1,
                        Deliveries = new Delivery[] { new Delivery { DeliveryMethod = DeliveryMethodEnum.Email, DeliveryValue = "tyuuu"} }
                    },
                    new GetNotificationConfigurationResponse
                    {
                        Id = Guid.NewGuid(),
                        MvnoId = mvnoId,
                        Type = notificationType2,
                        Deliveries = new Delivery[] { new Delivery { DeliveryMethod = DeliveryMethodEnum.SMS, DeliveryValue = "123"} }
                    }
                }
            };
        }

        [TestMethod]
        public void Translate_ShouldTranslate_EmptyNotificationInput_To_EmptyErrayNotifications()
        {
            var result = _notificationListDataModelTranslator.Translate(_responseEmpty);

            Assert.AreEqual(0, result.Notifications.Count);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_NullNotificationInput_To_EmptyErrayNotifications()
        {
            var result = _notificationListDataModelTranslator.Translate(_responseNull);

            Assert.AreEqual(0, result.Notifications.Count);
        }
    }
}
