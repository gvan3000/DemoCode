using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.FilterBusinessUnit
{
    [TestClass]
    public class NoChildrenNameFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_ShouldReturnTrueWhenNameFilterIsSetRegardlessOfIncludeChildrenFilter()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "something"
            };

            var filterUnderText = new NoChildrenNameFilter();
            Assert.IsTrue(filterUnderText.CanApplyFilter(request));
            request.IncludeChildren = true;
            Assert.IsTrue(filterUnderText.CanApplyFilter(request));
        }

        [TestMethod]
        public void FilterBusinessUnitsByName_MixedUpperAndLowerCaseRequest_ShouldFilterCaseInsensitive()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "SoMetHinG"
            };

            var listToSearch = new List<AccountContract>()
            {
                new AccountContract() { UserName = "something" }
            };

            var filterUnderText = new NoChildrenNameFilter();

            var response = filterUnderText.FilterBusinessUnitsByRequest(listToSearch, request);

            CollectionAssert.Contains(response, listToSearch[0]);
        }

        [TestMethod]
        public void FilterBusinessUnitsByName_LowerCaseRequest_ShouldFilterCaseInsensitive()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "something"
            };

            var listToSearch = new List<AccountContract>()
            {
                new AccountContract() { UserName = "something" }
            };

            var filterUnderText = new NoChildrenNameFilter();

            var response = filterUnderText.FilterBusinessUnitsByRequest(listToSearch, request);

            CollectionAssert.Contains(response, listToSearch[0]);
        }

        [TestMethod]
        public void FilterBusinessUnitsByName_UppeerCaseRequest_ShouldFilterCaseInsensitive()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "SOMETHING"
            };

            var listToSearch = new List<AccountContract>()
            {
                new AccountContract() { UserName = "something" }
            };

            var filterUnderText = new NoChildrenNameFilter();

            var response = filterUnderText.FilterBusinessUnitsByRequest(listToSearch, request);

            CollectionAssert.Contains(response, listToSearch[0]);
        }

        [TestMethod]
        public void FilterBusinessUnitsByName_MixedCaseRequestAndInput_ShouldFilterCaseInsensitive()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "SOmeTHinG"
            };

            var listToSearch = new List<AccountContract>()
            {
                new AccountContract() { UserName = "soMEthINg" }
            };

            var filterUnderText = new NoChildrenNameFilter();

            var response = filterUnderText.FilterBusinessUnitsByRequest(listToSearch, request);

            CollectionAssert.Contains(response, listToSearch[0]);
        }

        [TestMethod]
        public void FilterBusinessUnitsByName_MixedUpperAndLowerCaseRequest_ShouldFilterCaseInsensitive_AndReturnMultipleResults()
        {
            var request = new GetBusinessUnitRequest()
            {
                IncludeChildren = false,
                FilterBusinessUnitName = "sOmeThinG"
            };

            var listToSearch = new List<AccountContract>()
            {
                new AccountContract() { UserName = "soMEthINg" },
                new AccountContract() { UserName = "Homer" },
                new AccountContract() { UserName = "SometHING" },
                new AccountContract() { UserName = "something" },
                new AccountContract() { UserName = "SOMETHING" },
                new AccountContract() { UserName = "Somethin" }
            };

            var filterUnderText = new NoChildrenNameFilter();

            var response = filterUnderText.FilterBusinessUnitsByRequest(listToSearch, request);

            Assert.AreEqual(response.Count, 4);
            CollectionAssert.Contains(response, listToSearch[0]);
            CollectionAssert.Contains(response, listToSearch[2]);
            CollectionAssert.Contains(response, listToSearch[3]);
            CollectionAssert.Contains(response, listToSearch[4]);
        }
    }
}
