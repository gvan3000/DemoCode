using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.FilterBusinessUnit
{
    [TestClass]
    public class NoChildrenBusinessUnitIdFilterUT
    {
        [TestMethod]
        public void CanApplyFilter_ShouldSupportOnlyRequestWithIdSetAndIncludeChildrenNotSet()
        {
            var request = new GetBusinessUnitRequest()
            {
                FilterBusinessUnitId = Guid.NewGuid(),
                IncludeChildren = false
            };

            var filterUnderTest = new NoChildrenBusinessUnitIdFilter();
            Assert.IsTrue(filterUnderTest.CanApplyFilter(request));
            request.IncludeChildren = true;
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
            request.FilterBusinessUnitId = null;
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
            request.IncludeChildren = false;
            Assert.IsFalse(filterUnderTest.CanApplyFilter(request));
        }
    }
}
