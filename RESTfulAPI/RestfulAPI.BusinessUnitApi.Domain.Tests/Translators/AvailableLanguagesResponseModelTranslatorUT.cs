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
    public class AvailableLanguagesResponseModelTranslatorUT
    {
        private Guid _businessUnitId;
        private Guid _languageId;
        private bool _languageIsDefault;
        private string _languageName;

        private GetCompanyLanguageContract[] _companyLanguageContract;
        private AvailableLanguagesResponseModel _availableLanguageResponseModel;
        private AvailableLanguageModel _availableLanguageModel;
        private List<AvailableLanguageModel> _preferredLanguageList;

        private AvailableLanguagesResponseModelTranslator _translatorUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SetupVariables();
            SetupObjects();
            SetupSUT();
        }

        private void SetupVariables()
        {
            _businessUnitId = Guid.NewGuid();
            _languageId = Guid.NewGuid();
            _languageIsDefault = true;
            _languageName = "English";
        }

        private void SetupObjects()
        {
            _companyLanguageContract = new GetCompanyLanguageContract[]
            {
                new GetCompanyLanguageContract { LanguageId = _languageId, Name = _languageName }
            };
        }

        private void SetupSUT()
        {
            _translatorUnderTest = new AvailableLanguagesResponseModelTranslator();
        }

        [TestMethod]
        public void Translate_ValidInput_ShouldReturnAvailableLanguagesResponseModel()
        {
            var result = _translatorUnderTest.Translate(_companyLanguageContract);

            Assert.IsInstanceOfType(result, typeof(AvailableLanguagesResponseModel));
            Assert.IsTrue(result.AvailableLanguages != null);
            Assert.IsTrue(result.AvailableLanguages.Count > 0);
            var availableLanguageModel = result.AvailableLanguages[0];
            Assert.IsTrue(availableLanguageModel != null);
            Assert.AreEqual(_languageId, availableLanguageModel.LanguageId);
            Assert.AreEqual(_languageName, availableLanguageModel.Name);
        }

        [TestMethod]
        public void Translate_InvalidInputNull_ShouldReturnNull()
        {
            _companyLanguageContract = null;

            var result = _translatorUnderTest.Translate(_companyLanguageContract);

            Assert.IsTrue(result == null);
        }
    }
}
