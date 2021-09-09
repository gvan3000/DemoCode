using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Internal.GetBusinessUnit
{
    [TestClass]
    public class AccountIdEqualityComparerUT
    {
        AccountEqualityComparer _comparerUnderTest;
        Guid _accountIdA;
        Guid _accountIdB;
        Guid _planIdA;
        Guid _planIdB;

        [TestInitialize]
        public void SetUp()
        {
            _accountIdA = Guid.NewGuid();
            _accountIdB = Guid.NewGuid();
            _planIdA = Guid.NewGuid();
            _planIdB = Guid.NewGuid();

            _comparerUnderTest = new AccountEqualityComparer();
        }

        [TestMethod]
        public void Equals_ShouldReturn_True_When_Accounts_TheSameId_And_NoPlans()
        {
            var accountA = new AccountContract { Id = _accountIdA };
            var accountB = new AccountContract { Id = _accountIdA };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ShouldReturn_True_When_Accounts_HasTheSameId_And_PlanId()
        {
            var accountA = new AccountContract { Id = _accountIdA, PlanId = _planIdA };
            var accountB = new AccountContract { Id = _accountIdA, PlanId = _planIdA };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Equals_ShouldReturn_False_When_Accounts_HasTheSameId_And_Different_PlanId()
        {
            var accountA = new AccountContract { Id = _accountIdA, PlanId = _planIdA };
            var accountB = new AccountContract { Id = _accountIdA, PlanId = _planIdB };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_ShouldReturn_False_When_Accounts_Id_And_PlanId_AreDifferent()
        {
            var accountA = new AccountContract { Id = _accountIdA, PlanId = _planIdA };
            var accountB = new AccountContract { Id = _accountIdB, PlanId = _planIdB };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_ShouldReturn_False_When_Accounts_Id_AreDefferent_And_PlanId_AreTheSame()
        {
            var accountA = new AccountContract { Id = _accountIdA, PlanId = _planIdB };
            var accountB = new AccountContract { Id = _accountIdB, PlanId = _planIdB };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Equals_ShouldReturn_True_When_Accounts_Id_And_PlanId_AreTheSame()
        {
            var accountA = new AccountContract { Id = _accountIdB, PlanId = _planIdB };
            var accountB = new AccountContract { Id = _accountIdB, PlanId = _planIdB };

            var result = _comparerUnderTest.Equals(accountA, accountB);

            Assert.IsTrue(result);
        }
    }
}
