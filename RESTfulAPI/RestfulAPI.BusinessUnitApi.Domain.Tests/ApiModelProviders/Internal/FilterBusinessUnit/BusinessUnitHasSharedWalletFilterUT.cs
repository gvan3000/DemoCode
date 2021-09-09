using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.FilterBusinessUnit
{
    [TestClass]
    public class BusinessUnitHasSharedWalletFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_ShouldReturn_True_WhenHasSharedWalletIsSet()
        {
            var request = new GetBusinessUnitRequest()
            {               
                FilterHasSharedWallet = "true"
            };

            var filterUnderTest = new BusinessUnitHasSharedWalletFilter();
            var result = filterUnderTest.CanApplyFilter(request);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanApplyFilter_ShouldReturn_False_WhenHasSharedWalletNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterCustomerId = "wert"
            };

            var filterUnderTest = new BusinessUnitHasSharedWalletFilter();
            var result = filterUnderTest.CanApplyFilter(request);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FilterBusinessUnitsByRequest_ShouldReturnAccountsWithSharedWalletFlagValueSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterHasSharedWallet = "true"
            };

            var inputAccounts = new List<AccountContract>()
            {
                new AccountContract()
                {
                    Id = Guid.NewGuid(),
                    IsSharedWallet = true
                }
            };

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                IsSharedWallet = true
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                IsSharedWallet = false
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                IsSharedWallet = false
            });

            var filterUnderTest = new BusinessUnitHasSharedWalletFilter();
            var result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts, request);
            
            Assert.IsFalse(result.Any(x => x.IsSharedWallet != true));
        }
    }
}
