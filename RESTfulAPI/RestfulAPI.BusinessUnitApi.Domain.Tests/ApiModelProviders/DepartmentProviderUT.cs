using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using Moq;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using System.Net;
using RestfulAPI.Logging;
using System.ServiceModel;
using RestfulAPI.Common;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    /// <summary>
    /// DepartmentProvider Unit Tests
    /// </summary>
    [TestClass]
    public class DepartmentProviderUT
    {
        DepartmentProvider _providerUnderTest;

        Mock<ITeleenaServiceUnitOfWork> _serviceUnitOfWorkMock;
        Mock<IBusinessUnitApiTranslators> _translatorsMock;
        Mock<IJsonRestApiLogger> loggerMock;

        [TestInitialize]
        public void SetUp()
        {
            _serviceUnitOfWorkMock = new Mock<ITeleenaServiceUnitOfWork>();
            _serviceUnitOfWorkMock.Setup(x => x.DepartmentCostCenterService.AddDepartmentCostCenterAsync(It.IsAny<AddDepartmentCostCenterContract>()))
                .ReturnsAsync(new DepartmentCostCenterContract { AccountId = Guid.NewGuid(), CostCenterName = "c_name", DepartmentName = "d_name", Id = Guid.NewGuid() });

            _translatorsMock = new Mock<IBusinessUnitApiTranslators>();
            _translatorsMock.Setup(x => x.CreateDepartmentContractTranslator.Translate(It.IsAny<CreateDepartmentModel>(), It.IsAny<Guid>()))
                .Returns(new AddDepartmentCostCenterContract { AccountId = Guid.NewGuid(), CostCenterName = "c_name", DepartmentName = "d_name"});
            _translatorsMock.Setup(x => x.DepartmentModelTranslator.Translate(It.IsAny<DepartmentCostCenterContract>()))
                .Returns(new CreateDepartmentResponseModel { Id = Guid.NewGuid(), Location = "random/location"});
            _translatorsMock.Setup(x => x.UpdateDepartmentContractTranslator.Translate(It.IsAny<UpdateDepartmentModel>()))
                .Returns(new UpdateDepartmentCostCenterContract { Id = Guid.NewGuid(), CostCenterName = "c_name", DepartmentName = "d_name" });

            loggerMock = new Mock<IJsonRestApiLogger>();

            _providerUnderTest = new DepartmentProvider(_serviceUnitOfWorkMock.Object, _translatorsMock.Object, loggerMock.Object);
        }

        [TestMethod]
        public void CreateAsync_ShouldCall_CreateDepartmentContractTranslator_Translate()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _translatorsMock.Verify(x => x.CreateDepartmentContractTranslator.Translate(It.IsAny<CreateDepartmentModel>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void CreateAsync_ShouldCall_DepartmentCostCenter_AddDepartmentCostCenterAsync()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _serviceUnitOfWorkMock.Verify(x => x.DepartmentCostCenterService.AddDepartmentCostCenterAsync(It.IsAny<AddDepartmentCostCenterContract>()), Times.Once);
        }

        [TestMethod]
        public void CreateAsync_ShouldCall_DepartmentModelTranslator_Translate()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _translatorsMock.Verify(x => x.DepartmentModelTranslator.Translate(It.IsAny<DepartmentCostCenterContract>()), Times.Once);
        }

        [TestMethod]
        public void CreateAsync_ShouldReturn_OkResult()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.OK, providerResponse.HttpResponseCode);
        }

        [TestMethod]
        public void CreateAsync_ShouldCallLogger_WhenDepartmentCostCenterService_ThrowAnException()
        {
            _serviceUnitOfWorkMock.Setup(x => x.DepartmentCostCenterService.AddDepartmentCostCenterAsync(It.IsAny<AddDepartmentCostCenterContract>()))
                .ThrowsAsync(new Exception("error occurred"));

            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            loggerMock.Verify(x => x.LogException(It.IsAny<LogSeverity>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Exception>()));
        }

        [TestMethod]
        public void CreateAsync_ShouldReturn_InternalServerError_HttpStatus_WhenDepartmentCostCenterService_ThrowAnException()
        {
            _serviceUnitOfWorkMock.Setup(x => x.DepartmentCostCenterService.AddDepartmentCostCenterAsync(It.IsAny<AddDepartmentCostCenterContract>()))
                .ThrowsAsync(new Exception("error occurred"));

            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.InternalServerError, providerResponse.HttpResponseCode);
        }

        [TestMethod]
        public void CreateAsync_ShouldReturn_BadRequest_WhenDepartmentCostCenterService_Returns_TeleenaException_InvalidInputValue()
        {
            _serviceUnitOfWorkMock.Setup(x => x.DepartmentCostCenterService.AddDepartmentCostCenterAsync(It.IsAny<AddDepartmentCostCenterContract>()))
                .ThrowsAsync(new FaultException<TeleenaServiceReferences.DepartmentCostCenterService.TeleenaInnerExp>( new TeleenaServiceReferences.DepartmentCostCenterService.TeleenaInnerExp() {  ErrorCode = (int)ErrorCode.InvalidInputValue }));

            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_name" };

            var providerResponse = _providerUnderTest.CreateAsync(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, providerResponse.HttpResponseCode);
        }

        [TestMethod]
        public void UpdateDepartmentAsync_ShouldCall_UpdateDepartmentContractTranslator_Translate()
        {
            var input = new UpdateDepartmentModel { CostCenter = "c_name", Name = "d_name" };

            var providerResponse = _providerUnderTest.UpdateDepartmentAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            _translatorsMock.Verify(x => x.UpdateDepartmentContractTranslator.Translate(It.IsAny<UpdateDepartmentModel>()), Times.Once);
        }

        [TestMethod]
        public void UpdateDepartmentAsync_ShouldReturn_AcceptedResult()
        {
            var input = new UpdateDepartmentModel { CostCenter = "c_name", Name = "d_mame" };

            var providerResponse = _providerUnderTest.UpdateDepartmentAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.Accepted, providerResponse.HttpResponseCode);
        }

        [TestMethod]
        public void UpdateDepartmentAsync_ShouldReturn_BadRequest_WhenDepartmentCostCenterService_Returns_TeleenaException_InvalidInputValue()
        {
            _serviceUnitOfWorkMock.Setup(x => x.DepartmentCostCenterService.UpdateDepartmentCostCenterAsync(It.IsAny<UpdateDepartmentCostCenterContract>()))
                .ThrowsAsync(new FaultException<TeleenaServiceReferences.DepartmentCostCenterService.TeleenaInnerExp>(new TeleenaServiceReferences.DepartmentCostCenterService.TeleenaInnerExp() { ErrorCode = (int)ErrorCode.InvalidInputValue }));

            var input = new UpdateDepartmentModel { CostCenter = "c_name", Name = "d_name" };

            var providerResponse = _providerUnderTest.UpdateDepartmentAsync(input).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.AreEqual(HttpStatusCode.BadRequest, providerResponse.HttpResponseCode);
        }
    }
}
