using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BusinessUnitPatchModelPropositionsTranslatorUT
    {
        [TestMethod]
        public void Translate_NumberOfInputPropositionsAndResultPropositionsAreEqual()
        {
            BusinessUnitPatchModel model = new BusinessUnitPatchModel();
            model.Propositions = new Proposition[] 
            {
                new Proposition { Id = Guid.NewGuid(), EndUserSubscription = true },
                new Proposition { Id = Guid.NewGuid(), EndUserSubscription = false }
            };

            var translatorUnderTest = new BusinessUnitPatchModelPropositionsTranslator();

            var result = translatorUnderTest.Translate(model);

            Assert.AreEqual(model.Propositions.Count(), result.Propositions.Count);
        }
    }
}
