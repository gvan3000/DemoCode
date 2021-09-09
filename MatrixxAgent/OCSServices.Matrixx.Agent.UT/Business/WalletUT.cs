using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class WalletUT
    {
        private Wallet _walletTest;

        [TestInitialize]
        public void SetupTests()
        {
            _walletTest = new Wallet();
        }

        [TestMethod]
        public void GetQueryWalletMultiRequest_ShouldReturnMultiRequest_WhenInputMultiRequest()
        {
            //Arrange
            string msisdn = "44123456789";

            var request = new GetWalletRequest
            {
                MsIsdn = msisdn
            };

            //Act
            var result = _walletTest.GetQueryWalletMultiRequest(request);
            var matrixxObjResult = (WalletQueryRequest)result.RequestCollection.Values[0];
            var msisdnResult = matrixxObjResult.SearchData.SearchCollection.AccessNumber;
            //Assert
            Assert.IsInstanceOfType(result, typeof(MultiRequest));
            Assert.AreEqual(msisdn, msisdnResult);
        }

        [TestMethod]
        public void GetQueryWalletRequest_ShouldReturnWalletQueryRequest_WhenInputWalletQueryRequest()
        {
            //Arrange
            string msisdn = "44123456789";

            var request = new GetWalletRequest
            {
                MsIsdn = msisdn
            };

            //Act
            var result = _walletTest.GetQueryWalletRequest(request);

            //Assert
            Assert.IsInstanceOfType(result, typeof(WalletQueryRequest));
            Assert.AreEqual(msisdn, result.SearchData.SearchCollection.AccessNumber);
        }

        [TestMethod]
        public void GetQueryGroupWalletRequest_ShouldReturnGroupWalletQueryRequest_WhenInputGetGroupWalletRequest()
        {
            //Arrange
            string externalId = "44123456789";

            var request = new GetGroupWalletRequest
            {
                ExternalId = externalId
            };

            //Act
            var result = _walletTest.GetQueryGroupWalletRequest(request);

            //Assert
            Assert.IsInstanceOfType(result, typeof(GroupWalletQueryRequest));
            Assert.AreEqual(externalId, result.SearchData.SearchCollection.ExternalId);
        }
    }
}
