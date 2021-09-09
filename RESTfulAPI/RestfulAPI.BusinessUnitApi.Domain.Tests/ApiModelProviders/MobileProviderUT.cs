using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.MobileService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class MobileProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> mockServiceUnitOfWork;
        private Mock<MobileService> mockMobileService;
        private Mock<IBusinessUnitApiTranslators> mockBusinessUnitApiTranslators;
        private Mock<ITranslate<MsisdnContract[], AvailableMSISDNResponseModel>> mockMsisdnContractTranslator;
        private Mock<ISysCodeConstants> mockSysCodeConstants;
        private Mock<IAvailableMsisdnFactory> mockAvailableMsisdnFactory;

        [TestInitialize]
        public void Setup()
        {
            mockMobileService = new Mock<MobileService>();
            mockMobileService.Setup(x => x.GetAvailableMsisdnsAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new MsisdnContract[] { });

            mockMobileService.Setup(x => x.GetAvailableMsisdnsWithStatusAsync(It.IsAny<GetAvailableMsisdnsWithStatusContract>()))
                .ReturnsAsync(new MsisdnContract[] { });

            mockMobileService.Setup(x => x.GetAvailableMsisdnsNoStatusAsync(It.IsAny<GetAvailableMsisdnsNoStatusContract>()))
                .ReturnsAsync(new MsisdnContract[] { });

            mockMsisdnContractTranslator = new Mock<ITranslate<MsisdnContract[], AvailableMSISDNResponseModel>>();
            mockMsisdnContractTranslator.Setup(x => x.Translate(It.IsAny<MsisdnContract[]>()))
                .Returns(new AvailableMSISDNResponseModel());

            mockBusinessUnitApiTranslators = new Mock<IBusinessUnitApiTranslators>();
            mockBusinessUnitApiTranslators.SetupGet(x => x.MsisdnContractTranslator).Returns(mockMsisdnContractTranslator.Object);

            mockServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>();
            mockServiceUnitOfWork.SetupGet(x => x.MobileService).Returns(mockMobileService.Object);

            mockSysCodeConstants = new Mock<ISysCodeConstants>();
            var dummyStatuses = new Dictionary<string, Guid>
            {
                { "AVAILABLE", Guid.NewGuid() },
                { "ACTIVATED", Guid.NewGuid() },
                { "LOCKED", Guid.NewGuid() }
            };
            mockSysCodeConstants.SetupGet(m => m.MobileStatuses).Returns(dummyStatuses);

            mockAvailableMsisdnFactory = new Mock<IAvailableMsisdnFactory>();
        }

        [TestMethod]
        public void GetAvailabeMsisdns_ShouldReturnResult()
        {
            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), MsisdnStatus = "Available" };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
        }


        [TestMethod]
        public void GetAvailabeMsisdns_ShoudCallAvailableMsisdnFactoryWithStatusParam()
        {
            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), MsisdnStatus = "Available" };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
            mockAvailableMsisdnFactory.Verify(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetAvailabeMsisdns_ShoudCallAvailableMsisdnFactoryWithQueryParam()
        {
            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);
            mockBusinessUnitApiTranslators.Setup(x => x.MsisdnContractTranslator).Returns(new MsisdnContractTranslator());

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), QueryMsisdnStatus = "Available" };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
            mockAvailableMsisdnFactory.Verify(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>()), Times.Once);
        }

        [TestMethod]
        public void GetAvailabeMsisdns_ShoudReturnNotFoundIfNoTranslatedResults()
        {
            mockMsisdnContractTranslator.Setup(x => x.Translate(It.IsAny<MsisdnContract[]>())).Returns(default(AvailableMSISDNResponseModel));

            mockAvailableMsisdnFactory.Setup(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>())).Returns(new WithStatusParamStrategy(mockSysCodeConstants.Object));

            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), QueryMsisdnStatus = "Available" };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
            mockAvailableMsisdnFactory.Verify(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>()), Times.Once);
            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpResponseCode);
        }

        [TestMethod]
        public void GetAvailabeMsisdns_ShoudCallMobileService2MethodsForStatusParam()
        {
            mockMsisdnContractTranslator.Setup(x => x.Translate(It.IsAny<MsisdnContract[]>())).Returns(default(AvailableMSISDNResponseModel));

            mockAvailableMsisdnFactory.Setup(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>())).Returns(new WithStatusParamStrategy(mockSysCodeConstants.Object));

            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), QueryMsisdnStatus = "Available" };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
            mockAvailableMsisdnFactory.Verify(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>()), Times.Once);
            mockMobileService.Verify(x => x.GetAvailableMsisdnsWithStatusAsync(It.IsAny<GetAvailableMsisdnsWithStatusContract>()), Times.Once);
        }

        [TestMethod]
        public void GetAvailabeMsisdns_ShoudCallMobileService1MethodsIfNoStatusParams()
        {
            mockMsisdnContractTranslator.Setup(x => x.Translate(It.IsAny<MsisdnContract[]>())).Returns(default(AvailableMSISDNResponseModel));

            mockAvailableMsisdnFactory.Setup(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>())).Returns(new NoStatusParamStrategy(mockSysCodeConstants.Object));

            var provider = new MobileProvider(mockServiceUnitOfWork.Object, mockBusinessUnitApiTranslators.Object, mockSysCodeConstants.Object, mockAvailableMsisdnFactory.Object);

            var request = new AvailableMsisdnProviderRequest { BusinessUnitId = Guid.NewGuid(), QueryMsisdnStatus = null, MsisdnStatus = null };

            var response = provider.GetAvailableMsisdnsAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableMSISDNResponseModel>));
            mockAvailableMsisdnFactory.Verify(x => x.GetStrategy(It.IsAny<AvailableMsisdnProviderRequest>()), Times.Once);
            mockMobileService.Verify(x => x.GetAvailableMsisdnsNoStatusAsync(It.IsAny<GetAvailableMsisdnsNoStatusContract>()), Times.Once);
            
        }
    }
}
