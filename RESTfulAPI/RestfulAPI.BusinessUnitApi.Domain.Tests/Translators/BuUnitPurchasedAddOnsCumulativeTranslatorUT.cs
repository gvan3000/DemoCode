using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn;
using RestfulAPI.TeleenaServiceReferences.AddOnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BuUnitPurchasedAddOnsCumulativeTranslatorUT
    {
        private BusinessUnitPurchasedAddOnsCumulativeTranslator translatorUnderTest;

        Guid _dataSpeedId1 = Guid.NewGuid();
        Guid _dataSpeedId2 = Guid.NewGuid();
        Guid _dataSpeedId3 = Guid.NewGuid();
        Guid _dataSpeedId4 = Guid.NewGuid();

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new BusinessUnitPurchasedAddOnsCumulativeTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfInputIsNull()
        {
            BusinessUnitPurchasedAddOnListContract input = null;

            var translated = translatorUnderTest.Translate(input);

            Assert.IsNull(translated);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_IfInputProperty_BusinessUnitPurchasedAddOns_IsNull()
        {
            BusinessUnitPurchasedAddOnListContract input = new BusinessUnitPurchasedAddOnListContract { BusinessUnitPurchasedAddOns = null };

            var translated = translatorUnderTest.Translate(input);

            Assert.IsNull(translated);
        }

        [TestMethod]
        public void Translate_DifferentAddOnsSameAddOnId_ReturnSameTranslatedNumberOfAddOns()
        {
            var input = PrepareAddOns_call1();

            var translated = translatorUnderTest.Translate(input);

            Assert.AreEqual(input.BusinessUnitPurchasedAddOns.Count(), translated.AddOns.Count());
        }

        [TestMethod]
        public void Translate_DifferentAddOnsSameAddOnId_ReturnSameTranslatedNumberOfAddOns_OneDefinitionsPerAddOn()
        {
            var input = PrepareAddOns_call1();

            var translated = translatorUnderTest.Translate(input);

            Assert.IsTrue(translated.AddOns.All(x => x.Definitions.Count() == 1));
        }

        [TestMethod]
        public void Translate_IfPCRFAddOnsAreDifferent_ShouldReturnSameNumberAddOns()
        {
            var input = PrepareAddOns_call2();

            var translated = translatorUnderTest.Translate(input);

            Assert.AreEqual(input.BusinessUnitPurchasedAddOns.Count(), translated.AddOns.Count());
        }

        [TestMethod]
        public void Translate_ShouldGroupAddOns_AndReturnListOFDefinitions()
        {
            var input = PrepareAddOns_call3();

            var translated = translatorUnderTest.Translate(input);

            Assert.AreEqual(1, translated.AddOns.Count());
            Assert.IsTrue(translated.AddOns.All(x => x.Definitions.Count() > 1));

        }

        [TestMethod]
        public void Translate_ShouldReturn_DataSpeedId()
        {
            var input = PrepareAddOns_call4();

            var translated = translatorUnderTest.Translate(input);

            var addOnsSpeedIds = translated.AddOns.Select(x => x.SpeedId).ToList();

            var dataSpeed1Count = addOnsSpeedIds.Where(a => a == _dataSpeedId1).Count();
            var dataSpeed2Count = addOnsSpeedIds.Where(a => a == _dataSpeedId2).Count();
            var dataSpeed3Count = addOnsSpeedIds.Where(a => a == _dataSpeedId3).Count();

            Assert.AreEqual(2, dataSpeed1Count);
            Assert.AreEqual(1, dataSpeed2Count);
            Assert.AreEqual(2, dataSpeed3Count);
        }

        private BusinessUnitPurchasedAddOnListContract PrepareAddOns_call1()
        {
            BusinessUnitPurchasedAddOnListContract addOns = new BusinessUnitPurchasedAddOnListContract
            {
                BusinessUnitPurchasedAddOns = new List<BusinessUnitPurchasedAddOnContract>
                {
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:17:41.757"),
                        EndDate = null,
                        Resourceid = 9,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD"
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:01.290"),
                        EndDate = null,
                        Resourceid = 10,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD"
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:12.110"),
                        EndDate = null,
                        Resourceid = 5,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD"
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:15.933"),
                        EndDate = null,
                        Resourceid = 7,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD"
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 14:44:27.993"),
                        EndDate = null,
                        Resourceid = 8,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD"
                    }
                }
            };

            return addOns;
        }

        private BusinessUnitPurchasedAddOnListContract PrepareAddOns_call4()
        {
            var addOns = new BusinessUnitPurchasedAddOnListContract
            {
                BusinessUnitPurchasedAddOns = new List<BusinessUnitPurchasedAddOnContract>
                {
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:17:41.757"),
                        EndDate = null,
                        Resourceid = 9,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD",
                        DataSpeedId = _dataSpeedId1
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:01.290"),
                        EndDate = null,
                        Resourceid = 10,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD",
                        DataSpeedId = _dataSpeedId2
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:12.110"),
                        EndDate = null,
                        Resourceid = 5,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD",
                        DataSpeedId = _dataSpeedId3
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 03:19:15.933"),
                        EndDate = null,
                        Resourceid = 7,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD",
                        DataSpeedId = _dataSpeedId3
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("3ED34DAD-9D49-44D8-91B9-E5F484FF6291"),
                        Name = "Teleena - SHB 2hrs",
                        AddOnType = "CASH",
                        Amount = 23.0000M,
                        StartDate = DateTime.Parse("2017-07-12 14:44:27.993"),
                        EndDate = null,
                        Resourceid = 8,
                        ServiceTypeCode = "QUOTA",
                        PcrfTypeCode = null,
                        AddOnSubType = "QUOTA",
                        UnitType = "USD",
                        DataSpeedId = _dataSpeedId1
                    }
                }
            };

            return addOns;
        }

        private BusinessUnitPurchasedAddOnListContract PrepareAddOns_call2()
        {
            BusinessUnitPurchasedAddOnListContract addOns = new BusinessUnitPurchasedAddOnListContract
            {
                BusinessUnitPurchasedAddOns = new List<BusinessUnitPurchasedAddOnContract>
                {
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("44A1351A-7FFA-4CB4-97CA-0CD0261D52B6"),
                        Name = "Teleena - Pcrf - addon1",
                        AddOnType = "PCRF",
                        Amount = 20.0000M,
                        StartDate = DateTime.Parse("2017-08-09T01:16:02.770z"),
                        EndDate = null,
                        Resourceid = 1,
                        ServiceTypeCode = null,
                        PcrfTypeCode = "YT",
                        AddOnSubType = null,
                        UnitType = null
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("DB2A0472-B371-471C-9D0C-24E67EEE64E2"),
                        Name = "Teleena - Pcrf - addOn 2",
                        AddOnType = "PCRF",
                        Amount = 10.0000M,
                        StartDate = DateTime.Parse("2017-08-09T11:12:14.463z"),
                        EndDate = null,
                        Resourceid = 5,
                        ServiceTypeCode = null,
                        PcrfTypeCode = "SK",
                        AddOnSubType = null,
                        UnitType = null
                    }
                }
            };

            return addOns;
        }

        private BusinessUnitPurchasedAddOnListContract PrepareAddOns_call3()
        {
            BusinessUnitPurchasedAddOnListContract addOns = new BusinessUnitPurchasedAddOnListContract
            {
                BusinessUnitPurchasedAddOns = new List<BusinessUnitPurchasedAddOnContract>
                {
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("C0EE52A8-AA47-424E-9895-2C363CF36709"),
                        Name = "Roaming - 1 hrs",
                        AddOnType = Constants.AddOnConstants.ROAMING,
                        Amount = 20.0000M,
                        StartDate = DateTime.Parse("2017-09-13T09:06:29.790z"),
                        EndDate = DateTime.Parse("2017-09-13T08:06:30.373z"),
                        Resourceid = 5,
                        ServiceTypeCode = "DATA",
                        PcrfTypeCode = null,
                        AddOnSubType = null,
                        UnitType = "MB"
                    },
                    new BusinessUnitPurchasedAddOnContract
                    {
                        Id = Guid.Parse("C0EE52A8-AA47-424E-9895-2C363CF36709"),
                        Name = "Roaming - 1 hrs",
                        AddOnType = Constants.AddOnConstants.ROAMING,
                        Amount = 20.0000M,
                        StartDate = DateTime.Parse("2017-09-13T09:06:29.790z"),
                        EndDate = DateTime.Parse("2017-09-13T08:06:30.373z"),
                        Resourceid = 5,
                        ServiceTypeCode = "VOICE",
                        PcrfTypeCode = null,
                        AddOnSubType = null,
                        UnitType = "MINUTES"
                    }
                }
            };

            return addOns;
        }
    }
}
