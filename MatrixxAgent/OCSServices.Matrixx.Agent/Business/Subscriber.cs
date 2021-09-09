using System.Collections.Generic;
using System.Linq;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Model;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Subscriber : BaseMessageBuilder, ISubscriber
    {
        public ExternalIdQueryParameters GetSubscriberRequest(string identifier)
        {
            return new ExternalIdQueryParameters(identifier);
        }

        public MultiRequest BuildCreateSubscriberRequest(CreateSubscriberRequest request)
        {
            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection
                {
                    Values = new List<MatrixxObject>
                    {
                        new api.CreateSubscriberRequest
                        {
                            Status = request.CreateWithStatus,
                            ExternalId = request.CrmProductId.ToString().ToUpper(),
                            ContactPhoneNumber = request.MsisdnList.First(),
                            FirstName = request.ImsiList.FirstOrDefault()
                        }

                    }
                }
            };

            CreateAddDevicesForImsis(result.RequestCollection.Values, request);

            CreateAttachDevicesToSubscriber(result.RequestCollection.Values, request);

            CreateBillingCycleRequest(result.RequestCollection.Values, request);
            
            CreateAddGroupMembership(result.RequestCollection.Values, request);
            
            return result;
        }

        private void CreateAddGroupMembership(List<MatrixxObject> requestList, CreateSubscriberRequest request)
        {
            if (string.IsNullOrEmpty(request.GroupCode))
            {

                requestList.AddRange(request.MembershipCodes.Select(membershipCode => new AddGroupMembershipRequest
                {
                    GroupSearchData = new GroupSearchData
                    {
                        SearchCollection = new SearchCollection
                        {
                            ExternalId = membershipCode
                        }
                    },
                    GroupMembers = new SubscriberSearchArray
                    {
                        SearchCollection = new SearchCollection { MultiRequestIndex = "0" }
                    }
                }));
            } else
            {
                requestList.Add(new AddGroupMembershipRequest
                {
                    GroupSearchData = new GroupSearchData
                    {
                        SearchCollection = new SearchCollection
                        {
                            ExternalId = request.GroupCode
                        }
                    },
                    GroupMembers = new SubscriberSearchArray
                    {
                        SearchCollection = new SearchCollection { MultiRequestIndex = "0" }
                    }
                });
            }
        }

        private void CreateBillingCycleRequest(List<MatrixxObject> requestList, CreateSubscriberRequest request)
        {
            if (request.BillingCycleId != null)
            {
                requestList.Add(new api.ModifySubscriberRequest
                {
                    SearchData = new SubscriberSearchData
                    {
                        SearchCollection = new SearchCollection { MultiRequestIndex = "0" }
                    },
                    BillingCycleConfigurations = new BillingCycleCollection
                    {
                        BillingCycle = new BillingCycle
                        {
                            BillingCycleId = request.BillingCycleId,
                            DateOffset = request.BillingDateOffset //only works with BillingCycleId 500
                        },
                    }
                });
            }
        }

        private void CreateAttachDevicesToSubscriber(List<MatrixxObject> requestList, CreateSubscriberRequest request)
        {
            for (int imsiIndex = 0; imsiIndex < request.ImsiList.Count; imsiIndex++)
            {
                requestList.Add(new api.SubscriberAddDeviceRequest
                {
                    SubscriberSearchData = new SubscriberSearchData
                    {
                        SearchCollection = new SearchCollection { MultiRequestIndex = "0" },
                    },
                    DeviceSearchData = new DeviceSearchData
                    {
                        SearchCollection = new SearchCollection { MultiRequestIndex = (imsiIndex + 1).ToString() }
                    }
                });
            }
        }

        private void CreateAddDevicesForImsis(List<MatrixxObject> requestList, CreateSubscriberRequest request)
        {
            requestList.Add(new CreateDeviceRequest
            {
                MobileDeviceExtensions = new MobileDeviceExtensionCollection
                {
                    MobileDeviceExtension = new MobileDeviceExtension
                    {
                        Imsi = request.ImsiList[0],
                        AccessNumberList = new StringValueCollection
                        {
                            Values = request.MsisdnList
                        }
                    }
                }
            });

            for (int imsiIndex = 1; imsiIndex < request.ImsiList.Count; imsiIndex++)
            {
                requestList.Add(new CreateDeviceRequest
                {
                    MobileDeviceExtensions = new MobileDeviceExtensionCollection
                    {
                        MobileDeviceExtension = new MobileDeviceExtension
                        {
                            Imsi = request.ImsiList[imsiIndex]
                        }
                    }
                });
            }
        }

        private CustomSubscriberConfiguration SetCustomSubscriberConfiguration(Dictionary<string, string> customConfigurationParameters)
        {
            if (customConfigurationParameters == null || customConfigurationParameters.Count < 1)
                return null;

            CustomSubscriberConfiguration customSubscriberConfiguration = new CustomSubscriberConfiguration();

            string contractTypeKey = nameof(customSubscriberConfiguration.ContractType);
            if (customConfigurationParameters.ContainsKey(contractTypeKey))
                customSubscriberConfiguration.ContractType = customConfigurationParameters[contractTypeKey];

            customSubscriberConfiguration.Configuration = customConfigurationParameters.Where(dict => !dict.Equals(contractTypeKey)).ToDictionary(d => d.Key, d => d.Value);
            return customSubscriberConfiguration;
        }

        public MultiRequest BuildSetCustomSubscriberConfigurationRequest(SetCustomConfigurationRequest request)
        {
            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection
                {
                    Values = new List<MatrixxObject>
                    {
                        new api.ModifySubscriberRequest
                        {
                            SearchData = new SubscriberSearchData
                            {
                                SearchCollection = new SearchCollection
                                {
                                    ExternalId = request.CrmProductId.ToString().ToUpper()
                                }
                            },
                            CustomSubscriberConfigurations = new CustomSubscriberConfigurationCollection()
                            {
                                CustomSubscriberConfiguration = SetCustomSubscriberConfiguration(request.CustomConfigurationParameters)
                            }
                        }
                    }
                }
            };

            return result;
        }

        public MultiRequest BuildSetStatusRequest(SetSubscriberStatusRequest request)
        {
            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection
                {
                    Values = new List<MatrixxObject>
                    {
                        new api.ModifySubscriberRequest
                        {
                            SearchData = new SubscriberSearchData
                            {
                                SearchCollection = new SearchCollection
                                {
                                    ExternalId = request.CrmProductId.ToString().ToUpper()
                                }
                            },
                            Status = request.Status
                        }
                    }
                }
            };

            return result;
        }

        public api.CreateSubscriberRequest BuildCreateGroupAdmin(CreateGroupAdminRequest request)
        {
            return new api.CreateSubscriberRequest
            {
                Status = request.SubscriberCreateStatus,
                ExternalId = request.GroupCode
            };
        }

        public MultiRequest CreateDetachImsiFromSubscriber(DetachImsiFromSubscriberRequest request)
        {
            List<MatrixxObject> detachRequests = new List<MatrixxObject>();

            foreach (var imsi in request.Imsis)
            {
                detachRequests.Add(new api.SubscriberRemoveDeviceRequest
                {
                    SubscriberSearchData = new SubscriberSearchData
                    {
                        SearchCollection = new SearchCollection
                        {
                            ExternalId = request.CrmProductId.ToString().ToUpper()
                        }
                    },
                    DeviceSearchData = new DeviceSearchData()
                    {
                        SearchCollection = new SearchCollection()
                        {
                            Imsi = imsi
                        }
                    },
                    DeleteSession = true
                });
                detachRequests.Add(
                    new DeviceModifyRequest
                    {
                        DeviceSearchData = new DeviceSearchData
                        {
                            SearchCollection = new SearchCollection
                            {
                                Imsi = imsi
                            }
                        },
                        Status = 2
                    });
            }

            var result = new MultiRequest()
            {
                RequestCollection = new RequestCollection()
                {
                    Values = detachRequests
                }
            };

            return result;
        }

        public MultiRequest CreateModifySubscriberFirstNameRequest(SetSubscriberFirstNameRequest request)
        {
            List<MatrixxObject> modifySubscriberFirstNameRequest = new List<MatrixxObject>();

            modifySubscriberFirstNameRequest.Add(new api.ModifySubscriberRequest
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                },
                FirstName = request.FirstName
            });

            var result = new MultiRequest
            {
                RequestCollection = new RequestCollection
                {
                    Values = modifySubscriberFirstNameRequest
                }
            };

            return result;
        }

        public api.ModifySubscriberRequest CreateModifySubscriberContactPhoneNumberRequest(UpdateContactPhoneNumberRequest request)
        {
            api.ModifySubscriberRequest response = new api.ModifySubscriberRequest
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                },
                ContactPhoneNumber = request.PrimaryMsisdn
            };

            return response;
        }

        public MultiRequest CreateDeleteSubscriberRequest(DeactiveSubscriberRequest request)
        {
            List<MatrixxObject> deleteSubscriberRequests = new List<MatrixxObject>();
            deleteSubscriberRequests.Add(new api.DeleteSubscriberRequest
            {
                SearchData = new SubscriberSearchData()
                {
                    SearchCollection = new SearchCollection()
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                },
                Delete = true
            });

            var result = new MultiRequest()
            {
                RequestCollection = new RequestCollection()
                {
                    Values = deleteSubscriberRequests
                }
            };

            foreach (var imsi in request.Imsis)
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
