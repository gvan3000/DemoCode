using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit
{
    /// <summary>
    /// AccountContract Translator
    /// </summary>
    public class AccountContractTranslator : IAccountContractTranslator
    {
        /// <summary>
        /// Translate fetched accounts, propositions and addOns to BusinessUnitListModel 
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="propositionsAddOns"></param>
        /// <param name="pricePlans"></param>
        /// <param name="resolveParentChildRelationship"></param>
        /// <returns></returns>
        public BusinessUnitListModel Translate(List<AccountContract> accounts, List<BusinessUnitExtraInfoModel> propositionsAddOns,
                                               List<PricePlanContract> pricePlans, bool resolveParentChildRelationship = false)
        {
            if (accounts == null)
                return null;

            List<BusinessUnitModel> businessUnitModels = accounts.Select(Translate).Distinct(new BusinessUnitModelEqualityComparer()).ToList();

            JoinPropositionsAndAddOnsToBusinessUnits(businessUnitModels, propositionsAddOns);

            JoinPlansToBusinessUnits(businessUnitModels, accounts);

            foreach (BusinessUnitModel businessUnit in businessUnitModels)
            {
                PricePlanContract pricePlanContract = pricePlans.Find(x => x.RateKey == businessUnit.RateKey);
                businessUnit.WholesalePricePlan = pricePlanContract != null ? pricePlanContract.Description : null;
            }

            if (resolveParentChildRelationship)
            {
                var treeList = new List<BusinessUnitModel>(businessUnitModels);
                foreach (var item in accounts)
                {
                    var children = businessUnitModels.Where(x => x.ParentId == item.Id).ToList();
                    treeList.RemoveAll(x => x.ParentId == item.Id);
                    businessUnitModels.First(x => x.Id == item.Id).Children = children;
                }
                businessUnitModels = treeList;
            }

            return new BusinessUnitListModel() { BusinessUnits = businessUnitModels };
        }

        private void JoinPlansToBusinessUnits(List<BusinessUnitModel> translatedList, List<AccountContract> accounts)
        {
            var plansByAccount = accounts.GroupBy(account => account.Id, account => account.PlanId, (key, group) => new
            {
                Id = key,
                Plans = group.ToList()
            }).ToList();

            foreach (var businessUnitModel in translatedList)
            {
                List<Guid?> planIds = plansByAccount.FirstOrDefault(plan => plan.Id == businessUnitModel.Id).Plans;

                businessUnitModel.PlanIds = (planIds != null && planIds.All(p => p != null)) ? planIds : new List<Guid?>();
            }
        }

        /// <summary>
        /// Translate BusinessUnit Model
        /// </summary>
        /// <param name="input">Account wcf contract</param>
        /// <returns></returns>
        public BusinessUnitModel Translate(AccountContract input)
        {
            if (input == null)
                return null;

            BusinessUnitModel businessUnit = new BusinessUnitModel()
            {
                Id = input.Id,
                Name = input.UserName,
                ParentId = input.ParentId,
                PersonId = input.PersonId,
                CustomerId = input.CustomerNumber,
                Propositions = null,
                AddOns = null,
                HasSharedWallet = input.IsSharedWallet,
                Children = new List<BusinessUnitModel>(),
                RateKey = input.RateKey,
                BillCycleStartDay = input.BillCycleStartDay
            };

            return businessUnit;
        }

        private Models.AddOnModels.AddOn TranslateSingleAddOn(SimpleAddOnContract input)
        {
            if (input == null)
                return null;

            var result = new Models.AddOnModels.AddOn()
            {
                AddOnType = input.AddOnType,
                Id = input.AddOnId,
                IsOneTime = input.IsOneTime
            };

            return result;
        }

        private Models.BusinessUnitModels.Proposition TranslateSingleProposition(AllowedPropositionContract input)
        {
            if (input == null)
                return null;

            var result = new Models.BusinessUnitModels.Proposition()
            {
                Id = input.PropositionId,
                BusinessUnitId = input.AccountId,
                EndUserSubscription = input.EndUserSubscription
            };

            return result;
        }

        private void JoinPropositionsAndAddOnsToBusinessUnits(List<BusinessUnitModel> businessUnits, List<BusinessUnitExtraInfoModel> propositionsAddOns)
        {
            foreach (var businessUnit in businessUnits)
            {
                var foundExtraInfo = propositionsAddOns.FirstOrDefault(x => x.BusinessUnitId == businessUnit.Id);
                if (foundExtraInfo != null)
                {
                    businessUnit.Propositions = foundExtraInfo.Propositions.Select(TranslateSingleProposition).ToList();
                    businessUnit.AddOns = foundExtraInfo.AddOns.Select(TranslateSingleAddOn).ToList();
                }
            }
        }
    }
}
