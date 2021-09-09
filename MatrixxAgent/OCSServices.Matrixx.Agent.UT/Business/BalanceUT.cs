using System;
using OCSServices.Matrixx.Agent.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Agent.Contracts.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class BalanceUT
    {
        private Balance _balanceTest;

        [TestInitialize]
        public void SetupTests()
        {
            _balanceTest = new Balance();
        }

        [TestMethod]
        public void GetQueryBalanceParametersByProductId_ShouldReturnExternalIdQueryParameters_WhenInputProductId()
        {
            //Arrange
            Guid productId = Guid.NewGuid();
            //Act
            var result = (ExternalIdQueryParameters)_balanceTest.GetQueryBalanceParametersByProductId(productId);
            //Assert
            Assert.IsInstanceOfType(result, typeof(IQueryParameters));
            Assert.AreEqual(productId.ToString().ToUpper(), result.ExternalId);
        }

        [TestMethod]
        public void GetQueryBalanceParametersByMsisdn_ShouldReturnAccessNumberQueryParameters_WhenInputMsisdn()
        {
            //Arrange
            string msisdn = "44789456123";
            //Act
            var result = (AccessNumberQueryParameters)_balanceTest.GetQueryBalanceParametersByMsisdn(msisdn);
            //Assert
            Assert.IsInstanceOfType(result, typeof(IQueryParameters));
            Assert.AreEqual(msisdn, result.Msisdn);
        }

        [TestMethod]
        public void GetAdjustBalanceRequest_ShouldReturnSubscriberAdjustBalanceRequest_WhenFlexTopupRequestAndResourceId_ProductIdHasValue()
        {
            //Arrange
            Guid productId = Guid.NewGuid();
            string msisdn = "44789456123";
            decimal amount = 112;
            DateTime? endtime = DateTime.Now;
            string reason = "test reason";
            int templateid = 1;
            string balancetype = "test1";
            bool purchaseoffer = true;
            string offername = "test1";
            bool isquota = true;

            int resourceId = 123;

            var request = new FlexTopupRequest()
            {
                ProductId = productId,
                Msisdn = msisdn,
                Amount = amount,
                EndTime = endtime,
                Reason = reason,
                TemplateId = templateid,
                BalanceType = balancetype,
                PurchaseOffer = purchaseoffer,
                OfferName = offername,
                IsQuota = isquota
            };

            //Act
            var result = _balanceTest.GetAdjustBalanceRequest(request, resourceId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(SubscriberAdjustBalanceRequest));
            Assert.AreEqual(result.AdjustType, 1);
            Assert.AreNotEqual(result.AdjustType, 2);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreNotEqual(amount, result.Amount * -1);
            Assert.AreEqual(resourceId, result.BalanceResourceId);
            Assert.AreEqual(endtime, result.EndTime);
            Assert.AreEqual(reason, result.Reason);
            Assert.AreEqual(productId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
        }

        [TestMethod]
        public void GetAdjustBalanceRequest_ShouldReturnSubscriberAdjustBalanceRequest_WhenFlexTopupRequestAndResourceId_ProductIdHasNull()
        {
            //Arrange
            string msisdn = "44789456123";
            decimal amount = -1;
            DateTime? endtime = DateTime.Now;
            string reason = "test reason";
            int templateid = 1;
            string balancetype = "test1";
            bool purchaseoffer = true;
            string offername = "test1";
            bool isquota = true;

            int resourceId = 123;

            var request = new FlexTopupRequest()
            {
                Msisdn = msisdn,
                Amount = amount,
                EndTime = endtime,
                Reason = reason,
                TemplateId = templateid,
                BalanceType = balancetype,
                PurchaseOffer = purchaseoffer,
                OfferName = offername,
                IsQuota = isquota
            };

            //Act
            var result = _balanceTest.GetAdjustBalanceRequest(request, resourceId);

            //Assert
            Assert.IsInstanceOfType(result, typeof(SubscriberAdjustBalanceRequest));
            Assert.AreEqual(result.AdjustType, 2);
            Assert.AreNotEqual(result.AdjustType, 1);
            Assert.AreEqual(amount, result.Amount * -1);
            Assert.AreNotEqual(amount, result.Amount);
            Assert.AreEqual(resourceId, result.BalanceResourceId);
            Assert.AreEqual(endtime, result.EndTime);
            Assert.AreEqual(reason, result.Reason);
            Assert.AreEqual(msisdn, result.SearchData.SearchCollection.AccessNumber);
        }

        [TestMethod]
        public void GetAdjustBalanceRequest_ShouldReturnSubscriberAdjustBalanceRequest_WhenAdjustBalanceForSubscriberRequest()
        {
            //Arrange
            Guid productId = Guid.NewGuid();
            int balanceresourceid = 1;
            int adjusttype = 2;
            decimal amount = 122;
            DateTime? endtime = DateTime.Now.AddDays(1);
            DateTime? starttime = DateTime.Now;
            string reason = "test reason";

            var request = new AdjustBalanceForSubscriberRequest
            {
                ProductId = productId,
                BalanceResourceId = balanceresourceid,
                AdjustType = adjusttype,
                Amount = amount,
                EndTime = endtime,
                StartTime = starttime,
                Reason = reason
            };

            //Act
            var result = _balanceTest.GetAdjustBalanceRequest(request);

            //Assert
            Assert.IsInstanceOfType(result, typeof(SubscriberAdjustBalanceRequest));
            Assert.AreEqual(productId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(adjusttype ,result.AdjustType);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreNotEqual(amount, result.Amount * -1);
            Assert.AreEqual(balanceresourceid, result.BalanceResourceId);
            Assert.AreEqual(endtime, result.EndTime);
            Assert.AreEqual(starttime, result.StartTime);
            Assert.AreEqual(reason, result.Reason);
        }

        [TestMethod]
        public void GetGroupAdjustBalanceRequest_ShouldReturnGroupAdjustBalanceRequest_WhenAdjustBalanceForGroupRequest()
        {
            //Arrange
            Guid businessunitid = Guid.NewGuid();
            int balanceresourceid = 1;
            int adjusttype = 2;
            decimal amount = 122;
            DateTime? endtime = DateTime.Now.AddDays(1);
            DateTime? starttime = DateTime.Now;
            string reason = "test reason";

            var request = new AdjustBalanceForGroupRequest
            {
                BusinessUnitId = businessunitid,
                BalanceResourceId = balanceresourceid,
                AdjustType = adjusttype,
                Amount = amount,
                EndTime = endtime,
                StartTime = starttime,
                Reason = reason
            };

            //Act
            var result = _balanceTest.GetGroupAdjustBalanceRequest(request);

            //Assert
            Assert.IsInstanceOfType(result, typeof(GroupAdjustBalanceRequest));
            Assert.AreEqual(businessunitid.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(adjusttype, result.AdjustType);
            Assert.AreEqual(amount, result.Amount);
            Assert.AreNotEqual(amount, result.Amount * -1);
            Assert.AreEqual(balanceresourceid, result.BalanceResourceId);
            Assert.AreEqual(endtime, result.EndTime);
            Assert.AreEqual(starttime, result.StartTime);
            Assert.AreEqual(reason, result.Reason);
        }
    }
}
