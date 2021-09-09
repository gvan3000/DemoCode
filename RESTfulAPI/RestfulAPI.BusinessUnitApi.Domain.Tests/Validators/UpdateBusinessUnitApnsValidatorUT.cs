using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Validators.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class UpdateBusinessUnitApnsValidatorUT
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateModel_ShouldThrowWhenInputIsNull()
        {
            var input = default(UpdateBusinessUnitApnsModel);

            var available = new ApnSetWithDetailsContract[] { };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenDefaultIsSetButApnsAreEmpty()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>()
            };

            var available = new ApnSetWithDetailsContract[] { };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "DefaultApn");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "can't be set");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenDefaultisNotInApnList()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = Guid.NewGuid(),
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var available = new ApnSetWithDetailsContract[] { };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "DefaultApn");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "must be in list");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenDuplicatesAreFoundInApnList()
        {
            var defaultApn = Guid.NewGuid();
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = defaultApn,
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = defaultApn } }
            };

            var available = new ApnSetWithDetailsContract[] { };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "Apns");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "uplicate");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenApnsAreDefinedButThereAreNoCompanyLevelApnsAvailable()
        {
            var defaultApn = Guid.NewGuid();
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = defaultApn,
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var available = new ApnSetWithDetailsContract[] { };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "Apns");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "No company level");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenApnsHaveEntriesNotInCompanyLevelApnList()
        {
            var defaultApn = Guid.NewGuid();
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = defaultApn,
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var available = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id = Guid.NewGuid(), ApnSetDetailId = Guid.NewGuid() }
                    }
                },
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id= Guid.NewGuid(), ApnSetDetailId = Guid.NewGuid() }
                    }
                }
            };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "Apns");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "list contains entries not defined for company");
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnSuccessWhenInputIsValid()
        {
            var defaultApn = Guid.NewGuid();
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = defaultApn,
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = defaultApn }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var available = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id = Guid.NewGuid(), ApnSetDetailId = input.Apns[0].Id }
                    }
                },
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id = Guid.NewGuid(), ApnSetDetailId = input.Apns[1].Id }
                    }
                }
            };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void ValidateModel_ShouldReturnValidationErrorWhenDefaultIsNullButApnsListHasItems()
        {
            var input = new UpdateBusinessUnitApnsModel()
            {
                DefaultApn = null,
                Apns = new List<APNRequestDetail>() { new APNRequestDetail() { Id = Guid.NewGuid() }, new APNRequestDetail() { Id = Guid.NewGuid() } }
            };

            var available = new ApnSetWithDetailsContract[]
            {
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id = Guid.NewGuid(), ApnSetDetailId = input.Apns[0].Id }
                    }
                },
                new ApnSetWithDetailsContract()
                {
                    ApnSetDetails = new ApnDetailContract[]
                    {
                        new ApnDetailContract() { Id = Guid.NewGuid(), ApnSetDetailId = input.Apns[1].Id }
                    }
                }
            };

            var validatorUnderTest = new UpdateBusinessUnitApnsValidator();

            var result = validatorUnderTest.ValidateModel(input, available);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.ErrorMessageTarget);
            StringAssert.Contains(result.ErrorMessageTarget, "DefaultApn");
            Assert.IsNotNull(result.ErrorMessage);
            StringAssert.Contains(result.ErrorMessage, "must not be empty");
        }
    }
}
