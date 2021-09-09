using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class APNTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            ApnSetWithDetailsContract[] input = null;
            var translatorUnderTest = new BusinessUnitAPNsTranslator();
            var result = translatorUnderTest.Translate(input);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyContractWhenApnsAreEmpty()
        {
            var input = new ApnSetWithDetailsContract[0];
            
            var translatorUnderTest = new BusinessUnitAPNsTranslator();
            var result = translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.APNSets == null || result.APNSets.Count == 0);
        }

        [TestMethod]
        public void Translate_ShouldReturnFilledContratWithAppropriateDefaultSet()
        {
            var input = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSet = new ApnSetContract(){ Name = "JetSet"},
                    CompanyId = new Guid(),
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "Homer Simpson",
                            IsDefault = false
                        },
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "Eric Cartman",
                            IsDefault = false
                        },
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "Barney Gumble",
                            IsDefault = false
                        },
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "Deric Troter",
                            IsDefault = false
                        }
                    }
                }
            };

            var translatorUnderTest = new BusinessUnitAPNsTranslator();
            var result = translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.APNSets != null || result.APNSets.Count != 0);
            Assert.IsTrue(result.APNSets.FirstOrDefault().Name.Equals("JetSet"));
            Assert.AreEqual(4, result.APNSets[0].APNs.Count);
            Assert.AreEqual(input[0].ApnSetDetails[0].ApnSetDetailId, result.APNSets[0].APNs[0].Id);
            Assert.AreEqual(input[0].ApnSetDetails[0].Name, result.APNSets[0].APNs[0].Name);
            Assert.AreEqual(input[0].ApnSetDetails[1].ApnSetDetailId, result.APNSets[0].APNs[1].Id);
            Assert.AreEqual(input[0].ApnSetDetails[1].Name, result.APNSets[0].APNs[1].Name);
            Assert.AreEqual(input[0].ApnSetDetails[2].ApnSetDetailId, result.APNSets[0].APNs[2].Id);
            Assert.AreEqual(input[0].ApnSetDetails[2].Name, result.APNSets[0].APNs[2].Name);
            Assert.AreEqual(input[0].ApnSetDetails[3].ApnSetDetailId, result.APNSets[0].APNs[3].Id);
            Assert.AreEqual(input[0].ApnSetDetails[3].Name, result.APNSets[0].APNs[3].Name);
        }
    }
}
