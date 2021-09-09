using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BalanceProfile;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BalanceProfileTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            var input = default(SysCodeContract);

            var translatorUnderTest = new BalanceProfileTranslator();

            var result = translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldReturnFillIdAndCode()
        {
            var input = new SysCodeContract()
            {
                Code = "bla",
                Id = Guid.NewGuid()
            };

            var transalatorUnderTest = new BalanceProfileTranslator();

            var result = transalatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(input.Code, result.Code);
            Assert.AreEqual(input.Id, result.Id);
        }
    }
}
