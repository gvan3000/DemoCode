using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class AllowedPropositionContractTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldTranslateEndUserSubscription()
        {
            var translator = new AllowedPropositionContractTranslator();
            var allowedProposition = new AllowedPropositionContract() { EndUserSubscription = true };

            Proposition propositionModel = translator.Translate(allowedProposition);

            Assert.AreEqual(propositionModel.EndUserSubscription, allowedProposition.EndUserSubscription);
        }

        [TestMethod]
        public void Translate_ShouldTranslatePropositionId()
        {
            var translator = new AllowedPropositionContractTranslator();
            var allowedProposition = new AllowedPropositionContract() { PropositionId = new Guid() };

            Proposition propositionModel = translator.Translate(allowedProposition);

            Assert.AreEqual(allowedProposition.PropositionId, propositionModel.Id);
        }
    }
}
