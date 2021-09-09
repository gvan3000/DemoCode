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
    public class BusinessUnitCustomerIdFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_ShouldReturnTrueWhenCustomerIdIsSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterCustomerId = "abc",
                IncludeChildren = true
            };

            var filterUnderTest = new BusinessUnitCustomerIdFilter();
            Assert.IsTrue(filterUnderTest.CanApplyFilter(request));
        }

        [TestMethod]
        public void CanApplyFilter_ShouldReturnFalseWhenCustomerIdNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterCustomerId = null,
                IncludeChildren = true
            };

            var filterUnderTest = new BusinessUnitCustomerIdFilter();
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
        }

        [TestMethod]
        public void FilterBusinessUnitsByRequest_ShouldReturnOnlyBusinessUnitsWithGivenCustomerId()
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
                    Id = Guid.NewGuid(),
                    CustomerNumber = "first"
                }
            };

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CustomerNumber = "abc"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CustomerNumber = "1"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CustomerNumber = "2"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CustomerNumber = "3"
            });

            var request = new GetBusinessUnitRequest()
            {
                FilterCustomerId = inputAccounts[1].CustomerNumber
            };

            var filterUnderTest = new BusinessUnitCustomerIdFilter();

            var result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(inputAccounts[1].Id, result.First().Id);

            request.FilterCustomerId = "1";

            result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(inputAccounts[2].Id, result.First().Id);
        }

        [TestMethod]
        public void FilterBusinessUnitsByRequest_ShouldReturnBusinessUnitsNotMatchingGivenCustomerIdWhenIncludeCholdrenSet()
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
                    Id = Guid.NewGuid(),
                    CustomerNumber = "first"
                }
            };

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CustomerNumber = "abc"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[0].Id,
                CustomerNumber = "1"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CustomerNumber = "2"
            });

            inputAccounts.Add(new AccountContract()
            {
                Id = Guid.NewGuid(),
                ParentId = inputAccounts[1].Id,
                CustomerNumber = "3"
            });

            var request = new GetBusinessUnitRequest()
            {
                FilterCustomerId = inputAccounts[1].CustomerNumber,
                IncludeChildren = true
            };

            var filterUnderTest = new BusinessUnitCustomerIdFilter();

            var result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());

            request.FilterCustomerId = "1";

            result = filterUnderTest.FilterBusinessUnitsByRequest(inputAccounts.Reverse<AccountContract>().ToList(), request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }
    }
}
