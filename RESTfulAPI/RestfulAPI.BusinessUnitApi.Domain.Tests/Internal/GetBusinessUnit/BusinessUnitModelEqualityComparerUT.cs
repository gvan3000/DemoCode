using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Internal.GetBusinessUnit
{
    [TestClass]
    public class BusinessUnitModelEqualityComparerUT 
    {
        BusinessUnitModelEqualityComparer _comparerUnderTest;

        BusinessUnitModel _businessUnitModelA;
        BusinessUnitModel _businessUnitModelB;

        Guid _businessUnitIdA;
        Guid _businessUnitIdB;

        [TestInitialize]
        public void SetUp()
        {
            _businessUnitIdA = Guid.NewGuid();
            _businessUnitIdB = Guid.NewGuid();

            _businessUnitModelA = new BusinessUnitModel { Id = _businessUnitIdA };
            _businessUnitModelB = new BusinessUnitModel { Id = _businessUnitIdB };

            _comparerUnderTest = new BusinessUnitModelEqualityComparer();
        }

        [TestMethod]
        public void Equals_ShouldReturn_True_When_BusinessUnits_AreTheSame_ById()
        {
            var resultA = _comparerUnderTest.Equals(_businessUnitModelA, _businessUnitModelA);
            var resultB = _comparerUnderTest.Equals(_businessUnitModelB, _businessUnitModelB);

            Assert.IsTrue(resultA);
            Assert.IsTrue(resultB);
        }

        [TestMethod]
        public void Equals_ShouldReturn_False_When_BusinessUnits_AreDifferent_ById()
        {
            var result = _comparerUnderTest.Equals(_businessUnitModelA, _businessUnitModelB);

            Assert.IsFalse(result);
        }
    }
}
