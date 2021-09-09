using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class OfferUT
    {
        private Offer _offerUnderTest;

        [TestInitialize]
        public void SetupTests()
        {
            _offerUnderTest = new Offer();
        }

        [TestMethod]
        public void BuildPurchaseOfferForSubscriberRequest_ShouldReturn_PurchaseOfferForSubscriberRequest_WhenCustomPurchaseOfferConfigurationParametersArePresent()
        {
            var addOfferToSubscriberRequest = new AddOfferToSubscriberRequest
            {
                CrmProductId = Guid.NewGuid(),
                OffersToBePurchased = new List<string> { "R2D2", "3CPO" },
                CustomPurchaseOfferConfigurationParameters = new Dictionary<string, string>
                  {
                      {"1", "Homer" },
                      {"2", "Moe" }
                  }
            };

            var result = _offerUnderTest.BuildPurchaseOfferForSubscriberRequest(addOfferToSubscriberRequest);

            Assert.IsInstanceOfType(result, typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest));
            Assert.AreEqual(addOfferToSubscriberRequest.CrmProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(addOfferToSubscriberRequest.OffersToBePurchased[0], result.PurchaseInfoList.Values[0].ExternalId);
            Assert.AreEqual(addOfferToSubscriberRequest.OffersToBePurchased[1], result.PurchaseInfoList.Values[1].ExternalId);
            CollectionAssert.AreEqual(addOfferToSubscriberRequest.CustomPurchaseOfferConfigurationParameters.Values, result.PurchaseInfoList.Values[0].CustomPurchaseOfferConfiguration.CustomPurchaseOfferConfiguration.Configuration.Values);
            CollectionAssert.AreEqual(addOfferToSubscriberRequest.CustomPurchaseOfferConfigurationParameters.Keys, result.PurchaseInfoList.Values[0].CustomPurchaseOfferConfiguration.CustomPurchaseOfferConfiguration.Configuration.Keys);

        }

        [TestMethod]
        public void BuildPurchaseOfferForSubscriberRequest_ShouldReturn_PurchaseOfferForSubscriberRequest_WhenCustomPurchaseOfferConfigurationParametersNotPresent()
        {
            var addOfferToSubscriberRequest = new AddOfferToSubscriberRequest
            {
                CrmProductId = Guid.NewGuid(),
                OffersToBePurchased = new List<string> { "R2D2", "3CPO" }
            };

            var result = _offerUnderTest.BuildPurchaseOfferForSubscriberRequest(addOfferToSubscriberRequest);

            Assert.IsInstanceOfType(result, typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest));
            Assert.AreEqual(addOfferToSubscriberRequest.CrmProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(addOfferToSubscriberRequest.OffersToBePurchased[0], result.PurchaseInfoList.Values[0].ExternalId);
            Assert.AreEqual(addOfferToSubscriberRequest.OffersToBePurchased[1], result.PurchaseInfoList.Values[1].ExternalId);

        }

        [TestMethod]
        public void BuildCancelOfferForSubscriberRequest_ShouldReturnCancelOfferForGroupRequest_WhenCancelOfferForSubscriberRequest()
        {
            var cancelOfferForSubscriberRequest = new Contracts.Requests.CancelOfferForSubscriberRequest
            {
                ObjectId = "Id123",
                OfferIds = new List<int> { 1, 4, 32, 5 }
            };

            var expectedOfferIds = cancelOfferForSubscriberRequest.OfferIds[0].ToString() +
                cancelOfferForSubscriberRequest.OfferIds[1].ToString() +
                cancelOfferForSubscriberRequest.OfferIds[2].ToString() +
                cancelOfferForSubscriberRequest.OfferIds[3].ToString();

            var result = _offerUnderTest.BuildCancelOfferForSubscriberRequest(cancelOfferForSubscriberRequest);
            
            var actualOfferIds = new StringBuilder();
            foreach (var item in result.ResourceIdArray)
            {
                if (!item.Equals(','))
                    actualOfferIds.Append(item);
            }

            Assert.IsInstanceOfType(result, typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest));
            Assert.AreEqual(cancelOfferForSubscriberRequest.ObjectId, result.ObjectId);
            Assert.AreEqual(cancelOfferForSubscriberRequest.OfferIds.ToArray().Length, result.ObjectId.Length - 1);
            Assert.AreEqual(expectedOfferIds, actualOfferIds.ToString());
        }

        [TestMethod]
        public void BuildCancelOfferForGroupRequest_ShouldReturnCancelOfferForGroupRequest_WhenInputIsCancelOfferForGroupRequest()
        {
            var cancelOfferForGroupRequest = new Contracts.Requests.CancelOfferForGroupRequest
            {
                ObjectId = "R2D2",
                OfferIds = new List<int> { 1, 44, 6 }

            };

            var expectedOfferIds = string.Empty;
            foreach (var item in cancelOfferForGroupRequest.OfferIds)
            {
                expectedOfferIds += item;
            }

            var result = _offerUnderTest.BuildCancelOfferForGroupRequest(cancelOfferForGroupRequest);

            var actualOfferIds = new StringBuilder();
            foreach (var item in result.ResourceIdArray)
            {
                if (!item.Equals(','))
                    actualOfferIds.Append(item);
            }

            Assert.IsInstanceOfType(result, typeof(CancelOfferForGroupRequest));
            Assert.AreEqual(cancelOfferForGroupRequest.ObjectId, result.ObjectId);
            Assert.AreEqual(expectedOfferIds, actualOfferIds.ToString());
        }

        [TestMethod]
        public void BuildModifyOfferForSubscriberRequest_ShouldReturnModifyOfferForSubscriberRequest_WhenInputModifyOfferForSubscriberRequest()
        {
            var modifyOfferForSubscriberRequest = new Contracts.Offer.ModifyOfferForSubscriberRequest
            {
                ProductId = Guid.NewGuid(),
                EndTime = DateTime.Now.AddDays(2),
                StartTime = DateTime.Now,
                OfferResourceId = 22
            };

            var result = _offerUnderTest.BuildModifyOfferForSubscriberRequest(modifyOfferForSubscriberRequest);

            Assert.IsInstanceOfType(result, typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer.ModifyOfferForSubscriberRequest));
            Assert.AreEqual(modifyOfferForSubscriberRequest.OfferResourceId, result.OfferResourceId);
            Assert.AreEqual(modifyOfferForSubscriberRequest.StartTime, result.StartTime);
            Assert.AreEqual(modifyOfferForSubscriberRequest.EndTime, result.EndTime);
            Assert.AreEqual(modifyOfferForSubscriberRequest.ProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
        }

        [TestMethod]
        public void BuildModifyOfferForGroupRequest_ShouldReturnModifyOfferForGroupRequest_WhenInputContracts_Offer_ModifyOfferForGroupRequest()
        {
            var modifyOfferForGroupRequest = new Contracts.Offer.ModifyOfferForGroupRequest
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(4),
                OfferResourceId = 14,
                BusinessUnitId = Guid.NewGuid()
            };

            var result = _offerUnderTest.BuildModifyOfferForGroupRequest(modifyOfferForGroupRequest);

            Assert.IsInstanceOfType(result, typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer.ModifyOfferForGroupRequest));
            Assert.AreEqual(modifyOfferForGroupRequest.OfferResourceId, result.OfferResourceId);
            Assert.AreEqual(modifyOfferForGroupRequest.StartTime, result.StartTime);
            Assert.AreEqual(modifyOfferForGroupRequest.EndTime, result.EndTime);
            Assert.AreEqual(modifyOfferForGroupRequest.BusinessUnitId.ToString().ToUpper(), result.GroupSearchData.SearchCollection.ExternalId);
        }
    }
}
