using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class CreateNotificationValidatorUT
    {
        [TestMethod]
        public void ValidateModel_ShouldAllowOnlyOneDeliveryMethodTypePerEvent()
        {
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "test@teleena.com"
                    },
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "test2@teleena.com"
                    }
                }
            };

            var validatorUnderTest = new CreateNotificationValidator();

            var validationResult = validatorUnderTest.ValidateModel(input);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "Only one type of delivery");
        }

        [TestMethod]
        public void ValidateModel_ShouldRequireUsernameWhenDeliveryOptionTypeIsBasic()
        {
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "http://test.com/bla",
                        DeliveryOptions = new DeliveryOption()
                        {
                            Type = Domain.Models.Enums.NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = null,
                            Password = "bla"
                        }
                    },
                }
            };

            var validatorUnderTest = new CreateNotificationValidator();

            var validationResult = validatorUnderTest.ValidateModel(input);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "required");
        }

        [TestMethod]
        public void ValidateModel_ShouldRequirePasswordWhenDeliveryOptionTypeIsBasic()
        {
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "http://test.com/bla",
                        DeliveryOptions = new DeliveryOption()
                        {
                            Type = Domain.Models.Enums.NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "bla",
                            Password = ""
                        }
                    },
                }
            };

            var validatorUnderTest = new CreateNotificationValidator();

            var validationResult = validatorUnderTest.ValidateModel(input);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "required");
        }

        [TestMethod]
        public void ValidateModel_ShouldAllowDeliveryOptionsOnlyForHttp()
        {
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "test@teleena.com",
                        DeliveryOptions = new DeliveryOption()
                        {
                            Type = Domain.Models.Enums.NotificationsEnums.DeliveryOptionsType.Basic,
                            Password = "bla",
                            Username = "truc"
                        }
                    },
                }
            };

            var validatorUnderTest = new CreateNotificationValidator();

            var validationResult = validatorUnderTest.ValidateModel(input);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "can only be specified for HTTP");
        }

        [TestMethod]
        public void ValidateModel_ShouldNotAcceptInvalidUrl()
        {
            var input = new CreateNotificationModel()
            {
                Type = TeleenaServiceReferences.Constants.NotificationType.EMPTYBALANCE,
                Deliveries = new List<Delivery>()
                {
                    new Delivery()
                    {
                        DeliveryMethod = Domain.Models.Enums.NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "test"
                    },
                }
            };

            var validatorUnderTest = new CreateNotificationValidator();

            var validationResult = validatorUnderTest.ValidateModel(input);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "Url is not in the valid form");
        }
    }
}
