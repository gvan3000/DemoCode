using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Controllers;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.Common;
using System.Web.Http.Results;

namespace RestfulAPI.BusinessUnitApi.WebApi.Tests.Controllers
{
    /// <summary>
    /// Summary description for DepartmentProviderUT
    /// </summary>
    [TestClass]
    public class DepartmentControllerUT
    {
        DepartmentController _controllerUnderTest;

        Mock<IDepartmentProvider> _providerMocked;

        [TestInitialize]
        public void SetUp()
        {
            _providerMocked = new Mock<IDepartmentProvider>();
            _providerMocked.Setup(x => x.CreateAsync(It.IsAny<CreateDepartmentModel>(), It.IsAny<Guid>()))
                .ReturnsAsync(ProviderOperationResult<CreateDepartmentResponseModel>.OkResult(new CreateDepartmentResponseModel { Id = Guid.NewGuid(), Location = "fakeLocation" }));

            var departmentModel = new GetDepartmentModel
            {
                CostCenter = "Nokia Cost Center",
                Id = Guid.NewGuid(),
                Name = "Nokia"
            };
            
            _providerMocked.Setup(x => x.GetDepartmentAsync(It.IsAny<Guid>()))
            .ReturnsAsync(ProviderOperationResult<GetDepartmentsModel>.OkResult(new GetDepartmentsModel
            {
                GetDepartments = new List<GetDepartmentModel>()
                {
                    departmentModel
                }
            }));
           
            _controllerUnderTest = new DepartmentController(_providerMocked.Object);
        }

        [TestMethod]
        public void Post_ShouldReturnBadRequestIfModelStateNotValid()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_name" };
            _controllerUnderTest.ModelState.AddModelError("random key", "error mesage");

            var result = _controllerUnderTest.Post(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public void Post_ShouldCall_DepartmentProvider_CreateAsync()
        {
            var input = new CreateDepartmentModel { CostCenter = "c_name", Name = "d_name" };
            var result = _controllerUnderTest.Post(input, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();

            _providerMocked.Verify(x => x.CreateAsync(It.IsAny<CreateDepartmentModel>(), It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void Get_ShouldCall_DepartmentProvider_GetDepartmentAsync()
        {
            var businessUnit = Guid.NewGuid();
            var result = _controllerUnderTest.Get(Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
            _providerMocked.Verify(x => x.GetDepartmentAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}
