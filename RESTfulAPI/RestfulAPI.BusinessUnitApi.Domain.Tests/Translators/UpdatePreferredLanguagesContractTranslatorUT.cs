using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.PreferredLanguage;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class UpdatePreferredLanguagesContractTranslatorUT
    {
        private Guid _businessUnitId;
        private Guid _languageId;
        private bool _languageIsDefault;
        private string _languageName;

        private UpdateAccountLanguagesContract _updateAccountLanguageContract;
        private UpdatePreferredLanguagesRequestModel _updatePreferredLanguageRequestModel;
        private AccountLanguageContract _accountLanguageModel;
        private List<UpdatePreferredLanguageModel> _updatePreferredLanguageList;

        private UpdatePreferredLanguagesContractTranslator _translatorUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SetupVariables();
            SetupObject();
            SetupSUT();
        }

        private void SetupVariables()
        {
            _businessUnitId = Guid.NewGuid();
            _languageId = Guid.NewGuid();
            _languageIsDefault = true;
            _languageName = "English";
        }

        private void SetupObject()
        {
            _updatePreferredLanguageList = new List<UpdatePreferredLanguageModel>
            {
                new UpdatePreferredLanguageModel
                {
                    LanguageId = _languageId,
                    IsDefault = _languageIsDefault
                }
            };

            _updatePreferredLanguageRequestModel = new UpdatePreferredLanguagesRequestModel
            {
                PreferredLanguages = _updatePreferredLanguageList
            };
        }

        private void SetupSUT()
        {
            _translatorUnderTest = new UpdatePreferredLanguagesContractTranslator();
        }

        [TestMethod]
        public void Translate_ValidInput_ShouldReturnUpdateAccountLanguagesContract()
        {
            var result = _translatorUnderTest.Translate(_updatePreferredLanguageRequestModel);

            Assert.IsInstanceOfType(result, typeof(UpdateAccountLanguagesContract));
            Assert.IsTrue(result.AccountLanguages != null);
            Assert.IsTrue(result.AccountLanguages.ToList().Count > 0);
            var updateAccountLanguageContract = result.AccountLanguages[0];
            Assert.IsTrue(updateAccountLanguageContract != null);
            Assert.AreEqual(_languageId, updateAccountLanguageContract.LanguageId);
            Assert.AreEqual(_languageIsDefault, updateAccountLanguageContract.IsDefault);
        }

        [TestMethod]
        public void Translate_InvalidInputNull_ShouldReturnNull()
        {
            _updatePreferredLanguageRequestModel = null;

            var result = _translatorUnderTest.Translate(_updatePreferredLanguageRequestModel);

            Assert.IsTrue(result == null);
        }
    }
}
