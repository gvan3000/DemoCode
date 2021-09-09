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
    public class BusinessUnitCompanyIdFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_SholdReturnTrue()
        {
            GetBusinessUnitRequest request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid(),
                IncludeChildren = true
            };
            BusinessUnitCompanyIdFilter filter = new BusinessUnitCompanyIdFilter();

            bool canApply = filter.CanApplyFilter(request);

            Assert.IsTrue(canApply);
        }

        [TestMethod]
        public void CanApplyFilter_ShouldReturnFalse()
        {
            GetBusinessUnitRequest request = new GetBusinessUnitRequest()
            {
                UserBusinessUnitId = Guid.NewGuid(),
                IncludeChildren = true
            };
            BusinessUnitCompanyIdFilter filter = new BusinessUnitCompanyIdFilter();

            bool canApply = filter.CanApplyFilter(request);

            Assert.IsFalse(canApply);
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
            Guid companyId = Guid.NewGuid();
            var inputAccounts = new List<AccountContract>()
            {
                new AccountContract()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId
                }
            };
            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CompanyId = Guid.NewGuid()
            });
            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CompanyId = companyId
            });
            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CompanyId = companyId
            });
            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CompanyId = companyId
            });
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = companyId,
                IncludeChildren = true
            };
            var filterUnderTest = new BusinessUnitCompanyIdFilter();

            var result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(inputAccounts.Count - 1, result.Count());
        }
    }
}
