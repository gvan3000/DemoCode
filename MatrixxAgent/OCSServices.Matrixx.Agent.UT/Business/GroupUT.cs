using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class GroupUT
    {
        private Group _groupUnderTest;


        [TestInitialize]
        public void SetupTests()
        {
            _groupUnderTest = new Group();
        }


        [TestMethod]
        public void GetGroupRequest_ShouldReturnGroupIdQueryParameters_WhenInputString()
        {
            //Arrange
            string identifier = "identifier";

            //Act
            var result = _groupUnderTest.GetGroupRequest(identifier);

            //Assert
            Assert.IsInstanceOfType(result, typeof(GroupIdQueryParameters));
            Assert.AreEqual(identifier, result.ExternalId);
        }


        [TestMethod]
        public void BuildCreateGroupRequest_ShouldReturnMultiRequest_WhenAddGroupRequest()
        {
            //Arrange
            string adminExternalId = "adminExternalId";
            int? billCycleOffset = 11;
            string billingCycleId = "billingCycleId";
            string externalId = "externalId";
            string groupCode = "groupCode";
            int? groupReAuthPreference = 2;
            int? notificationPreference = 3;
            int status = 4;
            string tier = "tier";
            string multiRequestIndex = "0";//hardcoded

            var request = new AddGroupRequest()
            {
                AdminExternalId = adminExternalId,
                BillCycleOffset = billCycleOffset,
                BillingCycleId = billingCycleId,
                ExternalId = externalId,
                GroupCode = groupCode,
                GroupReAuthPreference = groupReAuthPreference,
                NotificationPreference = notificationPreference,
                Status = status,
                Tier = tier
            };

            //Act
            var result = _groupUnderTest.BuildCreateGroupRequest(request);

            //Assert
            Assert.IsInstanceOfType(result, typeof(MultiRequest));
            
            var createGroupRequest = result.RequestCollection.Values.Find(x => x.GetType() == typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.Group.CreateGroupRequest));
            Assert.AreEqual(externalId, ((CreateGroupRequest)createGroupRequest).ExternalId);
            Assert.AreEqual(adminExternalId, ((CreateGroupRequest)createGroupRequest).GroupAdmin.Subscriber.ExternalId);
            Assert.AreEqual(groupCode, ((CreateGroupRequest)createGroupRequest).Name);
            Assert.AreEqual(notificationPreference, ((CreateGroupRequest)createGroupRequest).NotificationPreference);
            Assert.AreEqual(groupReAuthPreference, ((CreateGroupRequest)createGroupRequest).GroupReAuthPreference);

            var modifyGroupRequest = result.RequestCollection.Values.Find(x => x.GetType() == typeof(OCSServices.Matrixx.Api.Client.Contracts.Request.Group.ModifyGroupRequest));
            Assert.AreEqual(billingCycleId, ((ModifyGroupRequest)modifyGroupRequest).BillingCycleCollection.BillingCycle.BillingCycleId);
            Assert.AreEqual(billCycleOffset, ((ModifyGroupRequest)modifyGroupRequest).BillingCycleCollection.BillingCycle.DateOffset);
            Assert.AreEqual(multiRequestIndex, ((ModifyGroupRequest)modifyGroupRequest).GroupSearchData.SearchCollection.MultiRequestIndex);
        }

        [TestMethod]
        public void BuildModifyGroupRequest_ShouldReturnModifyGroupRequest_WhenUpdateGroupRequest()
        {
            //Arrange
            string externalId = "externalId";
            string name = "name";
            string newExternalId = "newExternalId";
            string tierName = "tierName";

            var request = new UpdateGroupRequest()
            {
                ExternalId = externalId,
                Name = name,
                NewExternalId = newExternalId,
                TierName = tierName
            };

            //Act
            var result = _groupUnderTest.BuildModifyGroupRequest(request);

            //Assert
            Assert.AreEqual(newExternalId, result.ExternalId);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(tierName, result.TierName);
            Assert.AreEqual(externalId, result.GroupSearchData.SearchCollection.ExternalId);
        }

        [TestMethod]
        public void BuidPurchaseGroupOfferRequest_ShouldPurchaseGroupOfferRequest_WhenAddOfferToGroupRequest()
        {
            //Arrange
            string externalId = null;
            DateTime? startTime = DateTime.UtcNow;
            DateTime? endTime = DateTime.UtcNow.AddDays(20);
            string offerCode = "offerCode";

            var configuration = new Dictionary<string, string>();
           
            var request = new AddOfferToGroupRequest()
            {
                ExternalId = externalId,
                StartTime = startTime,
                EndTime = endTime,
                OfferCode = offerCode,
                CustomPurchaseOfferConfigurationParameters = configuration
            };

            //Act
            var result = _groupUnderTest.BuidPurchaseGroupOfferRequest(request);

            //Assert
            Assert.AreEqual(externalId, result.GroupSearchData.SearchCollection.ExternalId);
            Assert.AreEqual(offerCode, result.PurchaseInfoList.Values.First<PurchasedOfferData>().ExternalId);
            Assert.AreEqual(startTime, result.PurchaseInfoList.Values.First<PurchasedOfferData>().StartTime);
            Assert.AreEqual(endTime, result.PurchaseInfoList.Values.First<PurchasedOfferData>().EndTime);
            Assert.AreEqual(configuration, result.PurchaseInfoList.Values.First<PurchasedOfferData>().CustomPurchaseOfferConfiguration.CustomPurchaseOfferConfiguration.Configuration);
        }
    }
}
