using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Product;
using RestfulAPI.TeleenaServiceReferences.ProductService;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class ProductModelTranslatorUT
    {
        private ProductModelTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new ProductModelTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturn_ProductModel()
        {
            GetProductResponse model = new GetProductResponse();

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsInstanceOfType(translatedValue, typeof(ProductModel));
        }

        [TestMethod]
        public void Translate_IfPropositionIdIsNull_ShouldReturnNull()
        {
            GetProductResponse model = new GetProductResponse() { PropositionId = null };

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsNull(translatedValue.PropositionId);
        }

        [TestMethod]
        public void Translate_IfPropositionIdIsempty_ShoulReturnNull()
        {
            GetProductResponse model = new GetProductResponse { PropositionId = Guid.Empty };

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsNull(translatedValue.PropositionId);
        }

        [TestMethod]
        public void Translate_ShouldMappPropositionId()
        {
            GetProductResponse model = new GetProductResponse { PropositionId = Guid.NewGuid() };

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.AreEqual(model.PropositionId, translatedValue.PropositionId);
        }

        [TestMethod]
        public void Translate_ShouldRemoveNullValues_IfMsisdnsHaveNulls()
        {
            GetProductResponse model = new GetProductResponse { Msisdns = new string[] { "3123213", "2342354234", null, null } };

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsTrue(translatedValue.Msisdns.All(m => m != null));
        }

        [TestMethod]
        public void Translate_ShouldRemoveNullValues_IfImsisHaveNulls()
        {
            GetProductResponse model = new GetProductResponse { Imsis = new string[] { "8794", "456", null, null, "4234324", null } };

            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsTrue(translatedValue.Imsis.All(m => m != null));
        }

        [TestMethod]
        public void Translate_ShouldIncludeLastStatusDate()
        {
            GetProductResponse model = new GetProductResponse { LastProductStatusChangeDate = DateTime.Now.AddHours(33.6) };
            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsNotNull(translatedValue);
            Assert.AreEqual(model.LastProductStatusChangeDate, translatedValue.LastStatusChangeDate);
        }

        [TestMethod]
        public void Translate_ShouldIncludeDepartmentAndCostCenter()
        {
            GetProductResponse model = new GetProductResponse { DepartmentCostCenterId = Guid.NewGuid() };
            var translatedValue = translatorUnderTest.Translate(model);

            Assert.IsNotNull(translatedValue);
            Assert.AreEqual(model.DepartmentCostCenterId, translatedValue.DepartmentAndCostCenterId);
        }
    }
}
