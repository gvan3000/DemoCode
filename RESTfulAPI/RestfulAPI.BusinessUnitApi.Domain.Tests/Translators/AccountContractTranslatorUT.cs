using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    internal static class ShuffleExtension
    {
        public static void ShuffleInPlace<T>(this List<T> input)
        {
            var rng = new Random();
            var randomInt = rng.Next(input.Count);
            T temp;
            for (var i = 0; i < input.Count; ++i)
            {
                temp = input[i];
                input[i] = input[randomInt];
                input[randomInt] = temp;
                randomInt = rng.Next(input.Count);
            }
        }
    }

    [TestClass]
    public class AccountContractTranslatorUT
    {
        AccountContractTranslator _translatorUnderTest;

        [TestInitialize]
        public void Setup()
        {
            _translatorUnderTest = new AccountContractTranslator();
        }

        public TestContext TestContext { get; set; }

        private const int rateKey = 77;

        private AccountContract GenerateAccount(Guid id, Guid parentId)
        {
            return new AccountContract()
            {
                Id = id,
                UserName = "some name",
                ParentId = parentId,
                PersonId = Guid.NewGuid(),
                CustomerNumber = "123456789abc",
                RateKey = rateKey
            };
        }

        private void LogTestAccountData(IEnumerable<AccountContract> data, [System.Runtime.CompilerServices.CallerMemberName] string calledFrom = "<unknown>")
        {
            TestContext.WriteLine($"loggin test data for {calledFrom}");
            if (data.Any(item => item == null))
            {
                TestContext.WriteLine("test data contains nulls, this is not going to work well");
            }
            TestContext.WriteLine(string.Empty);
            foreach (var item in data)
            {
                TestContext.WriteLine($"\titem: {nameof(item.Id)} = {item.Id}, {nameof(item.ParentId)} = {item.ParentId}");
            }
        }

        [TestMethod]
        public void Translate_ShouldReturnNullIfInputContractIsNull()
        {
            List<AccountContract> input = null;
            List<BusinessUnitExtraInfoModel> propositions = null;

            var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

            Assert.IsNull(response);
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyModelIfContractIsEmptyList()
        {
            List<AccountContract> input = new List<AccountContract>();
            List<BusinessUnitExtraInfoModel> propositions = null;

            var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.BusinessUnits);
            Assert.AreEqual(0, response.BusinessUnits.Count);
        }

        [TestMethod]
        public void Translate_ShouldReturnSingleResultForOneInputGiven()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            var input = new List<AccountContract>()
            {
                new AccountContract()
                {
                    Id = Guid.NewGuid(),
                    UserName = "some name",
                    ParentId = Guid.NewGuid(),
                    PersonId = Guid.NewGuid(),
                    CustomerNumber = "123456789abc"
                }
            };
            List<BusinessUnitExtraInfoModel> propositions = new List<BusinessUnitExtraInfoModel>();

            var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.BusinessUnits);
            Assert.AreEqual(1, response.BusinessUnits.Count);

            response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), true);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.BusinessUnits);
            Assert.AreEqual(1, response.BusinessUnits.Count);
        }

        [TestMethod]
        public void Translate_ShouldReturnValidReconstructedTreeCase1()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // ----acc[2]
            // ------acc[3]
            var input = new List<AccountContract>();
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[1].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));

            List<BusinessUnitExtraInfoModel> propositions = new List<BusinessUnitExtraInfoModel>();

            for (var i = 0; i < 10; ++i)
            {
                input.ShuffleInPlace();

                LogTestAccountData(input);

                var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(input.Count, response.BusinessUnits.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children.Count);

                response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(1, response.BusinessUnits.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children[0].Children.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children[0].Children[0].Children.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children[0].Children[0].Children[0].Children.Count);
            }
        }

        [TestMethod]
        public void Translate_ShouldReturnValidResonstrutedTreeCase2()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // --acc[2]
            // --acc[3]
            var input = new List<AccountContract>();
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));

            List<BusinessUnitExtraInfoModel> propositions = new List<BusinessUnitExtraInfoModel>();

            for (var i = 0; i < 10; ++i)
            {
                input.ShuffleInPlace();

                LogTestAccountData(input);

                var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(input.Count, response.BusinessUnits.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children.Count);

                response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(1, response.BusinessUnits.Count);
                Assert.AreEqual(3, response.BusinessUnits[0].Children.Count);
            }
        }

        [TestMethod]
        public void Translate_ShouldReturnValidReconstructedTreeCase3()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // acc[2]
            // --acc[3]
            var input = new List<AccountContract>();
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));

            List<BusinessUnitExtraInfoModel> propositions = new List<BusinessUnitExtraInfoModel>();

            for (var i = 0; i < 10; ++i)
            {
                input.ShuffleInPlace();

                LogTestAccountData(input);

                var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(input.Count, response.BusinessUnits.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children.Count);

                response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(2, response.BusinessUnits.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children.Count);
                Assert.AreEqual(1, response.BusinessUnits[1].Children.Count);
            }
        }

        [TestMethod]
        public void Translate_ShouldReturnValidReconstructedTreeCase4()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // acc[2]
            // --acc[3]
            // --acc[4]
            // acc[5]
            var input = new List<AccountContract>();
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), Guid.NewGuid()));

            List<BusinessUnitExtraInfoModel> propositions = new List<BusinessUnitExtraInfoModel>();

            for (var i = 0; i < 10; ++i)
            {
                input.ShuffleInPlace();

                LogTestAccountData(input);

                var response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), false);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(input.Count, response.BusinessUnits.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children.Count);

                response = _translatorUnderTest.Translate(input, propositions, new List<PricePlanContract>(), true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(3, response.BusinessUnits.Count);
                Assert.IsTrue(response.BusinessUnits.Any(x => x.Children.Count == 0));
                Assert.IsTrue(response.BusinessUnits.Any(x => x.Children.Count == 1));
                Assert.IsTrue(response.BusinessUnits.Any(x => x.Children.Count == 2));
            }
        }

        [TestMethod]
        public void Translate_ShouldReturnValidReconstructedTreeCase5()
        {
            ////////////////////////////////////////////////
            // structure
            ////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // ----acc[2]
            // ------acc[3]
            // ------acc[4]

            Guid businessUnitID = Guid.NewGuid();

            var input = new List<AccountContract>();
            input.Add(GenerateAccount(businessUnitID, Guid.NewGuid()));
            input.Add(GenerateAccount(Guid.NewGuid(), input[0].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[1].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));
            input.Add(GenerateAccount(Guid.NewGuid(), input[2].Id));

            var addOns = new List<SimpleAddOnContract>()
            {
                new SimpleAddOnContract()
                {
                    AddOnId = Guid.NewGuid(),
                    AddOnType = "my_type"
                },
                new SimpleAddOnContract()
                {
                    AddOnId = Guid.NewGuid(),
                    AddOnType = "it_type"
                }
            };

            List<AllowedPropositionContract> propositions = new List<AllowedPropositionContract>()
            {
                new AllowedPropositionContract() { PropositionId = new Guid() }
            };

            List<BusinessUnitExtraInfoModel> propositionList = new List<BusinessUnitExtraInfoModel>
            {
                new BusinessUnitExtraInfoModel {BusinessUnitId = businessUnitID, AddOns = addOns, Propositions = propositions }
            };

            for (var i = 0; i < 10; ++i)
            {
                input.ShuffleInPlace();

                LogTestAccountData(input);

                var response = _translatorUnderTest.Translate(input, propositionList, new List<PricePlanContract>(), false);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(input.Count, response.BusinessUnits.Count);
                Assert.AreEqual(0, response.BusinessUnits[0].Children.Count);

                response = _translatorUnderTest.Translate(input, propositionList, new List<PricePlanContract>(), true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.BusinessUnits);
                Assert.AreEqual(1, response.BusinessUnits.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children.Count);
                Assert.AreEqual(1, response.BusinessUnits[0].Children[0].Children.Count);
                Assert.AreEqual(2, response.BusinessUnits[0].Children[0].Children[0].Children.Count);
            }
        }

        [TestMethod]
        public void Translate_SholdTranslate_PropositionsAndAddOns()
        {
            Guid businessUnitID = Guid.NewGuid();

            var input = new List<AccountContract>();
            input.Add(GenerateAccount(businessUnitID, Guid.NewGuid()));

            var addOns = new List<SimpleAddOnContract>()
            {
                new SimpleAddOnContract()
                {
                    AddOnId = Guid.NewGuid(),
                    AddOnType = "my_type"
                },
                new SimpleAddOnContract()
                {
                    AddOnId = Guid.NewGuid(),
                    AddOnType = "it_type"
                }
            };

            List<AllowedPropositionContract> propositions = new List<AllowedPropositionContract>()
            {
                new AllowedPropositionContract() { PropositionId = new Guid() },
                new AllowedPropositionContract() { PropositionId = new Guid() }
            };

            List<BusinessUnitExtraInfoModel> propositionAddOnsList = new List<BusinessUnitExtraInfoModel>
            {
                new BusinessUnitExtraInfoModel {BusinessUnitId = businessUnitID, AddOns = addOns, Propositions = propositions }
            };

            var response = _translatorUnderTest.Translate(input, propositionAddOnsList, new List<PricePlanContract>(), false);

            Assert.IsTrue(response.BusinessUnits.Any(x => x.AddOns.Count() == addOns.Count()));
            Assert.IsTrue(response.BusinessUnits.Any(x => x.Propositions.Count() == propositions.Count()));
        }

        [TestMethod]
        public void Translate_ShouldTranslate_WholePricePlan()
        {
            Guid businessUnitID = Guid.NewGuid();
            string description = "SomeDescription";
            var input = new List<AccountContract>();
            input.Add(GenerateAccount(businessUnitID, Guid.NewGuid()));
            var pricePlanList = new List<PricePlanContract>();
            pricePlanList.Add(new PricePlanContract() { Description = "SomeDescription", RateKey = rateKey });

            BusinessUnitListModel translatedBusinessUnits = _translatorUnderTest.Translate(input,
                new List<BusinessUnitExtraInfoModel>(), pricePlanList);

            Assert.AreEqual(translatedBusinessUnits.BusinessUnits.First().WholesalePricePlan, description);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_EndUserSubscription()
        {
            Guid businessUnitID = Guid.NewGuid(); var input = new List<AccountContract>();
            AccountContract account = GenerateAccount(businessUnitID, Guid.NewGuid());
            account.IsSharedWallet = true;
            input.Add(account);

            BusinessUnitListModel translatedBusinessUnits = _translatorUnderTest.Translate(input,
                new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>());

            Assert.IsTrue(translatedBusinessUnits.BusinessUnits.First().HasSharedWallet);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_BillCycleStartDay()
        {
            int billCycleStartDay = 10;

            Guid businessUnitID = Guid.NewGuid();

            var input = new List<AccountContract>();

            AccountContract account = new AccountContract { Id = businessUnitID, BillCycleStartDay = billCycleStartDay };

            input.Add(account);

            BusinessUnitListModel translatedBusinessUnits = _translatorUnderTest.Translate(input, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>());

            int? translatedBillCycleStartDay = translatedBusinessUnits.BusinessUnits.Where(x => x.Id == businessUnitID).Select(x => x.BillCycleStartDay).FirstOrDefault();

            Assert.AreEqual(billCycleStartDay, translatedBillCycleStartDay);
        }


        [TestMethod]
        public void Translate_ShouldTranslateBillCycleStartDayToNull_IfInputBValueIsNull()
        {
            int? billCycleStartDay = null;

            Guid businessUnitID = Guid.NewGuid();

            var input = new List<AccountContract>();

            AccountContract account = new AccountContract { Id = businessUnitID, BillCycleStartDay = billCycleStartDay };

            input.Add(account);

            BusinessUnitListModel translatedBusinessUnits = _translatorUnderTest.Translate(input, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>());

            int? translatedBillCycleStartDay = translatedBusinessUnits.BusinessUnits.Where(x => x.Id == businessUnitID).Select(x => x.BillCycleStartDay).FirstOrDefault();

            Assert.IsNull(translatedBillCycleStartDay);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_PlanIds_ByAccounts_BuTreeStructure()
        {
            var accounts = CreateParentChildsWithPlansAccounts();

            var result = _translatorUnderTest.Translate(accounts,new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>(), true);

            var parentBusinessUnit = result.BusinessUnits.FirstOrDefault();
            var childBusinessUnits = result.BusinessUnits.Select(x => x.Children).ToList();

            var parentBuPlanIds = parentBusinessUnit.PlanIds;
            var childBuAPlanIds = childBusinessUnits.Select(x => x.FirstOrDefault(a => a.Id == Guid.Parse("6E31CED7-5C6C-E711-A314-0050569D19DC")).PlanIds).FirstOrDefault();
            var childBuBPlanIds = childBusinessUnits.Select(x => x.FirstOrDefault(a => a.Id == Guid.Parse("3319C22E-5F6C-E711-A314-0050569D19DC")).PlanIds).FirstOrDefault();

            Assert.AreEqual(3, childBuAPlanIds.Count);
            Assert.AreEqual(2, childBuBPlanIds.Count);
            Assert.IsNull(parentBuPlanIds.FirstOrDefault());
        }

        [TestMethod]
        public void Translate_ShouldTranslate_PlanIds_7DifferentBusinessIds_3PlanIds_TheSameParentID()
        {
            var accounts = CreateAccountsSharedPlansDistinctAccountIds();

            var result = _translatorUnderTest.Translate(accounts, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>(), true);

            var parentBusinesssUnit = result.BusinessUnits.FirstOrDefault();
            var children = result.BusinessUnits[0].Children;

            var parentBuPlanIds = parentBusinesssUnit.PlanIds;

            var childBuAPlanIds = children.Find(x => x.Id == Guid.Parse("1131CED7-5C6C-E711-A314-0050569D19DC")).PlanIds;
            var childBuBPlanIds = children.Find(x => x.Id == Guid.Parse("1231CED7-5C6C-E711-A314-0050569D19DC")).PlanIds;

            Assert.AreEqual(1, parentBuPlanIds.Count);
            Assert.AreEqual(1, childBuAPlanIds.Count);
            Assert.AreEqual(1, childBuBPlanIds.Count);
        }

        [TestMethod]
        public void Translate_ShouldTranslate_Plain_BusinessUnits_7DifferentBusinessIds_3PlanIds_TheSameParentID()
        {
            var accounts = CreateAccountsSharedPlansDistinctAccountIds();

            var result = _translatorUnderTest.Translate(accounts, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>(), false);

            var businessUnitATranslated = result.BusinessUnits.FirstOrDefault(x => x.Id == Guid.Parse("1431CED7-5C6C-E711-A314-0050569D19DC"));


            Assert.AreEqual(Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC"), businessUnitATranslated.PlanIds.FirstOrDefault());
            Assert.AreEqual(7, result.BusinessUnits.Count);           
        }

        [TestMethod]
        public void Translate_ShouldTranslate_Plans_AsEmptyArray_When_No_Plans()
        {
            var accounts = CreateAccountsWithoutPlansDistinctAccountIds();

            var result = _translatorUnderTest.Translate(accounts, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>(), true);

            var parentPlanIds = result.BusinessUnits.Select(x => x.PlanIds).FirstOrDefault();

            Assert.AreEqual(0, parentPlanIds.Count());
        }

        [TestMethod]
        public void Translate_ShouldTranslate_Plans_OnlyParentBuHasPlan()
        {
            var accounts = CreateAccountsParentBusinessUnitHasPlansDistinctAccountIds();

            var result = _translatorUnderTest.Translate(accounts, new List<BusinessUnitExtraInfoModel>(), new List<PricePlanContract>(), true);
            var children = result.BusinessUnits[0].Children;

            var parentPlanIds = result.BusinessUnits.Select(x => x.PlanIds).FirstOrDefault();

            var childBuAPlanIds = children.Find(x => x.Id == Guid.Parse("1131CED7-5C6C-E711-A314-0050569D19DC")).PlanIds;
            var childBuBPlanIds = children.Find(x => x.Id == Guid.Parse("1231CED7-5C6C-E711-A314-0050569D19DC")).PlanIds;

            Assert.AreEqual(1, parentPlanIds.Count());
            Assert.AreEqual(Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC"), parentPlanIds.FirstOrDefault());
            Assert.AreEqual(0, childBuAPlanIds.Count());
            Assert.AreEqual(0, childBuBPlanIds.Count());
        }

        private List<AccountContract> CreateAccountsParentBusinessUnitHasPlansDistinctAccountIds()
        {
            //3 different AccountIDs
            //1 Parent, 2 childs
            //Only parent BU has plan
            var accounts = new List<AccountContract>
            {
                CreateAccount(Guid.Parse("1131CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),null),

                CreateAccount(Guid.Parse("1231CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),null),

                //parent account
                CreateAccount(Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), null, Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC"))
            };

            return accounts;
        }

        private List<AccountContract> CreateAccountsWithoutPlansDistinctAccountIds()
        {
            //3 different AccountIDs
            //1 Parent, 2 childs
            //No plans
            var accounts = new List<AccountContract>
            {
                CreateAccount(Guid.Parse("1131CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),null),

                CreateAccount(Guid.Parse("1231CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),null),

                //parent account
                CreateAccount(Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), null, null)
            };

            return accounts;
        }

        private List<AccountContract> CreateAccountsSharedPlansDistinctAccountIds()
        {
            //7 different AccountIDs
            //1 Parent, 6 childs
            //3 different PlanIds
            var accounts = new List<AccountContract>
            {
                CreateAccount(Guid.Parse("1131CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),
                Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC")),

                CreateAccount(Guid.Parse("1231CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"),
                Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC")),

                CreateAccount(Guid.Parse("1331CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), 
                Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC")),

                CreateAccount(Guid.Parse("1431CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), 
                Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC")),

                CreateAccount(Guid.Parse("1531CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), 
                Guid.Parse("221F89F6-E550-E711-A8D4-0050569D19DC")),

                CreateAccount(Guid.Parse("1631CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), 
                Guid.Parse("AA1F89F6-E550-E711-A8D4-0050569D19DC")),

                //parent account
                CreateAccount(Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0"), null,
                Guid.Parse("AA1F89F6-E550-E711-A8D4-0050569D19DC"))
            };

            return accounts;
        }

        private List<AccountContract> CreateParentChildsWithPlansAccounts()
        {
            List<AccountContract> accounts = new List<AccountContract>();

            //Child AccountA - 3 PlanIds
            accounts.Add(CreateAccount(Guid.Parse("6E31CED7-5C6C-E711-A314-0050569D19DC"),Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"),
                                       Guid.Parse("5FF142DE-F5AA-4E0F-8102-1FDB1CA9BEC0")));
            accounts.Add(CreateAccount(Guid.Parse("6E31CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"), 
                                       Guid.Parse("302223C4-2B10-4A9A-A7F6-23914F588D34")));
            accounts.Add(CreateAccount(Guid.Parse("6E31CED7-5C6C-E711-A314-0050569D19DC"), Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"),
                                       Guid.Parse("B9ECFC87-D191-4FF9-BCC8-DF092CAEC997")));

            //Child AccountB - 2 PlanIds
            accounts.Add(CreateAccount(Guid.Parse("3319C22E-5F6C-E711-A314-0050569D19DC"), Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"),
                                       Guid.Parse("D40E6671-9EF2-41CA-AC61-5CD7051566BF")));
            accounts.Add(CreateAccount(Guid.Parse("3319C22E-5F6C-E711-A314-0050569D19DC"), Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"),
                                       Guid.Parse("504CE18C-6D43-421E-B521-E8E717755F4D")));

            //Parent AccountC - 0 PlanIds
            accounts.Add(CreateAccount(Guid.Parse("041F89F6-E550-E711-A8D4-0050569D19DC"), null, null));

            return accounts;
        }

        private AccountContract CreateAccount(Guid id, Guid? parentID, Guid? planId)
        {
            var account = new AccountContract()
            {
                Id = id,
                ParentId = parentID,
                AccountNumber = 2109007011,
                UserName = "DataTest123",
                CustomerNumber = "13243546576879",
                PersonId = null,
                RateKey = 1835,
                IsSharedWallet = false,
                BillCycleStartDay = null,
                HasEndUserSubscription = true,
                PlanId = planId
            };

            return account;
        }
    }
}
