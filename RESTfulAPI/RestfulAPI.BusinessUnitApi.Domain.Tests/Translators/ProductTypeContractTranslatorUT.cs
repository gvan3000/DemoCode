using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.ProductType;
using RestfulAPI.TeleenaServiceReferences.ProductTypeService;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ProductTypeContractTranslatorUT
    {
        private ProductTypeContractTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new ProductTypeContractTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyListResponse_IfInputIsNull()
        {
            ProductTypeContract[] input = null;

            var result = translatorUnderTest.Translate(input);

            Assert.IsTrue(!result.ProductTypes.Any());
        }

        [TestMethod]
        public void Translate_ShouldReturnEmptyListResponse_IfInputIsEmptyArray()
        {
            ProductTypeContract[] input = new ProductTypeContract[] { };

            var result = translatorUnderTest.Translate(input);

            Assert.IsTrue(!result.ProductTypes.Any());
        }

        [TestMethod]
        public void Translate_ShouldReturn_ProductTypeListResponseModel()
        {
            ProductTypeContract[] input = new ProductTypeContract[] 
            {
                new ProductTypeContract { Id = Guid.NewGuid(), Name = "TypeProd1" },
                new ProductTypeContract { Id = Guid.NewGuid(), Name = "TypeName2"},
                new ProductTypeContract { Id = Guid.NewGuid(), Name = "TypeProd3" }
            };

            var result = translatorUnderTest.Translate(input);

            Assert.IsInstanceOfType(result, typeof(ProductTypeListResponseModel));
        }

        [TestMethod]
        public void Transalte_ShouldMappIdAndName()
        {
            Guid id = Guid.NewGuid();
            string typeName = "prodTypeName1";

            ProductTypeContract[] input = new ProductTypeContract[]
            {
                new ProductTypeContract { Id = id, Name = typeName }              
            };

            var result = translatorUnderTest.Translate(input);

            Assert.AreEqual(id, result.ProductTypes.Select(x => x.Id).FirstOrDefault());
            Assert.AreEqual(typeName, result.ProductTypes.Select(x => x.Name).FirstOrDefault());
        }
    }
}
