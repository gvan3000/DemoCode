using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using System.Linq;
using static RestfulAPI.BusinessUnitApi.Domain.Models.Enums.BusinessUnitsEnums;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class PropositionsContractTranslatorUT
    {
        [TestMethod]
        public void Translate_Should_Return_Null_If_Input_Is_Null()
        {
            var translator = new PropositionsContractTranslator();
            var output = translator.Translate(null, null);

            Assert.IsNull(output);
        }

        [TestMethod]
        public void Translate_Should_Return_Null_If_PropositionContracts_Have_No_Eleents()
        {
            var translator = new PropositionsContractTranslator();
            PropositionsContract input = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
            };

            var output = translator.Translate(input, null);

            Assert.IsNull(output);
        }

        [TestMethod]
        public void Translate_Should_Return_PropositionsResponseModel_With_CoConfigurations_Is_Null_If_CommercialOfferConfigurationsContract_Is_Null()
        {
            var translator = new PropositionsContractTranslator();
            PropositionsContract input = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
            };
            PropositionContract item = new PropositionContract()
            {
                CommercialOfferConfigurationsContract = null
            };
            input.PropositionContracts.Add(item);

            Assert.IsNull(input.PropositionContracts.FirstOrDefault().CommercialOfferConfigurationsContract);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Translate_Should_Throw_Exception()
        {
            var translator = new PropositionsContractTranslator();
            CommercialOfferConfigurationContract commercialOfferConfigurationContract = new CommercialOfferConfigurationContract()
            {
                ServiceLevelTypeCode = "wertyu",
                BlackListedZones = new CommercialOfferZonesContract(),
                Id = Guid.NewGuid(),
                BundleAmount = 121,
                IsSharedWallet = true,
                WhiteListedZones = new CommercialOfferZonesContract()
            };
            PropositionsContract input = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
            };
            List<CommercialOfferConfigurationContract> list = new List<CommercialOfferConfigurationContract>
            {
                commercialOfferConfigurationContract
            };
            PropositionContract item = new PropositionContract()
            {
                CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract()
                {
                    CommercialOfferConfigurationContracts = list
                },
                Id = Guid.NewGuid()
            };
            input.PropositionContracts.Add(item);
            var output = translator.Translate(input, null);
        }

        [TestMethod]
        public void Translate_ShouldMatchPropositionInSupliedPropositionsForProductCreation()
        {
            var translator = new PropositionsContractTranslator();
            var avaliablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract() { Id = Guid.NewGuid() },
                    new PropositionContract() { Id = Guid.NewGuid() },
                    new PropositionContract() { Id = Guid.NewGuid() }
                }
            };
            var productCreationPropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract() { Id = Guid.NewGuid() },
                    new PropositionContract() { Id = avaliablePropositions.PropositionContracts[1].Id }
                }
            };

            var result = translator.Translate(avaliablePropositions, productCreationPropositions);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Propositions.Where(x => x.IsAvailableForProductCreation).Count());
        }

        [TestMethod]
        public void Translate_ShouldNotFillProductCreationPropositionIndicationIfNoProductCreationPropositionsSupplied()
        {
            var translator = new PropositionsContractTranslator();
            var avaliablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract() { Id = Guid.NewGuid() },
                    new PropositionContract() { Id = Guid.NewGuid() },
                    new PropositionContract() { Id = Guid.NewGuid() }
                }
            };
            var result = translator.Translate(avaliablePropositions, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Propositions.Where(x => x.IsAvailableForProductCreation).Count());
        }

        [TestMethod]
        public void Translate_ShouldMappTestCommercialOfferPropositionCode()
        {
            string testCoOffPropCode1 = "TCOPC1";
            string testCoOffPropCode2 = "TCOPC2";
            string testCoOffPropCode3 = "TCOPC3";

            var translator = new PropositionsContractTranslator();

            var avaliablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = testCoOffPropCode1 },
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = testCoOffPropCode2 },
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = testCoOffPropCode3 }
                }
            };

            var translatedValue = translator.Translate(avaliablePropositions, null);

            Assert.IsTrue(translatedValue.Propositions.Any(x => x.TestComPropositionCode == testCoOffPropCode1));
            Assert.IsTrue(translatedValue.Propositions.Any(x => x.TestComPropositionCode == testCoOffPropCode2));
            Assert.IsTrue(translatedValue.Propositions.Any(x => x.TestComPropositionCode == testCoOffPropCode3));
        }

        [TestMethod]
        public void Translate_ShouldReturnNullTestCommercialOfferPropCode_IfInputTestComOffPropCodeIsNull()
        {
            var translator = new PropositionsContractTranslator();

            var avaliablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = null },
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = null },
                    new PropositionContract() { Id = Guid.NewGuid(), TestCommercialOfferPropositionCode = null }
                }
            };

            var translatedValue = translator.Translate(avaliablePropositions, null);

            Assert.IsTrue(translatedValue.Propositions.All(x => x.TestComPropositionCode == null));
        }

        [TestMethod]
        public void Translate_ShouldReturnResultIfSubscriptionCodeIsCorrect()
        {
            var translatorUnderTest = new PropositionsContractTranslator();
            var listOfSubscribtionTypeCodes = new List<string>() { "PPU", "SHB", "EUS" };

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10"

                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            var SubscriptionTypeCode = result.Propositions.Select(x => x.CoConfigurations.Select(y => y.SubscriptionCode).FirstOrDefault()).FirstOrDefault();
            Assert.IsNotNull(result);
            Assert.IsTrue(listOfSubscribtionTypeCodes.Contains(SubscriptionTypeCode));

        }

        [TestMethod]
        public void Translate_ShouldReturnWhiteListZoneNull_IfInputWhiteListZonesAreNull()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = null
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            var whiteListZone = result.Propositions.Select(x => x.CoConfigurations.Select(y => y.WhiteZones).FirstOrDefault()).FirstOrDefault();
            Assert.IsNotNull(result);
            Assert.IsNull(whiteListZone);

        }

        [TestMethod]
        public void Translate_ShouldSkippTresholdLimitsWithDefaultValues_EmptyArrayTresholdLimitsTranslated()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.Empty, Limit = 0, Type = null}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10"

                                }
                            }
                        }
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);

            var tresholdLimits = translatedValue.Propositions.Select(x => x.CoConfigurations.Select(y => y.TresholdLimits).FirstOrDefault()).FirstOrDefault();

            Assert.IsTrue(tresholdLimits.Count() == 0);
        }

        [TestMethod]
        public void Translate_ShouldSkippTresholdLimitsWithDefaultValues()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.Empty, Limit = 0, Type = null},
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.Empty, Limit = 0, Type = null},
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 3, Type = "QUOTA" },
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 2, Type = "BUNDLE" },
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10"

                                }
                            }
                        }
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);

            var tresholdLimits = translatedValue.Propositions.Select(x => x.CoConfigurations.Select(y => y.TresholdLimits).FirstOrDefault()).FirstOrDefault();

            Assert.IsTrue(tresholdLimits.Count() == 2);
        }

        [TestMethod]
        public void Translate_ShouldReturnNullBlackListZones_IfInputBlackListZonesIsNull()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = null,
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = new CommercialOfferZonesContract {Zones = new List<int> { 1,2,3} }
                                }
                            }
                        }
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);

            var blackListZones = translatedValue.Propositions.Select(x => x.CoConfigurations.Select(y => y.BlackZones).FirstOrDefault()).FirstOrDefault();

            Assert.IsNull(blackListZones);
        }

        [TestMethod]
        public void Translate_ShouldReturnTresholdLimitsNull_IfInputTresholdLimitsIsNull()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = null,
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = null,
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = new CommercialOfferZonesContract {Zones = new List<int> { 1,2,3} }
                                }
                            }
                        }
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);

            var tresholdLimits = translatedValue.Propositions.Select(x => x.CoConfigurations.Select(y => y.TresholdLimits).FirstOrDefault()).FirstOrDefault();

            Assert.IsNull(tresholdLimits);
        }

        [TestMethod]
        public void Translate_ShouldHandlePropositionWithNoCommercialOffers()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract()
                    {
                        CommercialOfferConfigurationsContract = null
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);
            Assert.IsNotNull(translatedValue);
            Assert.IsNotNull(translatedValue.Propositions);
            Assert.AreEqual(1, translatedValue.Propositions.Count);
            Assert.IsNull(translatedValue.Propositions[0].CoConfigurations);
        }

        [TestMethod]
        public void Translate_ShouldHandlePropositionWithCommercialOfferWithoutDefinitions()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
                {
                    new PropositionContract()
                    {
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract()
                        {
                            CommercialOfferConfigurationContracts = null
                        }
                    }
                }
            };

            var translatedValue = translatorUnderTest.Translate(availablePropositions, null);
            Assert.IsNotNull(translatedValue);
            Assert.IsNotNull(translatedValue.Propositions);
            Assert.AreEqual(1, translatedValue.Propositions.Count);
            Assert.IsNull(translatedValue.Propositions[0].CoConfigurations);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_ZoneType()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = null,
                                    ZoneType = "5Zone"
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            var listOfDigits = new List<int?> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            Assert.IsTrue(listOfDigits.Contains(result.Propositions[0].CoConfigurations[0].AvailableZones));
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_WhenZoneTypeDoesNotContainsDigits()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = null,
                                    ZoneType = "Zone One"
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);

            Assert.IsNull(result.Propositions[0].CoConfigurations[0].AvailableZones);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_WhenZoneTypeDoesNotContainsDigitsAtTheStartOfSequence()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = null,
                                    ZoneType = "Zone1"
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);

            Assert.IsNull(result.Propositions[0].CoConfigurations[0].AvailableZones);
        }

        [TestMethod]
        public void Translate_ShouldReturnNullValueForAvailableZones_WhenZoneTypeIsNull()
        {
            var translatorUnderTest = new PropositionsContractTranslator();

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    WhiteListedZones = null,
                                    ZoneType = null
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);

            Assert.IsNull(result.Propositions[0].CoConfigurations[0].AvailableZones);
        }

        [TestMethod]
        public void Translate_ShouldMap_Metering()
        {
            var translatorUnderTest = new PropositionsContractTranslator();
            var listOfSubscribtionTypeCodes = new List<string>() { "PPU", "SHB", "EUS" };

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 5, Type = "METERING"},
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 3, Type = "METERING"},
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 8, Type = "METERING"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    QuotaMeteringZones = new CommercialOfferZonesContract { Zones = new List<int> { 4,6,7,9} }
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            var metering = result.Propositions[0].CoConfigurations[0].Metering;
            
            var meteringActualTresholdLimit = new List<decimal>();
            var meteringExpectedTresholdLimit = new List<decimal>();
            foreach (var tresholdLimit in metering.ThresholdLimits)
            {
                meteringActualTresholdLimit.Add(tresholdLimit.Limit);
            }

            foreach (var tresholdLimit in availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits)
            {
                meteringExpectedTresholdLimit.Add(tresholdLimit.Limit);
            }

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(metering.Zones, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].QuotaMeteringZones.Zones);
            CollectionAssert.AreEqual(meteringExpectedTresholdLimit, meteringActualTresholdLimit);
            Assert.AreEqual(metering.Amount, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits.Max(l => l.Limit));
        }

        [TestMethod]
        public void Translate_ShouldMap_Different_TresholdTypes_Correctly()
        {
            var translatorUnderTest = new PropositionsContractTranslator();
            var listOfSubscribtionTypeCodes = new List<string>() { "PPU", "SHB", "EUS" };

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,2,3,4} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 4, Type = "METERING"},
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 2, Type = "QUOTA" }
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data10",
                                    Code = "EUS-10",
                                    QuotaMeteringZones = new CommercialOfferZonesContract { Zones = new List<int> { 4,6,7,9} }
                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            var metering = result.Propositions[0].CoConfigurations[0].Metering;

            var meteringActualTresholdLimit = new List<decimal>();
            var meteringExpectedTresholdLimit = new List<decimal>();
            foreach (var tresholdLimit in metering.ThresholdLimits)
            {
                meteringActualTresholdLimit.Add(tresholdLimit.Limit);
            }

            foreach (var tresholdLimit in availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits)
            {
                if(tresholdLimit.Type == "METERING")
                meteringExpectedTresholdLimit.Add(tresholdLimit.Limit);
            }

            Assert.AreEqual(result.Propositions[0].CoConfigurations[0].TresholdLimits[0].Type, TypeOfTreshold.QUOTA);
            Assert.AreEqual(result.Propositions[0].CoConfigurations[0].TresholdLimits[0].Limit, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits[1].Limit);
            Assert.AreEqual(result.Propositions[0].CoConfigurations[0].TresholdLimits[0].Id, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits[1].Id);

            CollectionAssert.AreEqual(meteringExpectedTresholdLimit, meteringActualTresholdLimit);
            CollectionAssert.AreEqual(metering.Zones, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].QuotaMeteringZones.Zones);
            Assert.AreEqual(metering.Amount, availablePropositions.PropositionContracts[0].CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts[0].ThresHoldLimits.Max(l => l.Limit));
        }

        [TestMethod]
        public void Translate_ShouldReturn_Metering_AsNull_WhenMeteringNotDefined()
        {
            var translatorUnderTest = new PropositionsContractTranslator();
            var listOfSubscribtionTypeCodes = new List<string>() { "PPU", "SHB", "EUS" };

            var availablePropositions = new PropositionsContract
            {
                PropositionContracts = new List<PropositionContract>
                {
                    new PropositionContract
                    {
                        Id = Guid.NewGuid(),
                        CommercialOfferConfigurationsContract = new CommercialOfferConfigurationsContract
                        {
                            CommercialOfferConfigurationContracts = new List<CommercialOfferConfigurationContract>
                            {
                                new CommercialOfferConfigurationContract
                                {
                                    BlackListedZones = new CommercialOfferZonesContract { Zones = new List<int> { 1,5,7,8} },
                                    IsSharedWallet = true,
                                    ServiceLevelTypeCode = "DATA",
                                    SubscriptionTypeCode = "EUS",
                                    ThresHoldLimits = new List<CommercialOfferThresHoldLimitContract>
                                    {
                                        new CommercialOfferThresHoldLimitContract { Id = Guid.NewGuid(), Limit = 1, Type = "BUNDLE"}
                                    },
                                    BundleAmount = 1,
                                    BundlePeriod = new CommercialOfferBundlePeriodContract { PeriodeCode = "Month" },
                                    Name = "Typical",
                                    Quota = 50,
                                    PricePlanCode = "Data34",
                                    Code = "EUS-34"

                                }
                            }
                        }
                    }
                }
            };

            var result = translatorUnderTest.Translate(availablePropositions, null);
            
            Assert.IsNotNull(result);
            Assert.IsNull(result.Propositions[0].CoConfigurations[0].Metering);
        }
    }
}

