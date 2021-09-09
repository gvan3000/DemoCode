using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.ProductImei;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using System.Diagnostics.CodeAnalysis;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ProductImeiTranslatorUT
    {
        private ProductImeiModelListTranslator translatorUnderTest;
        private ProductImeiByBusinessUnitDataContract[] modelWithValues;
        
        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new ProductImeiModelListTranslator();
            modelWithValues = new[]
            {
                new ProductImeiByBusinessUnitDataContract
                {
                    Iccid = "1",
                    CreationDate = DateTimeOffset.MinValue,
                    Imei = "2",
                    ProductId = Guid.Empty
                }
            };
        }

        [TestMethod]
        public void Translate_InputOfProductImeiByBusinessUnitDataContract_ShouldReturnProductImeiListModel()
        {
            var model = new ProductImeiByBusinessUnitDataContract[] {};

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsInstanceOfType(translatedValue, typeof(ProductImeiListModel));
        }

        [TestMethod]
        [SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
        public void Translate_IfInputIsNull_ShouldReturnNull()
        {
            ProductImeiByBusinessUnitDataContract[] model = null;

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsNull(translatedValue);
        }

        [TestMethod]
        public void Translate_IfInputHasImei_ShouldMapToImei()
        {
            var translatedValue = translatorUnderTest.Translate(modelWithValues);

            Assert.AreEqual(translatedValue.Products[0].Imei, modelWithValues[0].Imei);
        }
        
        [TestMethod]
        public void Translate_IfInputHasCreationDate_ShouldMapToCreationDate()
        {
            var translatedValue = translatorUnderTest.Translate(modelWithValues);

            Assert.AreEqual(translatedValue.Products[0].CreationDate, modelWithValues[0].CreationDate);
        }
                
        [TestMethod]
        public void Translate_IfInputHasIccid_ShouldMapToIccid()
        {
            var translatedValue = translatorUnderTest.Translate(modelWithValues);

            Assert.AreEqual(translatedValue.Products[0].Iccid, modelWithValues[0].Iccid);
        }
                        
        [TestMethod]
        public void Translate_IfInputHasProductId_ShouldMapToId()
        {
            var translatedValue = translatorUnderTest.Translate(modelWithValues);

            Assert.AreEqual(translatedValue.Products[0].Id, modelWithValues[0].ProductId);
        }
    }
}