using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Product;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class PageSizeInfoTranslatorUT
    {
        PageSizeInfoTranslator translatorUnderTest;

        [TestInitialize]
        public void SetUp()
        {
            translatorUnderTest = new PageSizeInfoTranslator();
        }

        [TestMethod]
        public void Translate_ShouldReturn_IsPaged_True_IfInputPageSizeHasValue()
        {
            int? pageSize = 350;

            var pageSizeInfo = translatorUnderTest.Translate(pageSize);

            Assert.IsTrue(pageSizeInfo.IsPaged);
        }

        [TestMethod]
        public void Translate_ShouldReturnPageSize_1000_IfInputPageSizeIsGreaterThan1000()
        {
            int? pageSize = 3500;

            var pageSizeInfo = translatorUnderTest.Translate(pageSize);

            Assert.AreEqual(1000, pageSizeInfo.PageSize);
        }

        [TestMethod]
        public void Translate_ShouldReturn_IsPaged_False_IfInputPageSize_IsNull()
        {
            int? pageSize = null;

            var pageSizeInfo = translatorUnderTest.Translate(pageSize);

            Assert.IsFalse(pageSizeInfo.IsPaged);
        }

        [TestMethod]
        public void Translate_ShouldReturn_pageSize_intMax_IfInputPageSizeIsNull()
        {
            int? pageSize = null;

            var pageSizeInfo = translatorUnderTest.Translate(pageSize);

            Assert.AreEqual(int.MaxValue, pageSizeInfo.PageSize);
        }

        [TestMethod]
        public void Translate_ShouldReturnPageSize_equalsAsInput_IfItIsLessThan_1000()
        {
            int? pageSize = 505;

            var pageSizeInfo = translatorUnderTest.Translate(pageSize);

            Assert.AreEqual(pageSize, pageSizeInfo.PageSize);
        }
    }
}
