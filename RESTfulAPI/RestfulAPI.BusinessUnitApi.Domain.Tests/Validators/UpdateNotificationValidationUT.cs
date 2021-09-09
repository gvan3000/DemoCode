using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.Enums;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Validators.Notifications;
using System;
using System.Collections.Generic;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class UpdateNotificationValidationUT
    {
        UpdateNotificationValidation _validatorUnderTest;

        UpdateNotificationModel _modelRequiredFieldsPassword;
        UpdateNotificationModel _modelRequiredFieldsUsername;
        UpdateNotificationModel _modelMoreThanOneDeliveryMethodPerEventType;

        UpdateNotificationModel _modelInvalidHttpDeliveryValue;
        UpdateNotificationModel _modelEmailDeliveryValue;
        UpdateNotificationModel _modelSmsDeliveryValue;
        UpdateNotificationModel _modelSmsDeliveryInvalid;

        UpdateNotificationModel _modelHttpUserNameEmpty;
        UpdateNotificationModel _modelHttpUserPasswordEmpty;

        [TestInitialize]
        public void SetUp()
        {
            _validatorUnderTest = new UpdateNotificationValidation();

            _modelHttpUserPasswordEmpty = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "http://bal.truc/me",
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "adad ",
                            Password = "   "
                        }
                    }
                }
            };

            _modelHttpUserNameEmpty = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "http://bal.truc/me",
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = " ",
                            Password = "12321312321321"
                        }
                    }
                }
            };

            _modelSmsDeliveryInvalid = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.SMS,
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "123123",
                            Password = "12321312321321"
                        }
                    }
                }
            };

            _modelMoreThanOneDeliveryMethodPerEventType = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "123123",
                            Password = "12321312321321"
                        }
                    },
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "dasdasda"
                    },
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "asd@afaa"
                    }
                }
            };

            _modelInvalidHttpDeliveryValue = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryValue = "ht:/invalidUrlSuperCool.",
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "123123",
                            Password = "123123123112"
                        }
                    }
                }
            };

            _modelSmsDeliveryValue = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.SMS,
                        DeliveryValue = "Ola ola ... la la "
                    }
                }
            };

            _modelEmailDeliveryValue = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.Email,
                        DeliveryValue = "hello hi there..."
                    }
                }
            };

            _modelRequiredFieldsPassword = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Username = "123123"
                        }
                    }
                }
            };

            _modelRequiredFieldsUsername = new UpdateNotificationModel
            {
                BusinessUnitId = Guid.NewGuid(),
                Deliveries = new List<Delivery>
                {
                    new Delivery
                    {
                        DeliveryMethod = NotificationsEnums.DeliveryMethod.HTTP,
                        DeliveryOptions = new DeliveryOption
                        {
                            Type = NotificationsEnums.DeliveryOptionsType.Basic,
                            Password = "1231312"
                        }
                    }
                }
            };

        }

        [TestMethod]
        public void Validate_HttpDeliveryMethod_ShouldReturnBadRequest_WhenTypeIsBasic_And_PasswordIsMissing()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelRequiredFieldsPassword);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Password is required when delivery option type is BASIC", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_HttpDeliveryMethod_ShouldReturnBadRequest_WhenTypeIsBasic_And_UsernameIsMissing()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelRequiredFieldsUsername);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Username is required when delivery option type is BASIC", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturnBadRequest_When_MoreThanOneDeliveryMethodPerEventType()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelMoreThanOneDeliveryMethodPerEventType);

            Assert.AreEqual(validationResult.HttpResponseCode, validationResult.HttpResponseCode);
            Assert.AreEqual("Only one type of delivery per event type is allowed (one sms, one email and / or one http)", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_HttpDeliveryMethod_ShouldReturnBadRequest_When_DeliveryValue_IsInvalid()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelInvalidHttpDeliveryValue);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Url is not in the valid form", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_EmaiLDeliveryMethod_ShouldReturn_OkResult_When_DeliveryValueIsRandomString()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelEmailDeliveryValue);

            Assert.AreEqual(HttpStatusCode.OK, validationResult.HttpResponseCode);
        }

        [TestMethod]
        public void Validate_SmsDeliverymethod_ShouldReturn_OkResult_When_DeliveryValueIsRandomString()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelSmsDeliveryValue);

            Assert.AreEqual(HttpStatusCode.OK, validationResult.HttpResponseCode);
        }

        [TestMethod]
        public void Validate_ShouldReturnBadRequest_When_SMSDeliveryMethod_And_DeliveryOptionsIsSet()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelSmsDeliveryInvalid);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Unsupported, can only be specified for HTTP delivery method", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturnBadRequest_When_UsernameIsEmptySpace()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelHttpUserNameEmpty);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Username is required when delivery option type is BASIC", validationResult.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturnBadRequest_When_UsernameIsNull()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelHttpUserNameEmpty);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Username is required when delivery option type is BASIC", validationResult.ErrorMessage);
        }
       
        [TestMethod]
        public void Validate_ShouldReturnBadRequest_When_PasswordIsEmpty()
        {
            var validationResult = _validatorUnderTest.ValidateModel(_modelHttpUserPasswordEmpty);

            Assert.AreEqual(HttpStatusCode.BadRequest, validationResult.HttpResponseCode);
            Assert.AreEqual("Password is required when delivery option type is BASIC", validationResult.ErrorMessage);
        }
    }
}
