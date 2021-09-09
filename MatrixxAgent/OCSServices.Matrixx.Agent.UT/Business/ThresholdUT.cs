using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Threshold;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class ThresholdUT
    {
        private Threshold _thresholdUnderTest;

        [TestInitialize]
        public void SetupTests()
        {
            _thresholdUnderTest = new Threshold();
        }

        [TestMethod]
        public void BuildSubscriberAddThresholdRequest_ShouldReturnSubscriberAddThresholdRequest_WhenInputAddThresholdToSubscriberRequest()
        {
            var request = new AddThresholdToSubscriberRequest
            {
                Amount = 2.9m,
                CrmProductId = Guid.NewGuid(),
                ResourceId = 223344,
                ThresholdId = 9876
            };

           var result = _thresholdUnderTest.BuildSubscriberAddThresholdRequest(request);

            Assert.IsInstanceOfType(result, typeof(SubscriberAddThresholdRequest));
            Assert.AreEqual(request.ResourceId, result.BalanceResourceId);
            Assert.AreEqual(request.CrmProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(request.ThresholdId, result.ThresholdData.ThresholdCollection.ThresholdId);
            Assert.AreEqual(request.Amount, result.ThresholdData.ThresholdCollection.Amount);
        }

        [TestMethod]
        public void BuildSubscriberAddThresholdRequest_ShouldReturnNull_WhenInputIsNull()
        {
            AddThresholdToSubscriberRequest request = null;

            var result = _thresholdUnderTest.BuildSubscriberAddThresholdRequest(request);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void BuildGroupAddThresholdRequest_ShouldReturnGroupAddThresholdRequest_WhenInputAddThresholdToGroupRequest()
        {
            var request = new AddThresholdToGroupRequest
            {
                Amount = 5.1m,
                ResourceId = 123,
                ThresholdId = 87594
            };

            var result = _thresholdUnderTest.BuildGroupAddThresholdRequest(request);
            Assert.IsInstanceOfType(result, typeof(GroupAddThresholdRequest));
            Assert.AreEqual(request.ResourceId, result.BalanceResourceId);
            Assert.AreEqual(request.ThresholdId, result.ThresholdData.ThresholdCollection.ThresholdId);
            Assert.AreEqual(request.Amount, result.ThresholdData.ThresholdCollection.Amount);
        }

        [TestMethod]
        public void BuildGroupAddThresholdRequest_ShouldReturnNull_WhenInputIsNull()
        {
            AddThresholdToGroupRequest request = null;

            var result = _thresholdUnderTest.BuildGroupAddThresholdRequest(request);
            
            Assert.IsNull(result);
        }

        [TestMethod]
        public void BuildSubscriberSetThresholdToInfinityRequest_ShouldReturnSubscriberSetThresholdToInfinityRequest_WhenInputSetThresholdSubscriberToInfinityRequest()
        {
            var request = new SetThresholdSubscriberToInfinityRequest
            {
                CrmProductId = Guid.NewGuid(),
                ResourceId = 1011,
                ThresholdId = 9876
            };

            var result = _thresholdUnderTest.BuildSubscriberSetThresholdToInfinityRequest(request);

            Assert.IsInstanceOfType(result, typeof(SubscriberSetThresholdToInfinityRequest));
            Assert.AreEqual(request.ResourceId, result.BalanceResourceId);
            Assert.AreEqual(request.CrmProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(request.ThresholdId, result.ThresholdData.ThresholdCollection.ThresholdId);
        }

        [TestMethod]
        public void BuildSubscriberSetThresholdToInfinityRequest_ShouldReturnNull_WhenInputNull()
        {
            SetThresholdSubscriberToInfinityRequest request = null;

            var result = _thresholdUnderTest.BuildSubscriberSetThresholdToInfinityRequest(request);

            Assert.IsNull(result);
        }
    }
}
