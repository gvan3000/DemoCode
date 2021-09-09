using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Validators.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class UpdateBusinessUnitDefaultApnValidatorUT
    {
        [TestMethod]
        public void ValidateModel_ShouldReturnErrorWhenThereAreNoBusinessUnitApns()
        {
            var attemptedDefult = new UpdateBusinessUnitDefaultApnModel() { Id = Guid.NewGuid() };
            var businessUnitApns = new ApnDetailContract[]
            {
            };

            var validatorUnderTest = new UpdateBusinessUnitDefaultApnValidator();

            var validationResult = validatorUnderTest.ValidateModel(attemptedDefult, businessUnitApns);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "No business unit APNs");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnErrorWhenAttemptedDefaultApnIsNotInListOfBusinessUnitApns()
        {
            var attemptedDefult = new UpdateBusinessUnitDefaultApnModel() { Id = Guid.NewGuid() };
            var businessUnitApns = new ApnDetailContract[]
            {
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = "bla",
                    ApnSetId = Guid.NewGuid(),
                    IsDefault = true
                },
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = "bla2",
                    ApnSetId = Guid.NewGuid(),
                    IsDefault = false
                }
            };

            var validatorUnderTest = new UpdateBusinessUnitDefaultApnValidator();

            var validationResult = validatorUnderTest.ValidateModel(attemptedDefult, businessUnitApns);

            Assert.IsNotNull(validationResult);
            Assert.IsFalse(validationResult.IsSuccess);
            StringAssert.Contains(validationResult.ErrorMessage, "Invalid default APN");
        }

        [TestMethod]
        public void ValidateModel_ShouldSucceedForValidInput()
        {
            var attemptedDefult = new UpdateBusinessUnitDefaultApnModel() { Id = Guid.NewGuid() };
            var businessUnitApns = new ApnDetailContract[]
            {
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = attemptedDefult.Id,
                    Name = "bla",
                    ApnSetId = Guid.NewGuid(),
                    IsDefault = true
                },
                new ApnDetailContract()
                {
                    Id = Guid.NewGuid(),
                    ApnSetDetailId = Guid.NewGuid(),
                    Name = "bla2",
                    ApnSetId = Guid.NewGuid(),
                    IsDefault = false
                }
            };

            var validatorUnderTest = new UpdateBusinessUnitDefaultApnValidator();

            var validationResult = validatorUnderTest.ValidateModel(attemptedDefult, businessUnitApns);

            Assert.IsNotNull(validationResult);
            Assert.IsTrue(validationResult.IsSuccess);
        }
    }
}
