using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class UpdateDefaultApnTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            var apnsOfBusinessUnit = new ApnDetailContract[]
            {
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = "validAPNName",
                    IsDefault = true,
                    ApnSetId = Guid.NewGuid()
                }
            };

            var input = default(UpdateBusinessUnitDefaultApnModel);

            var translatorUnderTest = new UpdateDefaultApnTranslator();

            var result = translatorUnderTest.Translate(input, apnsOfBusinessUnit);

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShouldThrowWhenApnsAreNull()
        {
            var apnsOfBusinessUnit = default(ApnDetailContract[]);

            var input = new UpdateBusinessUnitDefaultApnModel() { Id = Guid.NewGuid() };

            var translatorUnderTest = new UpdateDefaultApnTranslator();

            var result = translatorUnderTest.Translate(input, apnsOfBusinessUnit);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShouldThrowWhenApnsAreEmpty()
        {
            var apnsOfBusinessUnit = new ApnDetailContract[0];

            var input = new UpdateBusinessUnitDefaultApnModel() { Id = Guid.NewGuid() };

            var translatorUnderTest = new UpdateDefaultApnTranslator();

            var result = translatorUnderTest.Translate(input, apnsOfBusinessUnit);
        }

        [TestMethod]
        public void Translate_ShouldReturnFilledContractWithProperIsDefaultSet()
        {
            string initialDefaultApnaName = "initalDefault";
            string initialNonDefaultApnaName = "initialNonDefault";
            Guid apnSetId1 = Guid.NewGuid();
            Guid apnSetId2 = Guid.NewGuid();

            var apnsOfBusinessUnit = new ApnDetailContract[]
            {
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = initialDefaultApnaName,
                    IsDefault = true,
                    ApnSetId = apnSetId1
                },
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = initialNonDefaultApnaName,
                    IsDefault = false,
                    ApnSetId = apnSetId2
                }
            };

            var input = new UpdateBusinessUnitDefaultApnModel() { Id = apnsOfBusinessUnit[1].ApnSetDetailId };

            var translatorUnderTest = new UpdateDefaultApnTranslator();

            var result = translatorUnderTest.Translate(input, apnsOfBusinessUnit);

            Assert.IsNotNull(result.ApnDetails);
            Assert.AreEqual(2, result.ApnDetails.Count());
            Assert.AreEqual(initialNonDefaultApnaName, result.ApnDetails[1].Name);
            Assert.AreEqual(true, result.ApnDetails[1].IsDefault);
            Assert.AreEqual(false, result.ApnDetails[0].IsDefault);
            Assert.AreEqual(apnSetId1, result.ApnDetails[0].ApnSetId);
        }
    }
}
