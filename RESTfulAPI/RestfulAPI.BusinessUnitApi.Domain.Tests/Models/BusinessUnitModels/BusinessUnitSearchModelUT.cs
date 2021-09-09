using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Models.BusinessUnitModels
{
    [TestClass]
    public class BusinessUnitSearchModelUT
    {
        [TestMethod]
        public void Validate_ShouldReturnFailureWhenNoSearchCriteriaSpecified()
        {
            var model = new BusinessUnitSearchModel();

            var validationResults = model.Validate(null).ToList();

            Assert.IsNotNull(validationResults);
            Assert.AreNotEqual(0, validationResults.Count);
            Assert.AreNotEqual(ValidationResult.Success, validationResults.First());
        }

        [TestMethod]
        public void Validate_ShouldSucceedWhenOneSearchCriteriaIsSpecified()
        {
            var model = new BusinessUnitSearchModel()
            {
                CustomerId = "abc"
            };

            var validationResults = model.Validate(null).ToList();

            Assert.IsNotNull(validationResults);
            Assert.AreNotEqual(0, validationResults.Count);
            Assert.AreSame(ValidationResult.Success, validationResults.First());

            model.CustomerId = null;
            model.Name = "bla";

            validationResults = model.Validate(null).ToList();

            Assert.IsNotNull(validationResults);
            Assert.AreNotEqual(0, validationResults.Count);
            Assert.AreSame(ValidationResult.Success, validationResults.First());
        }

        [TestMethod]
        public void Validate_Sholdreturn_Failure_IfMoreThenOneSearchCriteriaSiSet()
        {
            var searchCriteria = new BusinessUnitSearchModel
            {
                CustomerId = "cust_123",
                HasSharedWallet = "true"
            };

            var resultValidation = searchCriteria.Validate(null).ToList();

            Assert.IsNotNull(resultValidation);
            Assert.AreNotSame(ValidationResult.Success, resultValidation.First());
        }

        [TestMethod]
        public void Validate_ShouldReturn_Failure_IfHasSharedWallet_InvalidValue()
        {
            var searchCriteria = new BusinessUnitSearchModel
            {
                HasSharedWallet = "true_invalid"
            };

            var resultValidation = searchCriteria.Validate(null).ToList();

            Assert.IsNotNull(resultValidation);
            Assert.AreNotSame(ValidationResult.Success, resultValidation.First());
        }

        [TestMethod]
        public void Validate_ShouldReturn_Success_IfHasSharedWalletIsSetValid_NotCaseSensitive()
        {
            List<ValidationResult> results = new List<ValidationResult>();
                
            List<BusinessUnitSearchModel> searchCriteriaList = new List<BusinessUnitSearchModel>
            {
                new BusinessUnitSearchModel { HasSharedWallet = "true" },
                new BusinessUnitSearchModel { HasSharedWallet = "false" },
                new BusinessUnitSearchModel { HasSharedWallet = "TRUE" },
                new BusinessUnitSearchModel { HasSharedWallet = "FALSE"},
                new BusinessUnitSearchModel { HasSharedWallet = "falSe"},
                new BusinessUnitSearchModel { HasSharedWallet = "trUE"}
            };            

            searchCriteriaList.ForEach(x => results.Add(x.Validate(null).First()));

            Assert.IsTrue(results.All(x => x == ValidationResult.Success));
            
        }

        [TestMethod]
        public void Validate_ShouldReturnSuccessIfEndUSerSubscriptionIsSpecifiedOnly()
        {
            List<BusinessUnitSearchModel> searchCriteriaList = new List<BusinessUnitSearchModel>
            {
                new BusinessUnitSearchModel { EndUserSubscription = "true" },
                new BusinessUnitSearchModel { EndUserSubscription = "false" },
                new BusinessUnitSearchModel { EndUserSubscription = "TRUE" },
                new BusinessUnitSearchModel { EndUserSubscription = "FALSE"},
                new BusinessUnitSearchModel { EndUserSubscription = "falSe"},
                new BusinessUnitSearchModel { EndUserSubscription = "trUE"}
            };

            var validationSResults = searchCriteriaList.SelectMany(x => x.Validate(null));

            Assert.IsTrue(validationSResults.All(x => x == ValidationResult.Success), "Not all validation results passed, need to inspect the resturned array andd etermin what specific case failed");
        }

        [TestMethod]
        public void Validate_ShouldNotPassWhenEndUserSubscriptionValueIsInvalid()
        {
            var searchCriteria = new BusinessUnitSearchModel
            {
                EndUserSubscription = "true_invalid"
            };

            var resultValidation = searchCriteria.Validate(null).ToList();

            Assert.IsNotNull(resultValidation);
            Assert.AreNotSame(ValidationResult.Success, resultValidation.First());
        }
    }
}
