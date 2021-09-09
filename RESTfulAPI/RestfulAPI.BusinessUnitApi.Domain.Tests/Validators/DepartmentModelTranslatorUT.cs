using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Department;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Validators
{
    [TestClass]
    public class DepartmentModelTranslatorUT
    {
        DepartmentModelTranslator _translatorUnderTest;

        Mock<ICustomAppConfiguration> _appConfigurationMock;

        [TestInitialize]
        public void SetUp()
        {
            _appConfigurationMock = new Mock<ICustomAppConfiguration>();
            _appConfigurationMock.Setup(x => x.GetConfigurationValue(It.IsAny<string>(), It.IsAny<string>())).Returns("random value");

            _translatorUnderTest = new DepartmentModelTranslator(_appConfigurationMock.Object);
        }

        [TestMethod]
        public void Translate_ShouldReturn_Null_IfInput_IsNull()
        {           
            DepartmentCostCenterContract input = null;

            var result = _translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_Result_ShouldContain_Id()
        {
            string costCenterName = "c_name";
            string departmentName = "d_name";

            var id = Guid.NewGuid();

            var input = new DepartmentCostCenterContract
            {
                AccountId = Guid.NewGuid(),
                CostCenterName = costCenterName,
                DepartmentName = departmentName,
                Id = id
            };

            var result = _translatorUnderTest.Translate(input);

            Assert.AreEqual(id, result.Id);
        }

        [TestMethod]
        public void Translate_Result_ShouldLocationWith_AccountId()
        {
            string costCenterName = "c_name";
            string departmentName = "d_name";

            var id = Guid.NewGuid();

            var input = new DepartmentCostCenterContract
            {
                AccountId = Guid.NewGuid(),
                CostCenterName = costCenterName,
                DepartmentName = departmentName,
                Id = id
            };

            var result = _translatorUnderTest.Translate(input);

            Assert.IsTrue(result.Location.Contains(input.AccountId.ToString()));
        }

        [TestMethod]
        public void Translate_ShouldCall_ApplicationConfiguration_Twice()
        {
            string costCenterName = "c_name";
            string departmentName = "d_name";

            var id = Guid.NewGuid();

            var input = new DepartmentCostCenterContract
            {
                AccountId = Guid.NewGuid(),
                CostCenterName = costCenterName,
                DepartmentName = departmentName,
                Id = id
            };

            var result = _translatorUnderTest.Translate(input);

            _appConfigurationMock.Verify(x => x.GetConfigurationValue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
