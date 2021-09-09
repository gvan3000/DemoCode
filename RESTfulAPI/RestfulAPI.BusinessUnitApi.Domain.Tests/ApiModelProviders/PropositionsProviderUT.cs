using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class PropositionsProviderUT
    {
        private Mock<ITeleenaServiceUnitOfWork> serviceUnitOfWorkMocked;
        private Mock<IBusinessUnitApiTranslators> businessUnitApiTranslatorsMocked;
        private Mock<IPropositionContractTranslator> propositionsContractTranslator;
        private Mock<IServiceTypeConfigurationProvider> mockConfig;

        [TestInitialize]
        public void Setup()
        {
            serviceUnitOfWorkMocked = new Mock<ITeleenaServiceUnitOfWork>();
            serviceUnitOfWorkMocked.Setup(x => x.PropositionService.GetActivePropositionsByBusinessUnitAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new PropositionsContract());

            propositionsContractTranslator = new Mock<IPropositionContractTranslator>();
            propositionsContractTranslator.Setup(x => x.Translate(It.IsAny<PropositionsContract>(), It.IsAny<PropositionsContract>()))
                .Returns(new PropositionsResponseModel());

            businessUnitApiTranslatorsMocked = new Mock<IBusinessUnitApiTranslators>();
            businessUnitApiTranslatorsMocked.SetupGet(p => p.PropositionsContractTranslator).Returns(propositionsContractTranslator.Object);

            mockConfig = new Mock<IServiceTypeConfigurationProvider>();
            mockConfig.Setup(x => x.CurrencyUnitNames).Returns(new Dictionary<string, string>() { { "$", "DOLLARS" }, { "€", "EUROS" } });
            mockConfig.Setup(x => x.DataCodes).Returns(new List<string>() { "KB", "MB", "GB", "TB" });
            mockConfig.Setup(x => x.GeneralCacheCodes).Returns(new List<string>() { "EUROS", "POUNDS", "DOLLARS", "ZLOTYCH", "NONE" });
            mockConfig.Setup(x => x.SmsCodes).Returns(new List<string>() { "UNIT", "SMS", "MINUTESMS" });
            mockConfig.Setup(x => x.VoiceCodes).Returns(new List<string>() { "SECOND", "MINUTE", "HOUR", "DAY", "WEEK", "MONTH", "YEAR" });
            mockConfig.Setup(x => x.VoiceUnitNames).Returns(new Dictionary<string, string>() { { "min", "MINUTE" }, { "h", "HOUR" }, { "d", "DAY" } });
        }

        [TestMethod]
        public void GetPropositionsByBusinessUnitIdAsync_ShouldReturnPropositionsResponseModel()
        {
            var provider = new PropositionsProvider(serviceUnitOfWorkMocked.Object, businessUnitApiTranslatorsMocked.Object);

            var response = provider.GetPropositionsAsync(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(PropositionsResponseModel));
        }

        [TestMethod]
        public void GetPropositionsByBusinessUnitIdAsync_ShouldUseAppropriateMethodsFromPropositionService()
        {
            var requestedBU = Guid.NewGuid();
            var provider = new PropositionsProvider(serviceUnitOfWorkMocked.Object, businessUnitApiTranslatorsMocked.Object);

            var response = provider.GetPropositionsAsync(requestedBU).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            serviceUnitOfWorkMocked.Verify(x => x.PropositionService.GetActivePropositionsByBusinessUnitAsync(It.Is<Guid>(id => id == requestedBU)), Times.Once);
            serviceUnitOfWorkMocked.Verify(x => x.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(It.Is<Guid>(id => id == requestedBU)), Times.Once);
        }
    }
}
