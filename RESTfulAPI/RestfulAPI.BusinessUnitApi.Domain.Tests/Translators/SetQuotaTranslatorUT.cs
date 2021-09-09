using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Quota;
using RestfulAPI.Constants;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;
using System.Security.Claims;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class SetQuotaTranslatorUT
    {
        [TestMethod]
        public void Translate_ShouldFillAllDataAccodringly()
        {
            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123,
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("email", "bla@test.com") }));
            var balanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "bla balance type"
            };

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);

            Assert.IsNotNull(result);
            Assert.AreEqual(input.Amount, result.Amount);
            Assert.AreEqual(accountId, result.AccountId);
            Assert.AreEqual(BalanceConstants.DELTA, result.BalanceMode);
            Assert.AreEqual(BalanceConstants.DAYS, result.BalanceExpiryUnit);
            Assert.AreEqual(BalanceConstants.BALANCE_EXPIRY, result.BalanceExpiry);
            Assert.AreEqual("bla@test.com", result.OriginatingUserName);
            Assert.IsFalse(result.NewBucket);
            Assert.AreEqual(balanceType.BalanceTypeName, result.BalanceType);
        }

        [TestMethod]
        public void Translate_ShouldAcceptStandardEmailClaimType()
        {
            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123,
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, "bla@test.com") }));
            var balanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "bla balance type"
            };

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla@test.com", result.OriginatingUserName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Translate_ShouldThrowWhenAccountIsNotSet()
        {
            var accountId = Guid.Empty;
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123,
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("email", "bla@test.com") }));
            var balanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "bla balance type"
            };

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Transalte_ShouldThrowWhenInputIsNull()
        {
            var accountId = Guid.NewGuid();
            var input = default(SetBusinessUnitQuotaModel);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("email", "bla@test.com") }));
            var balanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "bla balance type"
            };

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShouldThrowWhenUserIsNull()
        {
            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123,
            };
            var user = default(ClaimsPrincipal);
            var balanceType = new CompanyBalanceTypeTopUpSettingContract()
            {
                BalanceTypeName = "bla balance type"
            };

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Translate_ShouldThrowWhenBalanceTypeIsNull()
        {
            var accountId = Guid.NewGuid();
            var input = new SetBusinessUnitQuotaModel()
            {
                Amount = 123,
            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("email", "bla@test.com") }));
            var balanceType = default(CompanyBalanceTypeTopUpSettingContract);

            var translatorUnderTest = new SetQuotaTranslator();

            var result = translatorUnderTest.Translate(accountId, input, user, balanceType);
        }
    }
}
