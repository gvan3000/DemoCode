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
    public class PreferredLanguageResponseModelTranslatorUT
    {
        private Guid _businessUnitId;
        private Guid _languageId;
        private bool _languageIsDefault;
        private string _languageName;

        private GetAccountLanguageContract[] _accountLanguageContract;
        private PreferredLanguageResponseModel _preferredLanguageResponseModel;
        private PreferredLanguageModel _preferredLanguageModel;
        private List<PreferredLanguageModel> _preferredLanguageList;

        private PreferredLanguageResponseModelTranslator _translatorUnderTest;

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
            _accountLanguageContract = new GetAccountLanguageContract[]
            {
                new GetAccountLanguageContract
                    {LanguageId = _languageId, Name = _languageName, IsDefault = _languageIsDefault}
            };
        }

        private void SetupSUT()
        {
            _translatorUnderTest = new PreferredLanguageResponseModelTranslator();
        }

        [TestMethod]
        public void Translate_ValidInput_ShouldReturnPreferredLanguageResponseModel()
        {
            var result = _translatorUnderTest.Translate(_accountLanguageContract);

            Assert.IsInstanceOfType(result, typeof(PreferredLanguageResponseModel));
            Assert.IsTrue(result.PreferredLanguages!=null);
            Assert.IsTrue(result.PreferredLanguages.Count>0);
            var preferredLanguageModel = result.PreferredLanguages[0];
            Assert.IsTrue(preferredLanguageModel!=null);
            Assert.AreEqual(_languageId, preferredLanguageModel.LanguageId);
            Assert.AreEqual(_languageName, preferredLanguageModel.Name);
            Assert.AreEqual(_languageIsDefault, preferredLanguageModel.IsDefault);
        }

        [TestMethod]
        public void Translate_InvalidInputNull_ShouldReturnNull()
        {
            _accountLanguageContract = null;

            var result = _translatorUnderTest.Translate(_accountLanguageContract);

            Assert.IsTrue(result == null);
        }
    }
}
