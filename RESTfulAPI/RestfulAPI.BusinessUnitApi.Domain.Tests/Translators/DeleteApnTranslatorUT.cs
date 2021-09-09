using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class DeleteApnTranslatorUT
    {
        DeleteApnTranslator translatorUnderTest;

        Guid businessUnitId;

        Guid apnSetDetailId;

        ApnDetailContract[] contract;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new DeleteApnTranslator();

            businessUnitId = Guid.NewGuid();

            apnSetDetailId = Guid.NewGuid();

            contract = new ApnDetailContract[]
            {
                new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = true, Name = "rcomnet.mnc015.mcc405.gprs", ApnSetDetailId = Guid.NewGuid() },
                new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = false, Name = "bla", ApnSetDetailId = apnSetDetailId },
                new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = false, Name = "threetest.co.rs", ApnSetDetailId = Guid.NewGuid() },
                new ApnDetailContract { Id = Guid.NewGuid(), IsDefault = false, Name = "test.test.com", ApnSetDetailId = Guid.NewGuid() }
            };
        }

        [TestMethod]
        public void Translate_ShouldReturn_SetApnDetailsForAccountRequestContract()
        {           
            var result = translatorUnderTest.Translate(contract, apnSetDetailId, businessUnitId);

            Assert.IsInstanceOfType(result, typeof(SetApnDetailsForAccountRequestContract));
        }

        [TestMethod]
        public void Translate_Should_RemoveOneApnFromTheList()
        {
            var result = translatorUnderTest.Translate(contract, apnSetDetailId, businessUnitId);

            var removedExists = result.ApnDetails.Any(x => x.ApnSetDetailId == apnSetDetailId);

            Assert.AreEqual(contract.Count(), result.ApnDetails.Count() + 1);
            Assert.IsTrue(!removedExists);            
        }

        [TestMethod]
        public void Translate_Should_Mapp_BusinessUnitId_ToResult_AccountId()
        {
            var result = translatorUnderTest.Translate(contract, apnSetDetailId, businessUnitId);

            Assert.AreEqual(businessUnitId, result.AccountId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShoulThrow_ArgumentNullException_IfInputApnsisNull()
        {
            var result = translatorUnderTest.Translate(null, apnSetDetailId, businessUnitId);
        }      
    }
}
