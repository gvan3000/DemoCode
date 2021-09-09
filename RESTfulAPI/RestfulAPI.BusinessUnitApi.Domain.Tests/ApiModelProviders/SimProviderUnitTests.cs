using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.SimServiceV2;
using System;
using System.Linq;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class SimProviderUnitTests
    {
        private Mock<ITeleenaServiceUnitOfWork> _serviceUnitOfWorkMock;
        private Mock<SimServiceV2> _simServiceMock;
        private Mock<IBusinessUnitApiTranslators> _businessUnitApiTranslatorsMock;
        private Mock<ITranslate<string, Guid>> _simStatusTranslatorMock;
        private Mock<ITranslate<Guid, SimStatusWrapper>> _simStatusGuidToStringTranslatorMock;
        private Guid _simStatustId;
        private AvailableSimProviderRequest _availableSimsProviderRequest;

        [TestInitialize]
        public void Setup()
        {
            _simServiceMock = new Mock<SimServiceV2>();
            _simServiceMock.Setup(x => x.GetAvailableSimsPaginatedAsync(It.IsAny<GetAvailableSimsPaginatedContract>()))
                .ReturnsAsync(new AvailableSimsPaginatedResponseContract
                {
                    Sims = new TeleenaServiceReferences.SimServiceV2.Sim[]
                    {
                        new TeleenaServiceReferences.SimServiceV2.Sim { Iccid = "sim1" },
                        new TeleenaServiceReferences.SimServiceV2.Sim { Iccid = "sim2" }
                    },
                    TotalResults = 5
                });

            _serviceUnitOfWorkMock = new Mock<ITeleenaServiceUnitOfWork>();
            _serviceUnitOfWorkMock.SetupGet(x => x.SimService).Returns(_simServiceMock.Object);

            _simStatustId = Guid.NewGuid();
            _simStatusTranslatorMock = new Mock<ITranslate<string, Guid>>();
            _simStatusTranslatorMock.Setup(x => x.Translate(It.IsAny<string>())).Returns(_simStatustId);

            _simStatusGuidToStringTranslatorMock = new Mock<ITranslate<Guid, SimStatusWrapper>>();
            _simStatusGuidToStringTranslatorMock.Setup(x => x.Translate(It.IsAny<Guid>())).Returns(new SimStatusWrapper());

            _businessUnitApiTranslatorsMock = new Mock<IBusinessUnitApiTranslators>();
            _businessUnitApiTranslatorsMock.SetupGet(x => x.SimStatusTranslator).Returns(_simStatusTranslatorMock.Object);
            _businessUnitApiTranslatorsMock.SetupGet(x => x.SimStatusGuidToStringTranslator).Returns(_simStatusGuidToStringTranslatorMock.Object);

            _availableSimsProviderRequest = new AvailableSimProviderRequest
            {
                AccountId = Guid.NewGuid(),
                Status = "status",
                PerPage = 13,
                Page = 25
            };
        }

        [TestMethod]
        public void GetAvailableSIMs_ShouldReturnResult()
        {
            // Arrange
            var provider = new SimProvider(_serviceUnitOfWorkMock.Object, _businessUnitApiTranslatorsMock.Object);

            // Act
            var response = provider.GetAvailableSIMsAsync(Guid.NewGuid(), string.Empty).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableSIMResponseModel>));
        }

        [TestMethod]
        public void GetAvailableSIMs_ShouldReturnNotFound()
        {
            // Arrange
            var provider = new SimProvider(_serviceUnitOfWorkMock.Object, _businessUnitApiTranslatorsMock.Object);

            // Act
            var response = provider.GetAvailableSIMsAsync(Guid.NewGuid(), string.Empty).ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(response.HttpResponseCode, System.Net.HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void GetAvailableSIMs_ForValidAvailableSIMsProviderRequest_ShouldReturnResult()
        {
            // Arrange
            var provider = new SimProvider(_serviceUnitOfWorkMock.Object, _businessUnitApiTranslatorsMock.Object);
            var availableSiMsProviderRequest = new AvailableSimProviderRequest
            {
                AccountId = Guid.NewGuid(),
                Status = string.Empty,
                PerPage = 1,
                Page = 1
            };

            // Act
            var response = provider.GetAvailableSIMsAsync(availableSiMsProviderRequest)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableSimResponseV2Model>));
        }

        [TestMethod]
        public void GetAvailableSIMs_ForValidAvailableSIMsProviderRequest_ReturnsResponseFromSimService()
        {
            // Arrange
            var provider = new SimProvider(_serviceUnitOfWorkMock.Object, _businessUnitApiTranslatorsMock.Object);

            // Act
            var response = provider.GetAvailableSIMsAsync(_availableSimsProviderRequest)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsTrue(response.Result.Sims.Any(sim => sim.Iccid == "sim1"));
            Assert.IsTrue(response.Result.Sims.Any(sim => sim.Iccid == "sim2"));
            Assert.AreEqual(5, response.Result.Paging.TotalResults);
        }

        [TestMethod]
        public void GetAvailableSIMs_ForSimServiceNullResponse_ReturnsNotFoundResult()
        {
            // Arrange
            var provider = new SimProvider(_serviceUnitOfWorkMock.Object, _businessUnitApiTranslatorsMock.Object);
            _simServiceMock
                .Setup(x => x.GetAvailableSimsPaginatedAsync(It.IsAny<GetAvailableSimsPaginatedContract>()))
                .ReturnsAsync((AvailableSimsPaginatedResponseContract)null);

            // Act
            var response = provider.GetAvailableSIMsAsync(_availableSimsProviderRequest)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            // Assert
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<AvailableSimResponseV2Model>));
            Assert.AreEqual("No sims were found for your request.", response.ErrorMessage);
            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpResponseCode);
        }
    }
}
