using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class PreferredLanguageProviderUT
    {
        private Guid _businessUnitId;
        private Guid _companyId;
        private Guid _languageId;
        private bool _languageIsDefault;
        private string _languageName;

        private GetAccountLanguageContract[] _accountLanguageContract;
        private PreferredLanguageResponseModel _preferredLanguageResponseModel;
        private PreferredLanguageModel _preferredLanguageModel;
        private GetCompanyLanguageContract[] _companyLanguageContract;
        private AvailableLanguagesResponseModel _availableLanguagesResponseModel;
        private AvailableLanguageModel _availableLanguageModel;
        private UpdateAccountLanguagesContract _updateAccountLanguagesContract;
        private UpdatePreferredLanguagesRequestModel _updatePreferredLanguagesRequestModel;
        private UpdatePreferredLanguageModel _updatePreferredLanguageModel;
        private List<PreferredLanguageModel> _preferredLanguageList;
        private Mock<IJsonRestApiLogger> _mockLogger;

        private Mock<ITeleenaServiceUnitOfWork> _mockTeleenaServiceUnitOfWork;
        private Mock<IBusinessUnitApiTranslators> _mockBusinessUnitApiTranslators;
        private Mock<IBusinessUnitValidators> _mockBusinessUnitValidators;

        private IPreferredLanguageProvider _providerUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SetupVariables();
            SetupObjects();
            SetupMocks();
            SetupSUT();
        }

        private void SetupVariables()
        {
            _businessUnitId = Guid.NewGuid();
            _companyId = Guid.NewGuid();
            _languageId = Guid.NewGuid();
            _languageIsDefault = true;
            _languageName = "English";
        }

        private void SetupObjects()
        {
            _accountLanguageContract = new GetAccountLanguageContract[]
            {
                new GetAccountLanguageContract
                    {LanguageId = _languageId, Name = _languageName, IsDefault = _languageIsDefault}
            };

            _preferredLanguageModel = new PreferredLanguageModel
            {
                LanguageId = _languageId,
                Name = _languageName,
                IsDefault = _languageIsDefault
            };

            _preferredLanguageList = new List<PreferredLanguageModel> { _preferredLanguageModel };

            _preferredLanguageResponseModel = new PreferredLanguageResponseModel
            {
                PreferredLanguages = _preferredLanguageList
            };

            _companyLanguageContract = new GetCompanyLanguageContract[]
            {
                new GetCompanyLanguageContract { LanguageId = _languageId, Name = _languageName }
            };

            _availableLanguageModel = new AvailableLanguageModel
            {
                 LanguageId = _languageId,
                 Name = _languageName
            };

            _availableLanguagesResponseModel = new AvailableLanguagesResponseModel
            {
                AvailableLanguages = new List<AvailableLanguageModel> { _availableLanguageModel }
            };

            _updateAccountLanguagesContract = new UpdateAccountLanguagesContract
            {
                AccountId = _businessUnitId,
                AccountLanguages = new AccountLanguageContract[]
                    {new AccountLanguageContract {LanguageId = _languageId, IsDefault = true}}
            };

            _updatePreferredLanguageModel = new UpdatePreferredLanguageModel
            {
                LanguageId = _languageId,
                IsDefault = true
            };

            _updatePreferredLanguagesRequestModel = new UpdatePreferredLanguagesRequestModel
            {
                PreferredLanguages = new List<UpdatePreferredLanguageModel> {_updatePreferredLanguageModel}
            };
        }

        private void SetupMocks()
        {
            _mockLogger = new Mock<IJsonRestApiLogger>();

            _mockTeleenaServiceUnitOfWork = new Mock<ITeleenaServiceUnitOfWork>(MockBehavior.Strict);
            _mockTeleenaServiceUnitOfWork
                .Setup(s => s.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_accountLanguageContract);
            _mockTeleenaServiceUnitOfWork
                .Setup(s => s.PreferredLanguageService.GetAvailableAccountLanguagesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_companyLanguageContract);
            _mockTeleenaServiceUnitOfWork
                .Setup(s=>s.PreferredLanguageService.UpdateAccountLanguagesAsync(It.IsAny<UpdateAccountLanguagesContract>()))
                .Verifiable();

            _mockBusinessUnitApiTranslators = new Mock<IBusinessUnitApiTranslators>(MockBehavior.Strict);
            _mockBusinessUnitApiTranslators
                .Setup(t => t.PreferredLanguageModelTranslator.Translate(_accountLanguageContract))
                .Returns(_preferredLanguageResponseModel);
            _mockBusinessUnitApiTranslators
                .Setup(t => t.AvailableLanguagesModelTranslator.Translate(_companyLanguageContract))
                .Returns(_availableLanguagesResponseModel);
            _mockBusinessUnitApiTranslators
                .Setup(t => t.UpdatePreferredLanguageContractTranslator.Translate(_updatePreferredLanguagesRequestModel))
                .Returns(_updateAccountLanguagesContract);
            
            _mockBusinessUnitValidators = new Mock<IBusinessUnitValidators>();
            _mockBusinessUnitValidators
                .Setup(v => v.UpdatePreferredLanguagesValidator.ValidateModel(
                    It.IsAny<UpdatePreferredLanguagesRequestModel>()))
                .Returns(ProviderOperationResult<object>.OkResult());
        }

        private void SetupSUT()
        {
            _providerUnderTest = new PreferredLanguageProvider(_mockLogger.Object, _mockTeleenaServiceUnitOfWork.Object, _mockBusinessUnitApiTranslators.Object, _mockBusinessUnitValidators.Object);
        }

        [TestMethod]
        public void GetPreferredLanguagesAsync_InputValid_ShouldReturnListOfPreferredLanguages()
        {
            var result = _providerUnderTest.GetAccountLanguagesAsync(_businessUnitId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(result, typeof(ProviderOperationResult<PreferredLanguageResponseModel>));
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.Result.PreferredLanguages!=null);
            Assert.IsTrue(result.Result.PreferredLanguages.Count>0);
            var resultLanguage = result.Result.PreferredLanguages[0];
            Assert.AreEqual(_languageId, resultLanguage.LanguageId);
            Assert.AreEqual(_languageName, resultLanguage.Name);
            Assert.AreEqual(_languageIsDefault, resultLanguage.IsDefault);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetPreferredLanguagesAsync_ServiceResponseNull_ShouldCallServiceMethodAndReturnGenerateFailureResult()
        {
            _accountLanguageContract = null;

            _mockTeleenaServiceUnitOfWork
                .Setup(m => m.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()))
                .ReturnsAsync(_accountLanguageContract);

            var result = _providerUnderTest.GetAccountLanguagesAsync(_businessUnitId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsTrue(result.ErrorMessage.Equals("Internal server error. Please contact administrator."));
            Assert.IsTrue(result.HttpResponseCode==HttpStatusCode.InternalServerError);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetPreferredLanguagesAsync_EmptyBusinessUnitId_ShouldThrowInvalidInputValue()
        {
            _businessUnitId = Guid.Empty;
            _accountLanguageContract = null;

            var exception = new TeleenaInnerExp
            {
                ErrorCode = 2,
                ErrorDescription = "AccountId is Guid.Empty.",
                TicketId = Guid.NewGuid()
            };

            _mockTeleenaServiceUnitOfWork
                .Setup(m => m.PreferredLanguageService.GetAccountLanguagesAsync(Guid.Empty))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(exception));


            var result = _providerUnderTest.GetAccountLanguagesAsync(_businessUnitId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(result, typeof(ProviderOperationResult<PreferredLanguageResponseModel>));
            Assert.IsTrue(result.IsSuccess==false);
            Assert.AreEqual("InvalidInputValue - AccountId is Guid.Empty.", result.ErrorMessage);
            Assert.AreEqual(null, result.Result);
            Assert.AreEqual("PreferredLanguages", result.ErrorMessageTarget);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetPreferredLanguagesAsync_PreferredLanguageIsNotEnabled_ShouldThrowInvalidStatus()
        {
            var exception = new TeleenaInnerExp
            {
                ErrorCode = 17,
                ErrorDescription = $"Preferred Language BU feature for accountId {_businessUnitId} is not enabled.",
                TicketId = Guid.NewGuid()
            };

            _mockTeleenaServiceUnitOfWork
                .Setup(m => m.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(exception));


            var result = _providerUnderTest.GetAccountLanguagesAsync(_businessUnitId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(result, typeof(ProviderOperationResult<PreferredLanguageResponseModel>));
            Assert.IsTrue(result.IsSuccess == false);
            Assert.AreEqual($"InvalidStatus - Preferred Language BU feature for accountId {_businessUnitId} is not enabled.", result.ErrorMessage);
            Assert.AreEqual(null, result.Result);
            Assert.AreEqual("PreferredLanguages", result.ErrorMessageTarget);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetPreferredLanguagesAsync_ThereIsNoPreferredLanguages_ShouldThrowInvalidStatus()
        {
            var exception = new TeleenaInnerExp
            {
                ErrorCode = 17,
                ErrorDescription = $"There is no Preferred Language BU feature for accountId {_businessUnitId}.",
                TicketId = Guid.NewGuid()
            };

            _mockTeleenaServiceUnitOfWork
                .Setup(m => m.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new FaultException<TeleenaInnerExp>(exception));


            var result = _providerUnderTest.GetAccountLanguagesAsync(_businessUnitId)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            Assert.IsInstanceOfType(result, typeof(ProviderOperationResult<PreferredLanguageResponseModel>));
            Assert.IsTrue(result.IsSuccess == false);
            Assert.AreEqual($"InvalidStatus - There is no Preferred Language BU feature for accountId {_businessUnitId}.", result.ErrorMessage);
            Assert.AreEqual(null, result.Result);
            Assert.AreEqual("PreferredLanguages", result.ErrorMessageTarget);
            _mockTeleenaServiceUnitOfWork.Verify(x => x.PreferredLanguageService.GetAccountLanguagesAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void GetAvailableLanguagesAsync_BuCompanyEnabledLanguagesFeature_ShouldReturnAvailableLanguagesList()
        {
            var result = _providerUnderTest.GetAvailableLanguagesAsync(_businessUnitId, _companyId)
                 .ConfigureAwait(false)
                 .GetAwaiter()
                 .GetResult();

            Assert.IsInstanceOfType(result, typeof(ProviderOperationResult<AvailableLanguagesResponseModel>));
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK,result.HttpResponseCode);
            Assert.AreEqual(_languageId, result.Result.AvailableLanguages[0].LanguageId);
            Assert.AreEqual(_languageName, result.Result.AvailableLanguages[0].Name);
        }
    }
}
