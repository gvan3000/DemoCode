using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class UpdateApnsTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldReturnNullWhenInputIsNull()
        {
            var availableApns = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "bla",
                            IsDefault = false
                        }
                    }
                }
            };

            var input = default(UpdateBusinessUnitApnsModel);

            var translatorUnderTest = new UpdateApnsTranslator();

            var result = translatorUnderTest.Translate(input, availableApns);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyContractWhenApnsAreEmpty()
        {
            var availableApns = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "bla",
                            IsDefault = false
                        }
                    }
                }
            };

            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>(),
                DefaultApn = null
            };

            var translatorUnderTest = new UpdateApnsTranslator();

            var result = translatorUnderTest.Translate(input, availableApns);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ApnDetails == null || result.ApnDetails.Length == 0);

            input = new UpdateBusinessUnitApnsModel()
            {
                Apns = null,
                DefaultApn = null
            };

            result = translatorUnderTest.Translate(input, availableApns);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ApnDetails == null || result.ApnDetails.Length == 0);
        }

        [TestMethod]
        public void Translate_ShouldReturnFilledContractWithAppropriateDefaultSet()
        {
            var defaultApn = Guid.NewGuid();

            var availableApns = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = defaultApn,
                            Name = "bla",
                            IsDefault = false
                        },
                        new ApnDetailContract()
                        {
                            Id = Guid.NewGuid(),
                            ApnSetDetailId = Guid.NewGuid(),
                            Name = "truc",
                            IsDefault = false
                        }
                    }
                }
            };

            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = availableApns[0].ApnSetDetails[1].ApnSetDetailId } },
                DefaultApn = defaultApn
            };

            var translatorUnderTest = new UpdateApnsTranslator();

            var result = translatorUnderTest.Translate(input, availableApns);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ApnDetails);
            Assert.AreEqual(2, result.ApnDetails.Length);
            Assert.IsNotNull(result.ApnDetails.FirstOrDefault(x => x.ApnSetDetailId == defaultApn));
            Assert.IsTrue(result.ApnDetails.FirstOrDefault(x => x.ApnSetDetailId == defaultApn).IsDefault);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShouldThrowWhenInputHasApnsButAvailableApnsAreEmpty()
        {
            var availableApns = default(ApnSetWithDetailsContract[]);

            var defaultApn = Guid.NewGuid();

            var input = new UpdateBusinessUnitApnsModel()
            {
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = Guid.NewGuid() } },
                DefaultApn = defaultApn
            };

            var translatorUnderTest = new UpdateApnsTranslator();

            var result = translatorUnderTest.Translate(input, availableApns);
        }
    }
}
