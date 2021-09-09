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
    public class BusinessUnitIdFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_ShouldReturnTrueWhenIncludeChildrenSetANdFitlerBusinessUnitSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = Guid.NewGuid(),
                IncludeChildren = true
            };

            var filterUnderTest = new BusinessUnitIdFilter();
            Assert.IsTrue(filterUnderTest.CanApplyFilter(request));
        }

        [TestMethod]
        public void CanApplyFilter_ShouldReturnFalseWhenAppropriate()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = Guid.NewGuid(),
                IncludeChildren = false
            };

            var filterUnderTest = new BusinessUnitIdFilter();
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
            request.FilterBusinessUnitId = null;
            request.IncludeChildren = true;
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
        }

        [TestMethod]
        public void FilterBusinessUnitsByRequest_ShouldIncludeAllBusinessUnitsInHierarchy()
        {
            /////////////////////////////////////////////////
            // test account hierarchy
            /////////////////////////////////////////////////
            // acc[0]
            // --acc[1]
            // ----acc[3]
            // ----acc[4]
            // --acc[2]
            var inputAccounts = new List<AccountContract>()
            {
                new AccountContract()
                {
                    Id = Guid.NewGuid()
                }
            };

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id
            });

            var request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = inputAccounts[0].Id
            };


            var filterUnderTest = new BusinessUnitIdFilter();
            var result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(inputAccounts.Count, result.Count());

            request.FilterBusinessUnitId = inputAccounts[1].Id;

            result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }
    }
}
