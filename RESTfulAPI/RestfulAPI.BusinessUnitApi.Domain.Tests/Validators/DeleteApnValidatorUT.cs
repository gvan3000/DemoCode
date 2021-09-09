using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Validators.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Collections.Generic;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class DeleteApnValidatorUT
    {
        DeleteApnValidator validatorUnderTest;
        List<ApnDetailContract> apns;
        Guid defaultApnName;
        Guid apnRandomNotExist;
        Guid apnNameExist;

        [TestInitialize]
        public void SetUp()
        {
            validatorUnderTest = new DeleteApnValidator();

            apnNameExist = Guid.NewGuid();
            defaultApnName = Guid.NewGuid();
            apns = new List<ApnDetailContract>
            {
                new ApnDetailContract { IsDefault = true, Name = "internettest.mnc012.mcc345.gprs", Id = Guid.NewGuid(), ApnSetDetailId = defaultApnName },
                new ApnDetailContract { IsDefault = false, Name = "threetest.co.rs", Id = Guid.NewGuid(), ApnSetDetailId = apnNameExist },
                new ApnDetailContract { IsDefault = false, Name = "internettest.t-mobile", Id = Guid.NewGuid(), ApnSetDetailId = Guid.NewGuid() }
            };            

            apnRandomNotExist = Guid.NewGuid();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_ShouldThrow_ArgumentNullException_IfApns_IsNull()
        {
            List<ApnDetailContract> apns = null;
            var result = validatorUnderTest.Validate(apns, Guid.NewGuid());
        }

        [TestMethod]
        public void Validate_ShouldReturn_InternalServerError_If_Apns_IsEmptyList()
        {
            var apns = new List<ApnDetailContract>();

            var result = validatorUnderTest.Validate(apns, Guid.NewGuid());

            Assert.AreEqual(HttpStatusCode.InternalServerError, result.HttpResponseCode);
        }

        [TestMethod]
        public void Validate_ShouldReturn_ErrorMessage_If_Apns_IsEmptyList()
        {
            var apns = new List<ApnDetailContract>();

            var result = validatorUnderTest.Validate(apns, Guid.NewGuid());

            Assert.AreEqual("Could not find apns", result.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturn_BadRequest_If_ProvidedApn_NotExistsInApns()
        {
            var result = validatorUnderTest.Validate(apns, apnRandomNotExist);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);        
        }

        [TestMethod]
        public void Validate_ShouldReturn_ErrorMessage_If_ProvidedApn_NotExistsInApns()
        {
            var result = validatorUnderTest.Validate(apns, apnRandomNotExist);

            Assert.AreEqual($"Could not find apn with id: {apnRandomNotExist}", result.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturn_BadRequest_If_ProvidedApn_IsDefaultApn()
        {
            var result = validatorUnderTest.Validate(apns, defaultApnName);

            Assert.AreEqual(HttpStatusCode.BadRequest, result.HttpResponseCode);
        }

        [TestMethod]
        public void Validate_ShouldReturn_ErrorMessage_If_ProvidedApn_IsDefaultApn()
        {
            var resul = validatorUnderTest.Validate(apns, defaultApnName);

            Assert.AreEqual($"Default apn: {defaultApnName} cannot be removed", resul.ErrorMessage);
        }

        [TestMethod]
        public void Validate_ShouldReturn_OkResult_If_Apn_Exists()
        {
            var result = validatorUnderTest.Validate(apns, apnNameExist);

            Assert.AreEqual(HttpStatusCode.OK, result.HttpResponseCode);
        }
    }
}
