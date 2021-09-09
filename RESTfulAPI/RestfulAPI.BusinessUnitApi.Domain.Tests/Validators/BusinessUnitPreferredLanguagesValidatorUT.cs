using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class BusinessUnitPreferredLanguagesValidatorUT
    {
        private Guid _languageId;
        private bool _isDefault;

        private UpdatePreferredLanguagesRequestModel _input;
        private List<UpdatePreferredLanguageModel> _preferredLanguagesList;
        private UpdatePreferredLanguageModel _updatePreferredLanguage;

        private BusinessUnitPreferredLanguagesValidator _validatorUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SetupVariables();
            SetupObjects();
            SetupSUT();
        }

        private void SetupVariables()
        {
            _languageId = Guid.NewGuid();
            _isDefault = true;
        }

        private void SetupObjects()
        {
            _updatePreferredLanguage = new UpdatePreferredLanguageModel
            {
                LanguageId = _languageId,
                IsDefault = _isDefault
            };

           _preferredLanguagesList = new List<UpdatePreferredLanguageModel> { _updatePreferredLanguage };

           _input = new UpdatePreferredLanguagesRequestModel
           {
               PreferredLanguages = _preferredLanguagesList
           };
        }

        private void SetupSUT()
        {
            _validatorUnderTest = new BusinessUnitPreferredLanguagesValidator();
        }

        [TestMethod]
        public void ValidateModel_ValidInput_ShouldReturnOkResult()
        {
            var result = _validatorUnderTest.ValidateModel(_input);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.HttpResponseCode==HttpStatusCode.OK);
        }

        [TestMethod]
        public void ValidateModel_InputIsNull_ShouldReturnInvalidInputResult()
        {
            _input = null;

            var result = _validatorUnderTest.ValidateModel(_input);

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.ErrorMessage == "The preferred languages list is null.");
            Assert.IsTrue(result.HttpResponseCode==HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ValidateModel_InputPreferredLanguagesIsNull_ShouldReturnInvalidInputResult()
        {
            _input.PreferredLanguages = null;

            var result = _validatorUnderTest.ValidateModel(_input);

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.ErrorMessage == "The preferred languages list is empty.");
            Assert.IsTrue(result.HttpResponseCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ValidateModel_InputPreferredLanguagesIsEmptyList_ShouldReturnInvalidInputResult()
        {
            _preferredLanguagesList = new List<UpdatePreferredLanguageModel>();
            _input.PreferredLanguages = _preferredLanguagesList;

            var result = _validatorUnderTest.ValidateModel(_input);

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.ErrorMessage == "The preferred languages list is empty.");
            Assert.IsTrue(result.HttpResponseCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ValidateModel_InputPreferredLanguageIdIsGuidEmpty_ShouldReturnInvalidInputResult()
        {
            _preferredLanguagesList[0].LanguageId = Guid.Empty;
            _input.PreferredLanguages = _preferredLanguagesList;

            var result = _validatorUnderTest.ValidateModel(_input);

            Assert.IsNotNull(result);
            Assert.IsTrue(!result.IsSuccess);
            Assert.IsTrue(result.ErrorMessage == "The preferred language Id is GUID empty.");
            Assert.IsTrue(result.HttpResponseCode == HttpStatusCode.BadRequest);
        }
    }
}
