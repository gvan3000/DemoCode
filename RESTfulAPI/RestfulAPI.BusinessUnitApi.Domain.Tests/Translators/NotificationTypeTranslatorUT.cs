using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.TeleenaServiceReferences.Translators;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class NotificationTypeTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldConvertNotificationType()
        {
            var translatorUnderTest = new NotificationTypeTranslator();

            var input = "NONCHARGEABLENOTIFICATION_UKPORTOUTREQUEST";

            var result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationType.UK_PORTOUTREQUEST, result);

            input = "NONCHARGEABLENOTIFICATION_UKPORTINFOREQUEST";
            result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationType.UK_PORTINFOREQUEST, result);
        }

        [TestMethod]
        public void Translate_WhenInputStringIsNullOrEmpty()
        {
            var translatorUnderTest = new NotificationTypeTranslator();

            var input = string.Empty;

            var result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationType.NOTDEFINED, result);

            input = null;
            result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationType.NOTDEFINED, result);
        }

        [TestMethod]
        public void Translate_ShouldConvertFromNotificationTypeToNotificationTypeConstants()
        {
            var translatorUnderTest = new NotificationTypeTranslator();

            var input = NotificationType.UK_PORTOUTREQUEST;

            var result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationTypeConstants.NONCHARGEABLENOTIFICATION_UKPORTOUTREQUEST, result);

            input = NotificationType.UK_PORTINFOREQUEST;
            result = translatorUnderTest.Translate(input);
            Assert.AreEqual(NotificationTypeConstants.NONCHARGEABLENOTIFICATION_UKPORTINFOREQUEST, result);
        }
    }
}
