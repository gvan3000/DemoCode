using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class GetDepartmentsModelTranslatorUT
    {
        Domain.Translators.BusinessUnit.GetDepartmentsModelTranslator _translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            _translatorUnderTest = new Domain.Translators.BusinessUnit.GetDepartmentsModelTranslator();
        }

        [TestMethod]
        public void Translate_ShouldMapp_Id()
        {
            List<DepartmentCostCenterContract> departmentCostCenterContracts = new List<DepartmentCostCenterContract>
            {
                new DepartmentCostCenterContract { Id = Guid.NewGuid() },
                new DepartmentCostCenterContract { Id = Guid.NewGuid() },
                new DepartmentCostCenterContract { Id = Guid.NewGuid() }
            };

            var businessUnit = Guid.NewGuid();
            var result = _translatorUnderTest.Translate(departmentCostCenterContracts);

            Assert.AreEqual(result.GetDepartments[0].Id, departmentCostCenterContracts[0].Id);
            Assert.AreEqual(result.GetDepartments[1].Id, departmentCostCenterContracts[1].Id);
            Assert.AreEqual(result.GetDepartments[2].Id, departmentCostCenterContracts[2].Id);
        }

        [TestMethod]
        public void Translate_ShouldMapp_DepartmentName()
        {
            List<DepartmentCostCenterContract> departmentCostCenterContracts = new List<DepartmentCostCenterContract>
            {
                new DepartmentCostCenterContract { DepartmentName = "Nokia" },
                new DepartmentCostCenterContract { DepartmentName = "Siemens"},
                new DepartmentCostCenterContract { DepartmentName = "Xiaomi" }
            };

            var businessUnit = Guid.NewGuid();
            var result = _translatorUnderTest.Translate(departmentCostCenterContracts);

            Assert.AreEqual(result.GetDepartments[0].Name, departmentCostCenterContracts[0].DepartmentName);
            Assert.AreEqual(result.GetDepartments[1].Name, departmentCostCenterContracts[1].DepartmentName);
            Assert.AreEqual(result.GetDepartments[2].Name, departmentCostCenterContracts[2].DepartmentName);
        }

        [TestMethod]
        public void Translate_ShouldMapp_CostCenter()
        {
            List<DepartmentCostCenterContract> departmentCostCenterContracts = new List<DepartmentCostCenterContract>
            {
                new DepartmentCostCenterContract { CostCenterName = "Nokia Center"},
                new DepartmentCostCenterContract { CostCenterName = "Siemens Center"},
                new DepartmentCostCenterContract { CostCenterName = "Xiaomi Center"}
            };

            var businessUnit = Guid.NewGuid();
            var result = _translatorUnderTest.Translate(departmentCostCenterContracts);

            Assert.AreEqual(result.GetDepartments[0].CostCenter, departmentCostCenterContracts[0].CostCenterName);
            Assert.AreEqual(result.GetDepartments[1].CostCenter, departmentCostCenterContracts[1].CostCenterName);
            Assert.AreEqual(result.GetDepartments[2].CostCenter, departmentCostCenterContracts[2].CostCenterName);
        }

        [TestMethod]
        public void Translate_ShouldReturnNull_If_Input_IsNull()
        {
            List<DepartmentCostCenterContract> departmentCostCenterContracts = null;

            var result = _translatorUnderTest.Translate(departmentCostCenterContracts);

            Assert.IsNull(result);
        }
    }
}

