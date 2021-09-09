using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.TeleenaServiceReferences;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders.Internal.GetAvailableMsisdns
{
    [TestClass]
    public class AvailableMsisdnFactoryUT
    {
        private Mock<ISysCodeConstants> mockSysCodeConstants;
        
        [TestInitialize]
        public void Setup()
        {
            mockSysCodeConstants = new Mock<ISysCodeConstants>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetStrategy_ShouldThrowForNullArgument()
        {
            var factoryUnderTest = new AvailableMsisdnFactory(mockSysCodeConstants.Object);
            var result = factoryUnderTest.GetStrategy(null);
        }

        [TestMethod]
        public void GetLoader_ShouldReturnNoStatusParamStrategy()
        {
            var factoryUnderTest = new AvailableMsisdnFactory(mockSysCodeConstants.Object);

            var providerRequest = new AvailableMsisdnProviderRequest() { BusinessUnitId = Guid.NewGuid(), MsisdnStatus = null, QueryMsisdnStatus = null };

            var result = factoryUnderTest.GetStrategy(providerRequest);

            Assert.IsInstanceOfType(result, typeof(NoStatusParamStrategy));
        }

        [TestMethod]
        public void GetLoader_ShouldReturnWithStatusParamStrategyIfMsisdnStatusPresent()
        {
            var factoryUnderTest = new AvailableMsisdnFactory(mockSysCodeConstants.Object);

            var providerRequest = new AvailableMsisdnProviderRequest() { BusinessUnitId = Guid.NewGuid(), MsisdnStatus = "available", QueryMsisdnStatus = null };

            var result = factoryUnderTest.GetStrategy(providerRequest);

            Assert.IsInstanceOfType(result, typeof(WithStatusParamStrategy));
        }

        [TestMethod]
        public void GetLoader_ShouldReturnWithStatusParamStrategyIfQueryMsisdnStatusPresent()
        {
            var factoryUnderTest = new AvailableMsisdnFactory(mockSysCodeConstants.Object);

            var providerRequest = new AvailableMsisdnProviderRequest() { BusinessUnitId = Guid.NewGuid(), MsisdnStatus = null, QueryMsisdnStatus = "available" };

            var result = factoryUnderTest.GetStrategy(providerRequest);

            Assert.IsInstanceOfType(result, typeof(WithStatusParamStrategy));
        }
    }
}
