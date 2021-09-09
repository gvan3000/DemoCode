using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class PropositionInfoModelTranslatorUT
    {
        private PropositionInfoModelTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new PropositionInfoModelTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfInputIsNull()
        {
            PropositionsContract input = null;

            var translatedResult = translatorUnderTest.Translate(input);

            Assert.IsNull(translatedResult);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfInputIsEmpty()
        {
            PropositionsContract input = new PropositionsContract();

            var translatedResult = translatorUnderTest.Translate(input);

            Assert.IsNull(translatedResult);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfPropositionsContractIsNull()
        {
            PropositionsContract input = new PropositionsContract
            {
                PropositionContracts = null
            };

            var translatedResult = translatorUnderTest.Translate(input);

            Assert.IsNull(translatedResult);
        }

        [TestMethod]
        public void Translate_Should_Mapp_Code_ServiceTypeCode_and_SubscriptionTypeCode()
        {
            string serviceLevelTypeCode1 = "DATA";
            string serviceLevelTypeCode2 = "SMS";
            string serviceLevelTypeCode3 = "VOCIE";
            string SubscType1 = "SHB";
            string SubscType2 = "PPU";
            string SubscType3 = "EUS";
            string code1 = "SHB06";
            string code2 = "PPU01";
            string code3 = "EUS06";

            PropositionsContract input = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract { ServiceLevelTypeCode = serviceLevelTypeCode1, SubscriptionTypeCode = SubscType1, Code = code1, IsSharedWallet = true, Name = "name03", PricePlanCode = "pp" },
                                new CommercialOfferConfigurationContract { ServiceLevelTypeCode = serviceLevelTypeCode2, SubscriptionTypeCode = SubscType2, Code = code2, IsSharedWallet = false, Name = "name-02" },
                                new CommercialOfferConfigurationContract { ServiceLevelTypeCode = serviceLevelTypeCode3, SubscriptionTypeCode = SubscType3, Code = code3, Name = "name_1" },
                            }
                        }
                    }
                }
            };

            var translatedResult = translatorUnderTest.Translate(input);

            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.ServiceTypeCode == serviceLevelTypeCode1)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.ServiceTypeCode == serviceLevelTypeCode2)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.ServiceTypeCode == serviceLevelTypeCode3)).FirstOrDefault());

            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.CommercialOfferDefinitionCode == code1)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.CommercialOfferDefinitionCode == code2)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.CommercialOfferDefinitionCode == code3)).FirstOrDefault());

            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.SubscriptionTypeCode == SubscType1)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.SubscriptionTypeCode == SubscType2)).FirstOrDefault());
            Assert.IsTrue(translatedResult.Select(x => x.CommercialOfferDefinitions.Any(c => c.SubscriptionTypeCode == SubscType3)).FirstOrDefault());
        }
    }
}
