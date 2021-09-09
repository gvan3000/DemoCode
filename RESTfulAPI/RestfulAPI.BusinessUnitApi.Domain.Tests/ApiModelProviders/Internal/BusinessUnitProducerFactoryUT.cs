using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.GetBusinessUnit
{
    [TestClass]
    public class BusinessUnitLoadingFactoryUT
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetLoader_ShouldThrowForNullArgument()
        {
            var loaderFactoryUnderTest = new BusinessUnitProducerFactory();
            var result = loaderFactoryUnderTest.GetLoader(null);
        }

        [TestMethod]
        public void GetLoader_ShouldPrioritizeParentBusinessUnitStrategyOverCompanyStrategyWhenInputContansUserAccountId()
        {
            var loaderUnderTest = new BusinessUnitProducerFactory();
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid(),
                UserBusinessUnitId = Guid.NewGuid(),
            };

            var result = loaderUnderTest.GetLoader(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ParentBusinessUnitLoadingStrategy));
        }

        [TestMethod]
        public void GetLoader_ShouldFallbackToCompanyStrategyWhenNoUserAccountId()
        {
            var loaderUnderTest = new BusinessUnitProducerFactory();
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.NewGuid(),
                UserBusinessUnitId = null,
            };

            var result = loaderUnderTest.GetLoader(request);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(CompanyIdBusinessUnitLoadingStrategy));
        }

        [TestMethod]
        public void GetLoader_ShouldReturnNullWhenNoSuitableLoaderFound()
        {
            var loaderUnderTest = new BusinessUnitProducerFactory();
            var request = new GetBusinessUnitRequest()
            {
                UserCompanyId = Guid.Empty,
                UserBusinessUnitId = null,
            };

            var result = loaderUnderTest.GetLoader(request);

            Assert.IsNull(result);
        }
    }
}
