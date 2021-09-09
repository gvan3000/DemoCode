using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BalanceProfile;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.Translators
{
    [TestClass]
    public class BalanceProfileListTranslatorUT
    {
        private Mock<ITranslate<SysCodeContract, BalanceProfileModel>> mockInnerTranslator;
        [TestInitialize]
        public void SetupEachTest()
        {
            mockInnerTranslator = new Mock<ITranslate<SysCodeContract, BalanceProfileModel>>(MockBehavior.Strict);
            mockInnerTranslator.Setup(x => x.Translate(It.IsAny<SysCodeContract>()))
                .Returns(new BalanceProfileModel());
        }

        [TestMethod]
        public void Translate_ShouldReturnNullIfInputIsNull()
        {
            var input = default(SysCodeContract[]);

            var translatorUnderTest = new BalanceProfileListTranslator(mockInnerTranslator.Object);

            var result = translatorUnderTest.Translate(input);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Translate_ShouldCallInnerTranslatorForEachInputElement()
        {
            var input = new SysCodeContract[]
            {
                new SysCodeContract() { Id = Guid.NewGuid(), Code = "bla1" },
                new SysCodeContract() { Id = Guid.NewGuid(), Code = "bla2" },
                new SysCodeContract() { Id = Guid.NewGuid(), Code = "bla3" },
                new SysCodeContract() { Id = Guid.NewGuid(), Code = "bla4" }
            };

            var translatorUnderTest = new BalanceProfileListTranslator(mockInnerTranslator.Object);

            var result = translatorUnderTest.Translate(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(input.Length, result.BalanceProfiles.Count);

            mockInnerTranslator.Verify(x => x.Translate(It.IsAny<SysCodeContract>()), Times.Exactly(input.Length));
        }
    }
}
