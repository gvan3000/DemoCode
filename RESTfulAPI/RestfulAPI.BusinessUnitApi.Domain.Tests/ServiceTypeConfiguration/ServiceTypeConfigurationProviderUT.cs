using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ServiceTypeConfiguration
{
    [TestClass]
    public class ServiceTypeConfigurationProviderUT
    {
        [TestMethod]
        public void DeserializeList_ShouldParseStringValueIntoList()
        {
            var input = "ONE; TWO; TREE";
            var result = ServiceTypeConfigurationProvider.DeserializeList(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void DeserializeList_ShouldThrowWhenInputStringIsNull()
        {
            var result = ServiceTypeConfigurationProvider.DeserializeList(null);
        }

        [TestMethod]
        public void DeserialzieList_ShouldTrimOutput()
        {
            var input = "ONE; TWO; TREE";
            var result = ServiceTypeConfigurationProvider.DeserializeList(input);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result[2]);
            Assert.AreNotEqual(string.Empty, result[2]);
            Assert.AreNotEqual(" TREE", result[2]);
            Assert.IsNotNull(result[1]);
            Assert.AreNotEqual(string.Empty, result[1]);
            Assert.AreNotEqual(" TWO", result[1]);
        }

        [TestMethod]
        public void DeserializeDictionary_ShouldParseInputValueIntoDictionary()
        {
            var input = "ONE=1; TWO = 2; TREE=3;";
            var result = ServiceTypeConfigurationProvider.DeserializeDictionary(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void DeserializeDictionary_ShouldThrowWhenInputIsNull()
        {
            var result = ServiceTypeConfigurationProvider.DeserializeDictionary(null);
        }

        [TestMethod]
        public void DeserializeDictionary_ShouldTrimBothKeysAndValues()
        {
            var input = "ONE=1; TWO = 2; TREE=3;";
            var result = ServiceTypeConfigurationProvider.DeserializeDictionary(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            Assert.IsNotNull(result["TWO"]);
            Assert.AreNotEqual(string.Empty, result["TWO"]);
            Assert.AreEqual("2", result["TWO"]);

            Assert.IsNotNull(result["TREE"]);
            Assert.AreNotEqual(string.Empty, result["TREE"]);
            Assert.AreEqual("3", result["TREE"]);
        }

        [TestMethod]
        public void DeserializeSingle_ShouldParseStringIntoKeyValuePair()
        {
            var input = "key=value";

            var result = ServiceTypeConfigurationProvider.DeserializeSingle(input);

            Assert.AreEqual("key", result.Key);
            Assert.AreEqual("value", result.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void DeserializeSingle_ShouldThrowIfInputIsNull()
        {
            var result = ServiceTypeConfigurationProvider.DeserializeSingle(null);
        }

        [TestMethod]
        public void DeserializeSingle_ShouldTrimKeyAndValue()
        {
            var input = " key = value ";

            var result = ServiceTypeConfigurationProvider.DeserializeSingle(input);

            Assert.AreEqual("key", result.Key);
            Assert.AreEqual("value", result.Value);
        }
    }
}
