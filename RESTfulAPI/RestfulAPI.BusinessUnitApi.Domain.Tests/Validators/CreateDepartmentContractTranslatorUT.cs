using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Department;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class CreateDepartmentContractTranslatorUT
    {
        CreateDepartmentContractTranslator _translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            _translatorUnderTest = new CreateDepartmentContractTranslator();
        }

        [TestMethod]
        public void Translate_ShouldMappAllFields()
        {
            string costCenter = "c_name";
            string departmentName = "d_name";

            var input = new CreateDepartmentModel
            {
                CostCenter = costCenter,
                Name = departmentName
            };

            var businessUnitId = Guid.NewGuid();

            var result = _translatorUnderTest.Translate(input, businessUnitId);

            Assert.AreEqual(businessUnitId, result.AccountId);
            Assert.AreEqual(costCenter, result.CostCenterName);
            Assert.AreEqual(departmentName, result.DepartmentName);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_If_Input_IsNull()
        {
            CreateDepartmentModel input = null;

            var result = _translatorUnderTest.Translate(input, Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldFillEmptyForCOstCenterIfItsSetTonull()
        {
            string costCenter = null;
            string departmentName = "d_name";

            var input = new CreateDepartmentModel
            {
                CostCenter = costCenter,
                Name = departmentName
            };

            var businessUnitId = Guid.NewGuid();

            var result = _translatorUnderTest.Translate(input, businessUnitId);

            Assert.AreEqual(businessUnitId, result.AccountId);
            Assert.AreEqual(string.Empty, result.CostCenterName);
            Assert.AreEqual(departmentName, result.DepartmentName);
        }
    }
}
