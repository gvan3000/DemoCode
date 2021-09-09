using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Contracts.Device;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Sim.Swap;
using OCSServices.Matrixx.Api.Client.Contracts.Model;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;

namespace OCSServices.Matrixx.Agent.UT.Business
{
    [TestClass]
    public class DeviceUT
    {
        private Device _device;

        [TestInitialize]
        public void TestInitialize()
        {
            _device = new Device();
        }

        [TestMethod]
        public void Test_CreateValidateDeviceListSession()
        {
            var validateSessionForDeviceListRequest = new ValidateSessionForDeviceListRequest
            {
                SessionType = 2,
                Imsis = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                }
            };

            var result = _device.CreateValidateDeviceListSession(validateSessionForDeviceListRequest);

            Assert.AreEqual(validateSessionForDeviceListRequest.Imsis.Count, result.RequestCollection.Values.Count);
            Assert.AreEqual(validateSessionForDeviceListRequest.Imsis.Count, result.RequestCollection.Values.Count(x => x.GetType() == typeof(ValidateDeviceSession)));

            var validateDeviceSessions = result.RequestCollection.Values.Cast<ValidateDeviceSession>().ToList();
            CollectionAssert.AreEqual(validateSessionForDeviceListRequest.Imsis, validateDeviceSessions.Select(x => x.SearchData.SearchCollection.Imsi).ToList());
            Assert.IsTrue(validateDeviceSessions.Select(x => x.SessionType).All(x => x == 2));
        }

        [TestMethod]
        public void Test_CreateAddImsiToSubscriberRequest()
        {
            var addImsiToSubscriberRequest = new AddImsiToSubscriberRequest
            {
                SubscriberExternalId = Guid.NewGuid(),
                NewImsi = "204070000000001"
            };

            var result = _device.CreateAddImsiToSubscriberRequest(addImsiToSubscriberRequest);
            Assert.AreEqual(2, result.RequestCollection.Values.Count);
            Assert.AreEqual(1, result.RequestCollection.Values.Count(x => x.GetType() == typeof(CreateDeviceRequest)));
            Assert.AreEqual(1, result.RequestCollection.Values.Count(x => x.GetType() == typeof(SubscriberAddDeviceRequest)));

            var createDeviceRequest = (CreateDeviceRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(CreateDeviceRequest));
            Assert.AreEqual(addImsiToSubscriberRequest.NewImsi, createDeviceRequest.MobileDeviceExtensions.MobileDeviceExtension.Imsi);

            var subscriberAddDeviceRequest = (SubscriberAddDeviceRequest)result.RequestCollection.Values.First(x => x.GetType() == typeof(SubscriberAddDeviceRequest));
            Assert.AreEqual(addImsiToSubscriberRequest.SubscriberExternalId.ToString().ToUpper(), subscriberAddDeviceRequest.SubscriberSearchData.SearchCollection.ExternalId);
        }

        [TestMethod]
        public void Test_CreateAddImsisRequest()
        {
            var swapSimRequest = new SwapSimRequest
            {
                CrmProductId = Guid.NewGuid(),
                Imsis = new List<string>
                {
                    "204070000000001",
                    "204070000000002"
                }
            };

            var msisdns = new List<string>
            {
                "31637000001",
                "31637000002"
            };

            var result = _device.CreateAddImsisRequest(swapSimRequest, msisdns);

            Assert.AreEqual(swapSimRequest.Imsis.Count * 2, result.RequestCollection.Values.Count, "Number of requests does not match");
            Assert.AreEqual(swapSimRequest.Imsis.Count, result.RequestCollection.Values.Count(x => x.GetType() == typeof(CreateDeviceRequest)), "Number of CreateDeviceRequests does not match");

            var createDeviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(CreateDeviceRequest)).Cast<CreateDeviceRequest>().ToList();
            CollectionAssert.AreEqual(swapSimRequest.Imsis, createDeviceRequests.Select(x => x.MobileDeviceExtensions.MobileDeviceExtension.Imsi).ToList(), "Imsi list has not been correctly translated");
            CollectionAssert.AreEqual(msisdns, createDeviceRequests[0].MobileDeviceExtensions.MobileDeviceExtension.AccessNumberList.Values, "Msisdn list has not been correctly translated");
            Assert.IsNull(createDeviceRequests[1].MobileDeviceExtensions.MobileDeviceExtension.AccessNumberList, "AccessNumber list should be null");

            Assert.AreEqual(swapSimRequest.Imsis.Count, result.RequestCollection.Values.Count(x => x.GetType() == typeof(SubscriberAddDeviceRequest)), "Number of SubscriberAddDeviceRequests does not match");

            var subscriberAddDeviceRequests = result.RequestCollection.Values.Where(x => x.GetType() == typeof(SubscriberAddDeviceRequest)).Cast<SubscriberAddDeviceRequest>().ToList();

            for (var i = 0; i < swapSimRequest.Imsis.Count; i++)
            {
                Assert.AreEqual(swapSimRequest.CrmProductId.ToString().ToUpper(), subscriberAddDeviceRequests[i].SubscriberSearchData.SearchCollection.ExternalId, "CrmProductId has not been correctly translated");
                Assert.AreEqual((i * 2).ToString(), subscriberAddDeviceRequests[i].DeviceSearchData.SearchCollection.MultiRequestIndex, "MultiRequestIndex of SubscriberAddDeviceRequest is incorrect");
            }
        }

        [TestMethod]
        public void Test_BuildSwapMsIsdnRequest()
        {
            var deviceQueryResponse = new DeviceQueryResponse
            {
                MobileDeviceExtensionCollection = new MobileDeviceExtensionCollection
                {
                    MobileDeviceExtension = new MobileDeviceExtension
                    {
                        AccessNumberList = new StringValueCollection
                        {
                            Values = new List<string>
                            {
                                "31637000001", "31637000002"
                            }
                        },
                        Imsi = "204070000000001"
                    }
                }
            };
            var swapMsIsdnRequest = new SwapMsIsdnRequest
            {
                OldMsIsdn = "31637000001",
                NewMsIsdn = "31637000003"
            };

            var result = _device.BuildSwapMsIsdnRequest(deviceQueryResponse, swapMsIsdnRequest);

            Assert.AreEqual("204070000000001", result.DeviceSearchData.SearchCollection.Imsi);
            CollectionAssert.AreEqual(new List<string> { "31637000003", "31637000002" }, result.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList.Values);

        }

        [TestMethod]
        public void Test_BuildUpdateMsisdnListRequest()
        {
            var deviceQueryResponse = new DeviceQueryResponse
            {
                MobileDeviceExtensionCollection = new MobileDeviceExtensionCollection
                {
                    MobileDeviceExtension = new MobileDeviceExtension
                    {
                        AccessNumberList = new StringValueCollection
                        {
                            Values = new List<string>
                            {
                                "31637000001", "31637000002"
                            }
                        },
                        Imsi = "204070000000001"
                    }
                }
            };
            var updateMsisdnListRequest = new UpdateMsisdnListRequest
            {
                NewMsIsdns = new List<string> { "31637000003", "31637000004" }
            };

            var result = _device.BuildUpdateMsisdnListRequest(deviceQueryResponse, updateMsisdnListRequest);
            Assert.AreEqual("204070000000001", result.DeviceSearchData.SearchCollection.Imsi);
            CollectionAssert.AreEqual(updateMsisdnListRequest.NewMsIsdns, result.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList.Values);
        }

        [TestMethod]
        public void Test_CreateDeviceDeleteRequestList()
        {
            var oldImsis = new List<string>
            {
                "20407000000001",
                "20407000000002"
            };

            var result = _device.CreateDeviceDeleteRequestList(oldImsis);
            Assert.AreEqual(2, result.RequestCollection.Values.Count);
            Assert.AreEqual(2, result.RequestCollection.Values.Count(x=>x.GetType() == typeof(DeviceDeleteRequest)));

            var deviceDeleteRequests = result.RequestCollection.Values.Cast<DeviceDeleteRequest>().ToList();
            CollectionAssert.AreEqual(oldImsis, deviceDeleteRequests.Select(x=>x.DeviceSearchData.SearchCollection.Imsi).ToList());
        }
    }
}
