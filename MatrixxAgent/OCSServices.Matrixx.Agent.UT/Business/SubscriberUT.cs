using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using SplitProvisioning.Base.Data;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class SubscriberUT
    {
        private Subscriber _subscriber;

        [TestInitialize]
        public void TestInitialize()
        {
            _subscriber = new Subscriber();
        }

        [TestMethod]
        public void Test_BuildCreateSubscriberRequest_WithGroupCode()
        {
            var createSubscriberRequest = new CreateSubscriberRequest
            {
                Endpoint = new Endpoint(),
                CrmProductId = Guid.NewGuid(),
                BillingCycleId = "5",
                BillingDateOffset = 2,
                CreateWithStatus = 3,
                GroupCode = "GroupCode",
                ImsiList = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                },
                MembershipCodes = new List<string>
                {
                    "Code1",
                    "Code2"
                },
                MsisdnList = new List<string>
                {
                    "31637000001",
                    "31637000002"
                }
            };

            var result = _subscriber.BuildCreateSubscriberRequest(createSubscriberRequest);
            Assert.IsInstanceOfType(result, typeof(MultiRequest), "Result is not of type 'MultiRequest'");

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.CreateSubscriberRequest)), 1, "Result contains more than 1 CreateSubscriberRequest");

            var translatedRequest = (api.CreateSubscriberRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(api.CreateSubscriberRequest));
            Assert.AreEqual(createSubscriberRequest.CreateWithStatus, translatedRequest.Status, "Status has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.CrmProductId.ToString().ToUpper(), translatedRequest.ExternalId, "CrmProductId has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.MsisdnList.First(), translatedRequest.ContactPhoneNumber, "Msisdn to phone number has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.ImsiList.First(), translatedRequest.FirstName, "Imsi to firstname has not been correctly translated");


            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(CreateDeviceRequest)), createSubscriberRequest.ImsiList.Count, "Number of CreateDeviceRequest is not the same as number of imsi's");
            var deviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(CreateDeviceRequest)).Cast<CreateDeviceRequest>().ToList();

            for (var i = 0; i < createSubscriberRequest.ImsiList.Count; i++)
            {
                Assert.AreEqual(deviceRequests[i].MobileDeviceExtensions.MobileDeviceExtension.Imsi, createSubscriberRequest.ImsiList[i], "Imsi in CreateDeviceRequest does not match imsi in the source");
                if (i == 0)
                    CollectionAssert.AreEqual(createSubscriberRequest.MsisdnList, deviceRequests[0].MobileDeviceExtensions.MobileDeviceExtension.AccessNumberList.Values, "AccessNumberList for primary imsi does not match");
            }

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.SubscriberAddDeviceRequest)), createSubscriberRequest.ImsiList.Count, "Number of SubscriberAddDeviceRequest does not match number of imsi's");
            var subscriberAddDeviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(api.SubscriberAddDeviceRequest)).Cast<api.SubscriberAddDeviceRequest>().ToList();
            for (var i = 0; i < createSubscriberRequest.ImsiList.Count; i++)
            {
                Assert.AreEqual(subscriberAddDeviceRequests[i].SubscriberSearchData.SearchCollection.MultiRequestIndex, "0", "MultiRequestIndex value in SubscriberAddDeviceRequest is not '0'");
                Assert.AreEqual(subscriberAddDeviceRequests[i].DeviceSearchData.SearchCollection.MultiRequestIndex, (i + 1).ToString(), "MultiRequestIndex value in SubscriberAddDeviceRequest is not correct");
            }

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.ModifySubscriberRequest)), 1, "Number of ModifySubscriberRequest is not correct");
            var modifySubscriberRequest = (api.ModifySubscriberRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(api.ModifySubscriberRequest));
            Assert.AreEqual(modifySubscriberRequest.SearchData.SearchCollection.MultiRequestIndex, "0", "MultiRequestIndex value in ModifySubscriberRequest is not '0'");
            Assert.AreEqual(modifySubscriberRequest.BillingCycleConfigurations.BillingCycle.BillingCycleId, createSubscriberRequest.BillingCycleId, "BillingCycleId has not been correctly translated");
            Assert.AreEqual(modifySubscriberRequest.BillingCycleConfigurations.BillingCycle.DateOffset, 2, "DateOffset has not been correctly translated");

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(AddGroupMembershipRequest)), 1, "Number of AddGroupMembershipRequest is not correct");
            var addGroupMembershipRequest = (AddGroupMembershipRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(AddGroupMembershipRequest));
            Assert.AreEqual(addGroupMembershipRequest.GroupSearchData.SearchCollection.ExternalId, createSubscriberRequest.GroupCode, "GroupCode has not been correctly translated");
            Assert.AreEqual(addGroupMembershipRequest.GroupMembers.SearchCollection.MultiRequestIndex, "0", "MultiRequestIndex value of AddGroupMembershipRequest is not '0'");
        }

        [TestMethod]
        public void Test_BuildCreateSubscriberRequest_WithoutGroupCode()
        {
            var createSubscriberRequest = new CreateSubscriberRequest
            {
                Endpoint = new Endpoint(),
                CrmProductId = Guid.NewGuid(),
                BillingCycleId = "5",
                BillingDateOffset = 2,
                CreateWithStatus = 3,
                ImsiList = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                },
                MembershipCodes = new List<string>
                {
                    "Code1",
                    "Code2"
                },
                MsisdnList = new List<string>
                {
                    "31637000001",
                    "31637000002"
                }
            };

            var result = _subscriber.BuildCreateSubscriberRequest(createSubscriberRequest);
            Assert.IsInstanceOfType(result, typeof(MultiRequest), "Result is not of type 'MultiRequest'");

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.CreateSubscriberRequest)), 1, "Result contains more than 1 CreateSubscriberRequest");

            var translatedRequest = (api.CreateSubscriberRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(api.CreateSubscriberRequest));
            Assert.AreEqual(createSubscriberRequest.CreateWithStatus, translatedRequest.Status, "Status has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.CrmProductId.ToString().ToUpper(), translatedRequest.ExternalId, "CrmProductId has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.MsisdnList.First(), translatedRequest.ContactPhoneNumber, "Msisdn to phone number has not been correctly translated");
            Assert.AreEqual(createSubscriberRequest.ImsiList.First(), translatedRequest.FirstName, "Imsi to firstname has not been correctly translated");


            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(CreateDeviceRequest)), createSubscriberRequest.ImsiList.Count, "Number of CreateDeviceRequest is not the same as number of imsi's");
            var deviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(CreateDeviceRequest)).Cast<CreateDeviceRequest>().ToList();

            for (var i = 0; i < createSubscriberRequest.ImsiList.Count; i++)
            {
                Assert.AreEqual(deviceRequests[i].MobileDeviceExtensions.MobileDeviceExtension.Imsi, createSubscriberRequest.ImsiList[i], "Imsi in CreateDeviceRequest does not match imsi in the source");
                if (i == 0)
                    CollectionAssert.AreEqual(createSubscriberRequest.MsisdnList, deviceRequests[0].MobileDeviceExtensions.MobileDeviceExtension.AccessNumberList.Values, "AccessNumberList for primary imsi does not match");
            }

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.SubscriberAddDeviceRequest)), createSubscriberRequest.ImsiList.Count, "Number of SubscriberAddDeviceRequest does not match number of imsi's");
            var subscriberAddDeviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(api.SubscriberAddDeviceRequest)).Cast<api.SubscriberAddDeviceRequest>().ToList();
            for (var i = 0; i < createSubscriberRequest.ImsiList.Count; i++)
            {
                Assert.AreEqual(subscriberAddDeviceRequests[i].SubscriberSearchData.SearchCollection.MultiRequestIndex, "0", "MultiRequestIndex value in SubscriberAddDeviceRequest is not '0'");
                Assert.AreEqual(subscriberAddDeviceRequests[i].DeviceSearchData.SearchCollection.MultiRequestIndex, (i + 1).ToString(), "MultiRequestIndex value in SubscriberAddDeviceRequest is not correct");
            }

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.ModifySubscriberRequest)), 1, "Number of ModifySubscriberRequest is not correct");
            var modifySubscriberRequest = (api.ModifySubscriberRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(api.ModifySubscriberRequest));
            Assert.AreEqual(modifySubscriberRequest.SearchData.SearchCollection.MultiRequestIndex, "0", "MultiRequestIndex value in ModifySubscriberRequest is not '0'");
            Assert.AreEqual(modifySubscriberRequest.BillingCycleConfigurations.BillingCycle.BillingCycleId, createSubscriberRequest.BillingCycleId, "BillingCycleId has not been correctly translated");
            Assert.AreEqual(modifySubscriberRequest.BillingCycleConfigurations.BillingCycle.DateOffset, 2, "DateOffset has not been correctly translated");

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(AddGroupMembershipRequest)), createSubscriberRequest.MembershipCodes.Count, "Number of AddGroupMembershipRequest is not correct");
            var addGroupMembershipRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(AddGroupMembershipRequest)).Cast<AddGroupMembershipRequest>().ToList();
            var groupCodes = addGroupMembershipRequests.Select(x => x.GroupSearchData.SearchCollection.ExternalId);
            CollectionAssert.AreEqual(createSubscriberRequest.MembershipCodes, groupCodes.ToList(), "List of Membershipcodes is not correctly translated");
        }

        [TestMethod]
        public void Test_BuildSetCustomSubscriberConfigurationRequest()
        {
            var setCustomConfigurationRequest = new SetCustomConfigurationRequest
            {
                CrmProductId = Guid.NewGuid(),
                CustomConfigurationParameters = new Dictionary<string, string>
                {
                    {"Key1", "Value1"},
                    {"Key2", "Value2"},
                    {"ContractType", "ContractValue"}
                }
            };

            var result = _subscriber.BuildSetCustomSubscriberConfigurationRequest(setCustomConfigurationRequest);

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.ModifySubscriberRequest)), 1, "Result does not contain 1 ModifySubscriberRequest");
            var modifySubscriberRequest = (api.ModifySubscriberRequest)result.RequestCollection.Values.First();

            Assert.AreEqual(setCustomConfigurationRequest.CrmProductId.ToString().ToUpper(), modifySubscriberRequest.SearchData.SearchCollection.ExternalId, "CrmProductId not correctly translated");
            Assert.AreEqual(modifySubscriberRequest.CustomSubscriberConfigurations.CustomSubscriberConfiguration.ContractType, setCustomConfigurationRequest.CustomConfigurationParameters["ContractType"]);

            CollectionAssert.AreEqual(modifySubscriberRequest.CustomSubscriberConfigurations.CustomSubscriberConfiguration.Configuration, setCustomConfigurationRequest.CustomConfigurationParameters, "Configuration values dictionaries do not match");
        }

        [TestMethod]
        public void Test_BuildSetStatusRequest()
        {
            var setSubscriberStatusRequest = new SetSubscriberStatusRequest
            {
                CrmProductId = Guid.NewGuid(),
                Status = 2
            };

            var result = _subscriber.BuildSetStatusRequest(setSubscriberStatusRequest);

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.ModifySubscriberRequest)), 1, "Result does not contain 1 ModifySubscriberRequest");
            var modifySubscriberRequest = (api.ModifySubscriberRequest)result.RequestCollection.Values.First();

            Assert.AreEqual(setSubscriberStatusRequest.CrmProductId.ToString().ToUpper(), modifySubscriberRequest.SearchData.SearchCollection.ExternalId, "CrmProductId not correctly translated");
            Assert.AreEqual(setSubscriberStatusRequest.Status, modifySubscriberRequest.Status, "Status not correctly translated");
        }

        [TestMethod]
        public void Test_BuildCreateGroupAdmin()
        {
            var createGroupAdminRequest = new CreateGroupAdminRequest
            {
                GroupCode = "GroupCode",
                SubscriberCreateStatus = 2
            };

            var result = _subscriber.BuildCreateGroupAdmin(createGroupAdminRequest);

            Assert.AreEqual(createGroupAdminRequest.GroupCode, result.ExternalId);
            Assert.AreEqual(createGroupAdminRequest.SubscriberCreateStatus, result.Status);
        }

        [TestMethod]
        public void Test_CreateDetachImsiFromSubscriber()
        {
            var detachImsiFromSubscriberRequest = new DetachImsiFromSubscriberRequest
            {
                CrmProductId = Guid.NewGuid(),
                Imsis = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                }
            };

            var result = _subscriber.CreateDetachImsiFromSubscriber(detachImsiFromSubscriberRequest);

            Assert.AreEqual(result.RequestCollection.Values.Count, detachImsiFromSubscriberRequest.Imsis.Count * 2);
            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.SubscriberRemoveDeviceRequest)), detachImsiFromSubscriberRequest.Imsis.Count);

            var subscriberRemoveDeviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(api.SubscriberRemoveDeviceRequest)).Cast<api.SubscriberRemoveDeviceRequest>().ToList();
            Assert.IsTrue(subscriberRemoveDeviceRequests.Select(x => x.SubscriberSearchData.SearchCollection.ExternalId).All(x => x == detachImsiFromSubscriberRequest.CrmProductId.ToString().ToUpper()));
            CollectionAssert.AreEqual(detachImsiFromSubscriberRequest.Imsis, subscriberRemoveDeviceRequests.Select(x => x.DeviceSearchData.SearchCollection.Imsi).ToList());
            Assert.IsTrue(subscriberRemoveDeviceRequests.Select(x => x.DeleteSession).All(x => x));

            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(DeviceModifyRequest)), detachImsiFromSubscriberRequest.Imsis.Count);
            var deviceModifyRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(DeviceModifyRequest)).Cast<DeviceModifyRequest>().ToList();
            CollectionAssert.AreEqual(detachImsiFromSubscriberRequest.Imsis, deviceModifyRequests.Select(x => x.DeviceSearchData.SearchCollection.Imsi).ToList());
            Assert.IsTrue(deviceModifyRequests.Select(x => x.Status).All(x => x == 2));
        }

        [TestMethod]
        public void Test_CreateModifySubscriberFirstNameRequest()
        {
            var setSubscriberFirstNameRequest = new SetSubscriberFirstNameRequest
            {
                CrmProductId = Guid.NewGuid(),
                FirstName = "NewFirstname"
            };

            var result = _subscriber.CreateModifySubscriberFirstNameRequest(setSubscriberFirstNameRequest);
            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.ModifySubscriberRequest)), 1);
            var modifySubscriberRequest = (api.ModifySubscriberRequest)result.RequestCollection.Values.First();
            Assert.AreEqual(setSubscriberFirstNameRequest.CrmProductId.ToString().ToUpper(), modifySubscriberRequest.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(setSubscriberFirstNameRequest.FirstName, modifySubscriberRequest.FirstName);
        }

        [TestMethod]
        public void Test_CreateModifySubscriberContactPhoneNumberRequest()
        {
            var updateContactPhoneNumberRequest = new UpdateContactPhoneNumberRequest
            {
                CrmProductId = Guid.NewGuid(),
                PrimaryMsisdn = "31637000001"
            };

            var result = _subscriber.CreateModifySubscriberContactPhoneNumberRequest(updateContactPhoneNumberRequest);

            Assert.AreEqual(updateContactPhoneNumberRequest.CrmProductId.ToString().ToUpper(), result.SearchData.SearchCollection.ExternalId);
            Assert.AreEqual(updateContactPhoneNumberRequest.PrimaryMsisdn, result.ContactPhoneNumber);
        }

        [TestMethod]
        public void Test_CreateDeleteSubscriberRequest()
        {
            var deactiveSubscriberRequest = new DeactiveSubscriberRequest
            {
                CrmProductId = Guid.NewGuid(),
                Imsis = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                }
            };

            var result = _subscriber.CreateDeleteSubscriberRequest(deactiveSubscriberRequest);
            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(api.DeleteSubscriberRequest)), 1);
            Assert.AreEqual(result.RequestCollection.Values.Count(x => x.GetType() == typeof(DeviceDeleteRequest)), deactiveSubscriberRequest.Imsis.Count);

            var deleteSubscriberRequest = (api.DeleteSubscriberRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(api.DeleteSubscriberRequest));
            Assert.AreEqual(deactiveSubscriberRequest.CrmProductId.ToString().ToUpper(), deleteSubscriberRequest.SearchData.SearchCollection.ExternalId);
            Assert.IsTrue(deleteSubscriberRequest.Delete);

            var deviceDeleteRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(DeviceDeleteRequest)).Cast<DeviceDeleteRequest>().ToList();
            CollectionAssert.AreEqual(deactiveSubscriberRequest.Imsis, deviceDeleteRequests.Select(x => x.DeviceSearchData.SearchCollection.Imsi).ToList());
        }
    }
}
