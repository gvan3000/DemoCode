using System.Collections.Generic;
using OCSServices.Matrixx.Agent.Contracts.Device;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Model;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using System;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Agent.Contracts.Sim.Swap;
using OCSServices.Matrixx.Agent.Business.Interfaces;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Device : BaseMessageBuilder, IDevice
    {
        public MultiRequest CreateValidateDeviceListSession(ValidateSessionForDeviceListRequest request)
        {
            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection()
                {
                    Values = new List<MatrixxObject>()
                }
            };

            foreach (var imsi in request.Imsis)
            {
                var collectionItem = CreateValidateDeviceSession(imsi, request.SessionType);
                result.RequestCollection.Values.Add(collectionItem);
            }

            return result;
        }

        private ValidateDeviceSession CreateValidateDeviceSession(string imsi, int sessionType)
        {
            return new ValidateDeviceSession
            {
                SearchData = new DeviceSearchData()
                {
                    SearchCollection = new SearchCollection()
                    {
                        Imsi = imsi
                    }
                },
                SessionType = sessionType
            };
        }

        public MultiRequest CreateAddImsiToSubscriberRequest(AddImsiToSubscriberRequest request)
        {
            List<MatrixxObject> createRequests = new List<MatrixxObject>();

            createRequests.Add(
                new CreateDeviceRequest
                {
                    MobileDeviceExtensions = new MobileDeviceExtensionCollection
                    {
                        MobileDeviceExtension = new MobileDeviceExtension
                        {
                            Imsi = request.NewImsi
                        }
                    }
                });
            createRequests.Add(
                new SubscriberAddDeviceRequest
                {
                    SubscriberSearchData = new SubscriberSearchData
                    {
                        SearchCollection = new SearchCollection
                        {
                            ExternalId = request.SubscriberExternalId.ToString().ToUpper()
                        }
                    },
                    DeviceSearchData = new DeviceSearchData
                    {
                        SearchCollection = new SearchCollection
                        {
                            MultiRequestIndex = "0"
                        }
                    }
                });
            
            var result = new MultiRequest()
            {
                RequestCollection = new RequestCollection()
                {
                    Values = createRequests
                }
            };

            return result;
        }

        public MultiRequest CreateAddImsisRequest(SwapSimRequest request, List<string> msisdns)
        {
            List<MatrixxObject> createRequests = new List<MatrixxObject>();

            foreach (var imsi in request.Imsis)
            {
                createRequests.Add(
                    new CreateDeviceRequest
                    {
                        MobileDeviceExtensions = new MobileDeviceExtensionCollection
                        {
                            MobileDeviceExtension = new MobileDeviceExtension
                            {
                                Imsi = imsi,
                                AccessNumberList =
                                createRequests.Count == 0 ?
                                new StringValueCollection
                                {
                                    Values = msisdns
                                }
                                : null
                            }
                        }
                    }
                    );
                createRequests.Add(
                    new SubscriberAddDeviceRequest
                    {
                        SubscriberSearchData = new SubscriberSearchData
                        {
                            SearchCollection = new SearchCollection
                            {
                                ExternalId = request.CrmProductId.ToString().ToUpper()
                            }
                        },
                        DeviceSearchData = new DeviceSearchData
                        {
                            SearchCollection = new SearchCollection
                            {
                                MultiRequestIndex = (createRequests.Count - 1).ToString()
                            }
                        }
                    }
                    );
            }

            var result = new MultiRequest()
            {
                RequestCollection = new RequestCollection()
                {
                    Values = createRequests
                }
            };

            return result;
        }

        public MsisdnDeviceQueryParameters GetMsisdnDeviceQueryParameters(string identifier)
        {
            return new MsisdnDeviceQueryParameters(identifier);
        }

        public DeviceSessionIdParameters GetDeviceSessionsQueryParameters(string deviceId)
        {
            return new DeviceSessionIdParameters(deviceId);
        }

        public DeviceModifyRequest BuildSwapMsIsdnRequest(DeviceQueryResponse request, SwapMsIsdnRequest swapRequest)
        {
            List<string> _msisdnList = request.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList.Values;
            _msisdnList[_msisdnList.FindIndex(ind => ind.Equals(swapRequest.OldMsIsdn))] = swapRequest.NewMsIsdn;

            var result = new DeviceModifyRequest
            {
                DeviceSearchData = new DeviceSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        Imsi = request.MobileDeviceExtensionCollection.MobileDeviceExtension.Imsi
                    }
                },

                MobileDeviceExtensionCollection = new MobileDeviceExtensionCollection
                {
                    MobileDeviceExtension = new MobileDeviceExtension
                    {
                        AccessNumberList = new StringValueCollection
                        {
                            Values = new List<string>(_msisdnList)
                        }
                    }
                }

            };

            return result;
        }

        public DeviceModifyRequest BuildUpdateMsisdnListRequest(DeviceQueryResponse request, UpdateMsisdnListRequest updateMsisdnListRequest)
        {
            var _msisdnList = request.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList.Values;

            var newMsisdnList = updateMsisdnListRequest.NewMsIsdns;

            var result = new DeviceModifyRequest
            {
                DeviceSearchData = new DeviceSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        Imsi = request.MobileDeviceExtensionCollection.MobileDeviceExtension.Imsi
                    }
                },

                MobileDeviceExtensionCollection = new MobileDeviceExtensionCollection
                {
                    MobileDeviceExtension = new MobileDeviceExtension
                    {
                        AccessNumberList = new StringValueCollection
                        {
                            Values = new List<string>(newMsisdnList)
                        }
                    }
                }

            };

            return result;
        }
        public ImsiDeviceQueryParameters GetImsiDeviceQueryParameters(string identifier)
        {
            return new ImsiDeviceQueryParameters(identifier);
        }

        public MultiRequest CreateDeviceDeleteRequestList(List<string> oldImsis)
        {
            var result = new MultiRequest()
            {
                RequestCollection = new RequestCollection()
                {
                    Values = new List<MatrixxObject>()
                }
            };

            foreach (var imsi in oldImsis)
            {
                var collectionItem = CreateDeviceDeleteRequest(imsi);
                result.RequestCollection.Values.Add(collectionItem);
            };

            return result;
        }

        private DeviceDeleteRequest CreateDeviceDeleteRequest(string imsi)
        {
            return new DeviceDeleteRequest
            {
                DeviceSearchData = new DeviceSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        Imsi = imsi
                    }
                }
            };
        }
    }
}
