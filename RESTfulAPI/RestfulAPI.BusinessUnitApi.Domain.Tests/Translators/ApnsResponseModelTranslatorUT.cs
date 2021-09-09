using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ApnsResponseModelTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            var input = default(ApnDetailContract[]);

            var translatorUnderTest = new ApnsResponseModelTranslator();

            var result = translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void Translate_ShouldReturnEmptyModel_WhenContractIsEmpty()
        {
            var input = new ApnDetailContract[0];

            var translatorUnderTest = new ApnsResponseModelTranslator();

            var result = translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.Apns.Any() && result.DefaultApn == null);
        }
        
        [TestMethod]
        public void Translate_ShouldReturnValidModel_WithDefaultApn()
        {
            var nameDefault = "apnDefault";
            var nameNonDefault = "apnNonDefault";
            var idOfDefault = Guid.NewGuid();
            var input = new ApnDetailContract[]
            {
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = nameNonDefault,
                    IsDefault = false
                 },
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = idOfDefault,
                    Name = nameDefault,
                    IsDefault = true
                }
            };

            var translatorUnderTest = new ApnsResponseModelTranslator();

            var result = translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Apns);
            Assert.AreEqual(2, result.Apns.Count);
            Assert.AreEqual(nameNonDefault, result.Apns[0].Name);
            Assert.AreEqual(input[0].ApnSetDetailId, result.Apns[0].Id);
            Assert.AreEqual(nameDefault, result.Apns[1].Name);
            Assert.AreEqual(input[1].ApnSetDetailId, result.Apns[1].Id);
            Assert.AreEqual(idOfDefault, result.DefaultApn);
        }
    }
}
