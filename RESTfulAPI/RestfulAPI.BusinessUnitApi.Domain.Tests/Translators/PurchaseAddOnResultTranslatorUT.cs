using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn;
using RestfulAPI.TeleenaServiceReferences.AddOnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class PurchaseAddOnResultTranslatorUT
    {
        [TestMethod]
        public void Translate_IfInputIsNull_ShoudReturn_FailFlagSetToTrue()
        {
            PurchaseAddOnResultContract contract = null;
            var translatorUnderTest = new PurchaseAddOnResultTranslator();

            var translatedValue = translatorUnderTest.Translate(contract);

            Assert.IsNotNull(translatedValue);
            Assert.IsTrue(translatedValue.Fail);
        }

        [TestMethod]
        public void Translate_IfInputSuccessFlag_IsSetToTrue_FailIsSetToFalse()
        {
            PurchaseAddOnResultContract contract = new PurchaseAddOnResultContract { Success = true };

            var translatorUnderTest = new PurchaseAddOnResultTranslator();

            var translatedValue = translatorUnderTest.Translate(contract);

            Assert.IsFalse(translatedValue.Fail);
        }

        [TestMethod]
        public void Translate_IfInputSuccessIsFalseAndMessageIsSet_TranslatedValueContainsMessage()
        {
            PurchaseAddOnResultContract contract = new PurchaseAddOnResultContract { Success = false, Message = "error message 1" };

            var translatorUnderTest = new PurchaseAddOnResultTranslator();

            var translatedValue = translatorUnderTest.Translate(contract);

            Assert.IsTrue(translatedValue.Fail);
            Assert.AreEqual(contract.Message, translatedValue.Message);
        }
    }
}
