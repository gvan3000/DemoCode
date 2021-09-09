using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.TeleenaServiceReferences.Translators;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class CreateNotificationTranslatorUT
    {
        private Mock<INotificationTypeTranslator> _mockTypeTranslator;

        private CreateNotificationTranslator _translatorUnderTest;

        [TestInitialize]
        public void SetupEachTest()
        {
            _mockTypeTranslator = new Mock<INotificationTypeTranslator>(MockBehavior.Strict);
            _mockTypeTranslator.Setup(x => x.Translate(It.IsAny<NotificationType>()))
                .Returns("bla_success");

            _translatorUnderTest = new CreateNotificationTranslator(_mockTypeTranslator.Object);
        }

        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            var input = default(CreateNotificationModel);

            var result = _translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldTranslateProcessIdUsingTypeTranslator()
        {
            var input = new CreateNotificationModel()
            {
                Type = NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
            };

            var result = _translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ProcessId);
            _mockTypeTranslator.Verify(x => x.Translate(It.Is<NotificationType>(t => t == input.Type)), Times.Once);
        }

        [TestMethod]
        public void Translate_ShouldHandleNullDeliveries()
        {
            var input = new CreateNotificationModel()
            {
                Type = NotificationType.EMPTYBALANCE,
                Deliveries = null
            };

            var result = _translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsNull(result.Deliveries);
        }

        [TestMethod]
        public void Translate_ShouldHandleNullForDeliveryOptions()
        {
            var input = new CreateNotificationModel()
            {
                Type = NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "bla@truc.moc",
                        DeliveryOptions = null
                    }
                }
            };

            var result = _translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Deliveries);
            Assert.AreEqual(1, result.Deliveries.Length);
            Assert.IsNull(result.Deliveries[0].DeliveryOptions);
        }
    }
}
