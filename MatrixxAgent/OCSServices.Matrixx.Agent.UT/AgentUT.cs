using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts;
using OCSServices.Matrixx.Agent.Contracts.Balance;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Agent.Contracts.Offer;
using OCSServices.Matrixx.Agent.Contracts.Threshold;
using OCSServices.Matrixx.Agent.Contracts.Wallet;
using OCSServices.Matrixx.Api.Client.ApiClient;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber;
using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.UT
{
    [TestClass]
    public class AgentUT
    {
        private Mock<IMessageBuilderUnitOfWork> _messageBuilderUnitOfWork;

        private Mock<ILog> _logger;

        private Mock<IClient> _client;

        private Agent _agentUnderTest;

        int? _code;
        string _text;
        Mock<IQueryParameters> _queryParametersMocked;
        int _resourceId;
        string _templateId;
        string _name;
        string _availableAmount;
        string _thresholdLimit;
        string _quantityUnit;
        DateTime _startTime;
        DateTime _endTime;

        string _externalId;

        DeviceSessionIdParameters _deviceSessionIdParameters;
        ResponseDeviceSession _responseDeviceSession;

        [TestInitialize]
        public void SetupTests()
        {
            SetupVariable();

            _queryParametersMocked = new Mock<IQueryParameters>();


            _logger = new Mock<ILog>(MockBehavior.Strict);
            _client = new Mock<IClient>(MockBehavior.Strict);



            _client.Setup(x => x.OfferProxy.ModifyOfferForGroup(It.IsAny<Api.Client.Contracts.Request.ProductOffer.ModifyOfferForGroupRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.MatrixxResponse { Code = _code, Text = _text });
            _client.Setup(x => x.OfferProxy.ModifyOfferForSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.ModifyOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.MatrixxResponse { Code = _code, Text = _text });
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Subscriber.SubscriberQueryResponse
                {
                    Result = 1,
                    ResultText = "abc 123",
                    BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection
                    {
                        Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>
                        {
                            new Api.Client.Contracts.Model.Balance.BalanceInfo
                            {
                                ResourceId = _resourceId,
                                TemplateId = _templateId,
                                Name = _name,
                                AvailableAmount = _availableAmount,
                                ThresholdLimit = _thresholdLimit,
                                QuantityUnit = _quantityUnit,
                                StartTime = _startTime,
                                EndTime = _endTime
                            }
                        }
                    }
                });
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<SplitProvisioning.Base.Data.Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.BalanceProxy.SubscriberSetThresholdToInfinity(It.IsAny<Api.Client.Contracts.Request.Balance.SubscriberSetThresholdToInfinityRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });
            _client.Setup(x => x.BalanceProxy.SubscriberAddThreshold(It.IsAny<OCSServices.Matrixx.Api.Client.Contracts.Request.Balance.SubscriberAddThresholdRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.BalanceProxy.SubscriberAddThreshold(It.IsAny<OCSServices.Matrixx.Api.Client.Contracts.Request.Balance.SubscriberAddThresholdRequest>(),
                                                                     It.IsAny<SplitProvisioning.Base.Data.Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.BalanceProxy.GroupAddThreshold(It.IsAny<GroupAddThresholdRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.GroupProxy.CreateGroupAdmin(It.IsAny<CreateSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.GroupProxy.GroupModify(It.IsAny<ModifyGroupRequest>()))
                .ReturnsAsync(new MatrixxResponse() { Code = _code, Text = _text });

            _client.Setup(x => x.GroupProxy.PurchaseGroupOffer(It.IsAny<PurchaseGroupOfferRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new AddOfferToSubscriberResponse()
                {
                    DeviceList = new Api.Client.Contracts.Model.StringValueCollection()
                    {
                        Values = new List<string>() { "dev1", "dev2" }
                    },
                    ObjectId = "ObjectId",
                    PurchaseInfoList = new PurchaseInfoCollection()
                    {
                        Values = new List<PurchaseInfo>()
                            {
                               new  PurchaseInfo()
                               {
                                     ResourceId = 1
                               }
                            }
                    }
                }
                );

            _responseDeviceSession = new ResponseDeviceSession
            {
                Result = 0,
                ChargingSessionCollection = new Api.Client.Contracts.Model.Device.ChargingSessionCollection
                {
                    Values = new List<Api.Client.Contracts.Model.Device.ChargingSessionInfo>
                    {
                        new Api.Client.Contracts.Model.Device.ChargingSessionInfo
                        {
                            ContextArray = new Api.Client.Contracts.Model.Device.ContextArray
                            {
                                Values = new List<Api.Client.Contracts.Model.Device.SessionContextInfo>
                                {
                                    new Api.Client.Contracts.Model.Device.SessionContextInfo
                                    {
                                        AuthQuantity = 15,
                                        QuantityUnit = 200,
                                        ServiceId = 1111111
                                    },
                                    new Api.Client.Contracts.Model.Device.SessionContextInfo
                                    {
                                        AuthQuantity = 10,
                                        QuantityUnit = 102,
                                        ServiceId = 1111112                                       
                                    }
                                }
                            },
                            SessionStartTime = DateTime.UtcNow
                        },
                        new Api.Client.Contracts.Model.Device.ChargingSessionInfo
                        {
                            ContextArray = new Api.Client.Contracts.Model.Device.ContextArray
                            {
                                Values = new List<Api.Client.Contracts.Model.Device.SessionContextInfo>
                                {
                                    new Api.Client.Contracts.Model.Device.SessionContextInfo
                                    {
                                        AuthQuantity = 14,
                                        QuantityUnit = 203,
                                        ServiceId = 1102425
                                    },
                                    new Api.Client.Contracts.Model.Device.SessionContextInfo
                                    {
                                        AuthQuantity = 106,
                                        QuantityUnit = 201,
                                        ServiceId = 123
                                    }
                                }
                            },
                            SessionStartTime = DateTime.UtcNow
                        }
                    }
                }
            };

            _client.Setup(x => x.DeviceProxy.DeviceSessionQuery(It.IsAny<DeviceSessionIdParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(_responseDeviceSession);
          
            _messageBuilderUnitOfWork = new Mock<IMessageBuilderUnitOfWork>();

            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildModifyOfferForGroupRequest(It.IsAny<ModifyOfferForGroupRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.ModifyOfferForGroupRequest());
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildModifyOfferForSubscriberRequest(It.IsAny<ModifyOfferForSubscriberRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.ModifyOfferForSubscriberRequest());
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Returns(_queryParametersMocked.Object);

            _messageBuilderUnitOfWork.Setup(x => x.Group.BuildCreateGroupRequest(It.IsAny<AddGroupRequest>()))
                .Returns(new Api.Client.Contracts.Request.MultiRequest());

            _messageBuilderUnitOfWork.Setup(x => x.Group.GetGroupRequest(It.IsAny<string>()))
                .Returns(new GroupIdQueryParameters(_externalId));

            _messageBuilderUnitOfWork.Setup(x => x.Threshold.BuildSubscriberSetThresholdToInfinityRequest(It.IsAny<SetThresholdSubscriberToInfinityRequest>()))
                .Returns(new Api.Client.Contracts.Request.Balance.SubscriberSetThresholdToInfinityRequest());

            _messageBuilderUnitOfWork.Setup(x => x.Threshold.BuildSubscriberAddThresholdRequest(It.IsAny<AddThresholdToSubscriberRequest>()))
                .Returns(new Api.Client.Contracts.Request.Balance.SubscriberAddThresholdRequest());

            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildCreateGroupAdmin(It.IsAny<CreateGroupAdminRequest>()))
                .Returns(new CreateSubscriberRequest());

            _messageBuilderUnitOfWork.Setup(x => x.Wallet.GetQueryWalletRequest(It.IsAny<GetWalletRequest>()))
                .Returns(new Api.Client.Contracts.Request.Wallet.WalletQueryRequest());

            _deviceSessionIdParameters = new DeviceSessionIdParameters("123");

            _agentUnderTest = new Agent(_client.Object, _messageBuilderUnitOfWork.Object);
        }

        [TestMethod]
        public async Task AddThresholdToSubscriber_ShouldCall_MessageBuilderUnitOfWork_Threshold_BuildSubscriberAddThresholdRequest()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var endpointId = 1;
            var splitProvisioningEndpointHLR1 = new Endpoint()
            {
                Enabled = 1,
                EndpointAttributes = new List<EndpointAttribute>(),
                EndpointUrl = "EnddpointUrl",
                EndpointType = EndpointType.HLR,
                Id = endpointId,
                Name = "Endpoint-HLR-1"
            };

            int requestResourceId = 1;
            var request = new AddThresholdToSubscriberRequest()
            {
                Amount = 100,
                CrmProductId = productId,
                Endpoint = splitProvisioningEndpointHLR1,
                ResourceId = requestResourceId
            };

            //Act
            var response = await _agentUnderTest.AddThresholdToSubscriber(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Threshold.BuildSubscriberAddThresholdRequest(It.IsAny<AddThresholdToSubscriberRequest>()));
        }

        [TestMethod]
        public async Task AddThresholdToSubscriber_ShouldCall_Client_BalanceProxy_OneParam_When_EndpointIsNull()
        {
            //Arrange
            var productId = Guid.NewGuid();

            int requestResourceId = 1;
            var request = new AddThresholdToSubscriberRequest()
            {
                Amount = 100,
                CrmProductId = productId,
                Endpoint = null,
                ResourceId = requestResourceId
            };

            //Act
            var response = await _agentUnderTest.AddThresholdToSubscriber(request).ConfigureAwait(false);

            //Assert
            _client.Verify(x => x.BalanceProxy.SubscriberAddThreshold(It.IsAny<OCSServices.Matrixx.Api.Client.Contracts.Request.Balance.SubscriberAddThresholdRequest>(), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task AddThresholdToSubscriber_ShouldCall_Client_BalanceProxy_When_EndpointIsNotNull()
        {
            //Arrange
            var productId = Guid.NewGuid();


            var endpointId = 1;
            var splitProvisioningEndpointHLR1 = new Endpoint()
            {
                Enabled = 1,
                EndpointAttributes = new List<EndpointAttribute>(),
                EndpointUrl = "EnddpointUrl",
                EndpointType = EndpointType.HLR,
                Id = endpointId,
                Name = "Endpoint-HLR-1"
            };

            int requestResourceId = 1;
            var request = new AddThresholdToSubscriberRequest()
            {
                Amount = 100,
                CrmProductId = productId,
                Endpoint = splitProvisioningEndpointHLR1,
                ResourceId = requestResourceId
            };


            //Act
            var response = await _agentUnderTest.AddThresholdToSubscriber(request).ConfigureAwait(false);

            //Assert
            _client.Verify(x => x.BalanceProxy.SubscriberAddThreshold(It.IsAny<OCSServices.Matrixx.Api.Client.Contracts.Request.Balance.SubscriberAddThresholdRequest>(),
                                                                      It.IsAny<SplitProvisioning.Base.Data.Endpoint>()),
                                                                      Times.Once);
        }

        private void SetupVariable()
        {
            _code = 123;
            _text = "123123123 adsgadfg a";

            _templateId = "12fwdf";
            _resourceId = 1112;
            _startTime = DateTime.Now.AddDays(-2);
            _endTime = DateTime.Now;
            _quantityUnit = "eragrga234";
            _thresholdLimit = "5555";
            _name = "good name 111";
            _availableAmount = "many many 123";

            _externalId = "externalId";
        }

        [TestMethod]
        public async Task AddThresholdToSubscriber_ShouldReturn_BasicResponse_ProperlyMappedData()
        {
            //Arrange
            var productId = Guid.NewGuid();


            var endpointId = 1;
            var splitProvisioningEndpointHLR1 = new Endpoint()
            {
                Enabled = 1,
                EndpointAttributes = new List<EndpointAttribute>(),
                EndpointUrl = "EnddpointUrl",
                EndpointType = EndpointType.HLR,
                Id = endpointId,
                Name = "Endpoint-HLR-1"
            };

            int requestResourceId = 1;
            var request = new AddThresholdToSubscriberRequest()
            {
                Amount = 100,
                CrmProductId = productId,
                Endpoint = splitProvisioningEndpointHLR1,
                ResourceId = requestResourceId
            };

            //Act
            var response = await _agentUnderTest.AddThresholdToSubscriber(request).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task ModifyOfferGroup_ShouldCall_MessageBuilderUnitOfWork_Offer_BuildModifyOfferForGroupRequest()
        {
            var response = await _agentUnderTest.ModifyOfferGroup(new ModifyOfferForGroupRequest()).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Offer.BuildModifyOfferForGroupRequest(It.IsAny<ModifyOfferForGroupRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task ModifyOfferGroup_ShouldCall_Client_OfferProxy_ModifyOfferForGroup()
        {
            var response = await _agentUnderTest.ModifyOfferGroup(new ModifyOfferForGroupRequest()).ConfigureAwait(false);

            _client.Verify(x => x.OfferProxy.ModifyOfferForGroup(It.IsAny<Api.Client.Contracts.Request.ProductOffer.ModifyOfferForGroupRequest>(), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task ModifyOfferGroup_ShouldReturn_BasicResponse_MessageFilledWithCorrespondingData()
        {
            var response = await _agentUnderTest.ModifyOfferGroup(new ModifyOfferForGroupRequest()).ConfigureAwait(false);

            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task ModifyOfferSubscriber_ShouldCall_MessageBuilderUnitOfWork_Offer_BuildModifyOfferForSubscriberRequest()
        {
            var response = await _agentUnderTest.ModifyOfferSubscriber(new ModifyOfferForSubscriberRequest()).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Offer.BuildModifyOfferForSubscriberRequest(It.IsAny<ModifyOfferForSubscriberRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task ModifyOfferSubscriber_ShouldCall_Client_OfferProxy_ModifyOfferForSubscriber_With_MatrixxRequest_When_RequestEndpointIsNull()
        {
            var response = await _agentUnderTest.ModifyOfferSubscriber(new ModifyOfferForSubscriberRequest { Endpoint = null }).ConfigureAwait(false);

            _client.Verify(x => x.OfferProxy.ModifyOfferForSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.ModifyOfferForSubscriberRequest>(), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task ModifyOfferSubscriber_ShouldCall_Client_OfferProxy_ModifyOfferForSubscriber_With_MatrixxRequest_And_Endpoint_When_RequestContainsEndpoint()
        {
            ModifyOfferForSubscriberRequest request = new ModifyOfferForSubscriberRequest
            {
                Endpoint = new Endpoint
                {
                    Name = "abc"
                }
            };

            var response = await _agentUnderTest.ModifyOfferSubscriber(request).ConfigureAwait(false);

            _client.Verify(x => x.OfferProxy.ModifyOfferForSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.ModifyOfferForSubscriberRequest>(),
                                                                       It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task ModifyOfferSubscriber_Should_Return_BasicResponse_MappedWithCorrespondingData()
        {
            var response = await _agentUnderTest.ModifyOfferSubscriber(new ModifyOfferForSubscriberRequest { Endpoint = null }).ConfigureAwait(false);

            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldCall_MessageBuilderUnitOfWork_Balance_GetQueryBalanceParametersByProductId_When_RequestContainsProductId()
        {
            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = Guid.NewGuid()
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldCall_MessageBuilderUnitOfWork_Balance_GetQueryBalanceParametersByMsisdn_When_RequestDoNotContainProductId()
        {
            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = null
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldCall_Client_SubscriberProxy_SubscriberQuery()
        {
            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = null
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            _client.Verify(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldReturn_QueryBalanceResponse_MappedWithCorrespondingData()
        {
            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = Guid.NewGuid()
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            Assert.AreEqual(_templateId, response.BalanceList[0].TemplateId);
            Assert.AreEqual(_resourceId, response.BalanceList[0].ResourceId);
            Assert.AreEqual(_availableAmount, response.BalanceList[0].Amount);
            Assert.AreEqual(_name, response.BalanceList[0].Name);
            Assert.AreEqual(_thresholdLimit, response.BalanceList[0].TresholdLimit);
            Assert.AreEqual(_startTime, response.BalanceList[0].StartTime);
            Assert.AreEqual(_endTime, response.BalanceList[0].EndTime);
            Assert.AreEqual(_resourceId, response.BalanceList[0].ResourceId);
            Assert.AreEqual(_quantityUnit, response.BalanceList[0].Unit);
        }

        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldReturn_Null_When_Client_SubscriberProxy_SubscriberQuery_ReturnsNull()
        {
            Api.Client.Contracts.Response.Subscriber.SubscriberQueryResponse clientResponse = null;
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(clientResponse);

            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = Guid.NewGuid()
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            Assert.IsNull(response);
        }

        [TestMethod]
        public async Task AddGroup_ShouldCall_MessageBuilderUnitOfWork_Group_BuildCreateGroupRequest()
        {
            var response = await _agentUnderTest.AddGroup(new AddGroupRequest()).ConfigureAwait(false);
            _messageBuilderUnitOfWork.Verify(x => x.Group.BuildCreateGroupRequest(It.IsAny<AddGroupRequest>()), Times.Once);
        }

        [TestMethod]
        public async Task AddGroup_ShouldCall_Client_MultiProxy_RequestMulti()
        {
            var response = await _agentUnderTest.AddGroup(new AddGroupRequest()).ConfigureAwait(false);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        public async Task AddGroup_ShouldReturn_MultiResponse_MessageFilledWihtCorrspondingData()
        {
            var response = await _agentUnderTest.AddGroup(new AddGroupRequest()).ConfigureAwait(false);

            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task AddThresholdToGroup_ShouldReturnResponseProperlyMapped_WhenAddThresholdToGroupRequest()
        {
            //Arrange
            var request = new AddThresholdToGroupRequest()
            {
                Amount = 1,
                BusinessUnitId = Guid.NewGuid(),
                GroupCode = "Group Code",
                ResourceId = 1,
                ThresholdId = 1
            };

            //Act
            var response = await _agentUnderTest.AddThresholdToGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Threshold.BuildGroupAddThresholdRequest(It.IsAny<AddThresholdToGroupRequest>()), Times.Once);
            _client.Verify(x => x.BalanceProxy.GroupAddThreshold(It.IsAny<GroupAddThresholdRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }



        [TestMethod]
        public async Task QueryBalanceSharedSubscriber_ShouldReturn_QueryBalanceResponse_BalanceList_Null_When_SubscriberQueryResponse_BalanceInfoList_IsNull()
        {
            Api.Client.Contracts.Response.Subscriber.SubscriberQueryResponse clientResponse = new Api.Client.Contracts.Response.Subscriber.SubscriberQueryResponse
            {
                BalanceInfoList = null,
                Result = 1,
                ResultText = "abc 0071"
            };

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(clientResponse);

            Contracts.Balance.QueryBalanceRequest request = new Contracts.Balance.QueryBalanceRequest
            {
                ProductId = Guid.NewGuid()
            };

            var response = await _agentUnderTest.QueryBalanceSharedSubscriber(request).ConfigureAwait(false);

            Assert.IsNull(response.BalanceList);
        }

        [TestMethod]
        public async Task AdjustBalanceGroup_ShouldReturnBasicResponseMapped_WhenAdjustBalanceForGroupRequestAndEndpointIsNull()
        {
            //Arrange
            var request = new AdjustBalanceForGroupRequest();

            //Act
            var response = await _agentUnderTest.AdjustBalanceGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetGroupAdjustBalanceRequest(It.IsAny<AdjustBalanceForGroupRequest>()), Times.Once);
            _client.Verify(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()), Times.Once);


            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task AdjustBalanceGroup_ShouldReturnBasicResponseMapped_WhenAdjustBalanceForGroupRequestAndEndpointIsNotNull()
        {
            //Arrange
            var request = new AdjustBalanceForGroupRequest()
            {
                Endpoint = new Endpoint()
                {
                    Name = "Endpoint name"
                }
            };

            //Act
            var response = await _agentUnderTest.AdjustBalanceGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetGroupAdjustBalanceRequest(It.IsAny<AdjustBalanceForGroupRequest>()), Times.Once);
            _client.Verify(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()), Times.Once);


            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task CreateGroupAdmin_ShouldReturnBasicResponseMapped_WhenCreateGroupAdminRequest()
        {
            //Arrange
            var matrixxRequest = new CreateGroupAdminRequest();

            //Act
            var matrixxResponse = await _agentUnderTest.CreateGroupAdmin(matrixxRequest).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.BuildCreateGroupAdmin(It.IsAny<CreateGroupAdminRequest>()), Times.Once);
            _client.Verify(x => x.GroupProxy.CreateGroupAdmin(It.IsAny<CreateSubscriberRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsInstanceOfType(matrixxResponse, typeof(BasicResponse));
            Assert.AreEqual(_code, matrixxResponse.Code);
            Assert.AreEqual(_text, matrixxResponse.Text);
        }

        [TestMethod]
        public async Task ModifyGroup_ShouldReturnBasicResponseMapped_WhenUpdateGroupRequest()
        {
            //Arrange
            var request = new UpdateGroupRequest();

            //Act
            var matrixxResponse = await _agentUnderTest.ModifyGroup(request).ConfigureAwait(false);


            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Group.BuildModifyGroupRequest(It.IsAny<UpdateGroupRequest>()), Times.Once);
            _client.Verify(x => x.GroupProxy.GroupModify(It.IsAny<ModifyGroupRequest>()), Times.Once);

            Assert.IsInstanceOfType(matrixxResponse, typeof(BasicResponse));
            Assert.AreEqual(_code, matrixxResponse.Code);
            Assert.AreEqual(_text, matrixxResponse.Text);
        }

        [TestMethod]
        public async Task AddOfferToGroup_ShouldReturnAddOfferToSubscriberResponse_WhenAddOfferToGroupRequest()
        {
            //Arrange
            var request = new AddOfferToGroupRequest()
            {
                CustomPurchaseOfferConfigurationParameters = new Dictionary<string, string>()
                {
                    { "offerConfig1Key", "offerConfig2Value" },
                    { "offerConfig2Key", "offerConfig2Value" }
                },
                OfferCode = "offerCode1",
                ExternalId = "externalId"
            };

            //Act
            var matrixxResponse = await _agentUnderTest.AddOfferToGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Group.BuidPurchaseGroupOfferRequest(It.IsAny<AddOfferToGroupRequest>()), Times.Once);
            _client.Verify(x => x.GroupProxy.PurchaseGroupOffer(It.IsAny<PurchaseGroupOfferRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsInstanceOfType(matrixxResponse, typeof(AddOfferToSubscriberResponse));
        }

     

        [TestMethod]
        public async Task QueryWallet_ShouldReturnQueryWalletResponse_WhenGetWalletRequestAndEnpointSupplied()
        {
            //Arrange
            string msisdn = "9012345678989";
            Guid productId = Guid.NewGuid();

            var request = new GetWalletRequest()
            {
                Endpoint = new Endpoint()
                {
                    Name = "Endpoint1"
                },
                MsIsdn = msisdn,
                ProductId = productId
            };

            var subscriberWalletResponseResult = 1;
            var subscriberWalletResponseResultText = "subscriberWalletResponseResultText";

            //BillCycleInfo
            var billCycleInfoBillingCycleId = 1;
            var billCycleInfoCurrentPeriodDuration = 2;
            var billCycleInfoCurrentPeriodEndTime = DateTime.UtcNow.AddDays(20);
            var billCycleInfoCurrentPeriodOffset = 3;
            var billCycleInfoCurrentPeriodStartTime = DateTime.UtcNow;
            var billCycleInfoDateOffset = 4;
            var billCycleInfoDatePolicy = 5;
            var billCycleInfoPeriod = 6;
            var billCycleInfoPeriodInterval = 7;

            //WalletInfo
            var walletInfoAmount = "amount";
            var walletInfoAvailableAmount = "AvailableAmount";
            var walletInfoCategory = 11;
            var walletInfoClassId = "ClassId";
            var walletInfoClassName = "ClassName";
            var walletInfoCreditLimit = "CreditLimit";
            var walletInfoEndTime = DateTime.UtcNow.AddDays(21);
            var walletInfoIsAggregate = false;
            var walletInfoIsPeriodic = true;
            var walletInfoIsPrepaid = true;
            var walletInfoIsVirtual = true;
            var walletInfoName = "Name";
            var walletInfoQuantityUnit = "QuantityUnit";
            var walletInfoReservedAmount = "ReservedAmount";
            var walletInfoResourceId = 12;
            var walletInfoStartTime = DateTime.UtcNow;
            var walletInfoTemplateId = "TemplateId";
            var walletInfoThresholdLimit = "ThresholdLimit";

            var walletInfo1 = new Api.Client.Contracts.Model.Wallet.WalletInfo()
            {
                Amount = walletInfoAmount,
                AvailableAmount = walletInfoAvailableAmount,
                Category = walletInfoCategory,
                ClassId = walletInfoClassId,
                ClassName = walletInfoClassName,
                CreditLimit = walletInfoCreditLimit,
                EndTime = walletInfoEndTime,
                IsAggregate = walletInfoIsAggregate,
                IsPeriodic = walletInfoIsPeriodic,
                IsPrepaid = walletInfoIsPrepaid,
                IsVirtual = walletInfoIsVirtual,
                Name = walletInfoName,
                QuantityUnit = walletInfoQuantityUnit,
                ReservedAmount = walletInfoReservedAmount,
                ResourceId = walletInfoResourceId,
                StartTime = walletInfoStartTime,
                TemplateId = walletInfoTemplateId,
                ThresholdLimit = walletInfoThresholdLimit
            };

            //WalletInfoPeriodicCollection - WalletInfo
            var walletInfo2Amount = "Amount2";
            var walletInfo2AvailableAmount = "AvailableAmount2";
            var walletInfo2Category = 21;
            var walletInfo2ClassId = "ClassId2";
            var walletInfo2ClassName = "ClassName2";
            var walletInfo2CreditLimit = "CreditLimit2";
            var walletInfo2EndTime = DateTime.UtcNow.AddDays(19);
            var walletInfo2IsAggregate = true;
            var walletInfo2IsPeriodic = true;
            var walletInfo2IsPrepaid = true;
            var walletInfo2IsVirtual = true;
            var walletInfo2Name = "Name2";
            var walletInfo2QuantityUnit = "QuantityUnit2";
            var walletInfo2ReservedAmount = "ReservedAmount2";
            var walletInfo2ResourceId = 22;
            var walletInfo2StartTime = DateTime.UtcNow;
            var walletInfo2TemplateId = "TemplateId2";
            var walletInfo2ThresholdLimit = "ThresholdLimit2";

            var walletInfo2 = new Api.Client.Contracts.Model.Wallet.WalletInfo()
            {
                Amount = walletInfo2Amount,
                AvailableAmount = walletInfo2AvailableAmount,
                Category = walletInfo2Category,
                ClassId = walletInfo2ClassId,
                ClassName = walletInfo2ClassName,
                CreditLimit = walletInfo2CreditLimit,
                EndTime = walletInfo2EndTime,
                IsAggregate = walletInfo2IsAggregate,
                IsPeriodic = walletInfo2IsPeriodic,
                IsPrepaid = walletInfo2IsPrepaid,
                IsVirtual = walletInfo2IsVirtual,
                Name = walletInfo2Name,
                QuantityUnit = walletInfo2QuantityUnit,
                ReservedAmount = walletInfo2ReservedAmount,
                ResourceId = walletInfo2ResourceId,
                StartTime = walletInfo2StartTime,
                TemplateId = walletInfo2TemplateId,
                ThresholdLimit = walletInfo2ThresholdLimit
            };

            var subscriberWalletResponse = new Api.Client.Contracts.Response.Wallet.SubscriberWalletResponse()
            {
                Result = subscriberWalletResponseResult,
                ResultText = subscriberWalletResponseResultText,
                WalletInfoBillingCycleList = new Api.Client.Contracts.Model.Wallet.WalletInfoBillingCycleCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.BillCycleInfo>()
                            {
                                 new Api.Client.Contracts.Model.Wallet.BillCycleInfo()
                                 {
                                      BillingCycleId = billCycleInfoBillingCycleId,
                                      CurrentPeriodDuration = billCycleInfoCurrentPeriodDuration,
                                      CurrentPeriodEndTime = billCycleInfoCurrentPeriodEndTime,
                                      CurrentPeriodOffset = billCycleInfoCurrentPeriodOffset,
                                      CurrentPeriodStartTime = billCycleInfoCurrentPeriodStartTime,
                                      DateOffset = billCycleInfoDateOffset,
                                      DatePolicy = billCycleInfoDatePolicy,
                                      Period = billCycleInfoPeriod,
                                      PeriodInterval = billCycleInfoPeriodInterval
                                 }
                            }
                },
                WalletInfoList = new Api.Client.Contracts.Model.Wallet.WalletInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>() { walletInfo1 }
                },
                WalletInfoPeriodicList = new Api.Client.Contracts.Model.Wallet.WalletInfoPeriodicCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>() { walletInfo2 }
                }
            };


            _client.Setup(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(subscriberWalletResponse);

            //Act
            var matrixxResponse = await _agentUnderTest.QueryWallet(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Wallet.GetQueryWalletRequest(It.IsAny<GetWalletRequest>()), Times.Once);
            _client.Verify(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsInstanceOfType(matrixxResponse, typeof(QueryWalletResponse));
            Assert.AreEqual(subscriberWalletResponseResult.ToString(), matrixxResponse.ErrorCode);
            Assert.AreEqual(subscriberWalletResponseResultText, matrixxResponse.ErrorMessage);
            Assert.AreEqual(1, matrixxResponse.SimpleBalanceList.Count);
            var simpleBalance = matrixxResponse.SimpleBalanceList[0];
            Assert.AreEqual(walletInfoAvailableAmount, simpleBalance.Amount);
            Assert.AreEqual(walletInfoEndTime, simpleBalance.EndTime);
            Assert.AreEqual(walletInfoName, simpleBalance.Name);
            Assert.AreEqual(walletInfoReservedAmount, simpleBalance.ReservedAmount);
            Assert.AreEqual(walletInfoResourceId, simpleBalance.ResourceId);
            Assert.AreEqual(walletInfoStartTime, simpleBalance.StartTime);
            Assert.AreEqual(walletInfoTemplateId, simpleBalance.TemplateId);
            Assert.AreEqual(walletInfoThresholdLimit, simpleBalance.TresholdLimit);
            Assert.AreEqual(walletInfoQuantityUnit, simpleBalance.Unit);

            //PeriodicAggregateBalanceList
            Assert.AreEqual(1, matrixxResponse.PeriodicAggregateBalanceList.Count);
            var periodicAggregateBalance = matrixxResponse.PeriodicAggregateBalanceList[0];
            Assert.AreEqual(walletInfo2AvailableAmount, periodicAggregateBalance.Amount);
            Assert.AreEqual(walletInfo2EndTime, periodicAggregateBalance.EndTime);
            Assert.AreEqual(walletInfo2Name, periodicAggregateBalance.Name);
            Assert.AreEqual(walletInfo2ReservedAmount, periodicAggregateBalance.ReservedAmount);
            Assert.AreEqual(walletInfo2ResourceId, periodicAggregateBalance.ResourceId);
            Assert.AreEqual(walletInfo2StartTime, periodicAggregateBalance.StartTime);
            Assert.AreEqual(walletInfo2TemplateId, periodicAggregateBalance.TemplateId);
            Assert.AreEqual(walletInfo2ThresholdLimit, periodicAggregateBalance.TresholdLimit);
            Assert.AreEqual(walletInfo2QuantityUnit, periodicAggregateBalance.Unit);

            //WalletInfoBillingCycleList
            Assert.AreEqual(1, matrixxResponse.MatrixxBillCycleList.Count);
            var matrixxBillCycle = matrixxResponse.MatrixxBillCycleList[0];
            Assert.AreEqual(billCycleInfoBillingCycleId, matrixxBillCycle.BillingCycleId);
            Assert.AreEqual(billCycleInfoCurrentPeriodDuration, matrixxBillCycle.CurrentPeriodDuration);
            Assert.AreEqual(billCycleInfoCurrentPeriodEndTime, matrixxBillCycle.CurrentPeriodEndTime);
            Assert.AreEqual(billCycleInfoCurrentPeriodOffset, matrixxBillCycle.CurrentPeriodOffset);
            Assert.AreEqual(billCycleInfoCurrentPeriodStartTime, matrixxBillCycle.CurrentPeriodStartTime);
            Assert.AreEqual(billCycleInfoDateOffset, matrixxBillCycle.DateOffset);
            Assert.AreEqual(billCycleInfoPeriod, matrixxBillCycle.Period);
            Assert.AreEqual(billCycleInfoPeriodInterval, matrixxBillCycle.PeriodInterval);
        }

        [TestMethod]
        public async Task QueryWallet_ShouldReturnQueryWalletResponse_WhenGetWalletRequestAndEnpointNOTSupplied()
        {
            //Arrange
            string msisdn = "9012345678989";
            Guid productId = Guid.NewGuid();

            var request = new GetWalletRequest()
            {
                MsIsdn = msisdn,
                ProductId = productId
            };

            var subscriberWalletResponseResult = 1;
            var subscriberWalletResponseResultText = "subscriberWalletResponseResultText";

            //BillCycleInfo
            var billCycleInfoBillingCycleId = 1;
            var billCycleInfoCurrentPeriodDuration = 2;
            var billCycleInfoCurrentPeriodEndTime = DateTime.UtcNow.AddDays(20);
            var billCycleInfoCurrentPeriodOffset = 3;
            var billCycleInfoCurrentPeriodStartTime = DateTime.UtcNow;
            var billCycleInfoDateOffset = 4;
            var billCycleInfoDatePolicy = 5;
            var billCycleInfoPeriod = 6;
            var billCycleInfoPeriodInterval = 7;

            //WalletInfo
            var walletInfoAmount = "amount";
            var walletInfoAvailableAmount = "AvailableAmount";
            var walletInfoCategory = 11;
            var walletInfoClassId = "ClassId";
            var walletInfoClassName = "ClassName";
            var walletInfoCreditLimit = "CreditLimit";
            var walletInfoEndTime = DateTime.UtcNow.AddDays(21);
            var walletInfoIsAggregate = false;
            var walletInfoIsPeriodic = true;
            var walletInfoIsPrepaid = true;
            var walletInfoIsVirtual = true;
            var walletInfoName = "Name";
            var walletInfoQuantityUnit = "QuantityUnit";
            var walletInfoReservedAmount = "ReservedAmount";
            var walletInfoResourceId = 12;
            var walletInfoStartTime = DateTime.UtcNow;
            var walletInfoTemplateId = "TemplateId";
            var walletInfoThresholdLimit = "ThresholdLimit";

            var walletInfo1 = new Api.Client.Contracts.Model.Wallet.WalletInfo()
            {
                Amount = walletInfoAmount,
                AvailableAmount = walletInfoAvailableAmount,
                Category = walletInfoCategory,
                ClassId = walletInfoClassId,
                ClassName = walletInfoClassName,
                CreditLimit = walletInfoCreditLimit,
                EndTime = walletInfoEndTime,
                IsAggregate = walletInfoIsAggregate,
                IsPeriodic = walletInfoIsPeriodic,
                IsPrepaid = walletInfoIsPrepaid,
                IsVirtual = walletInfoIsVirtual,
                Name = walletInfoName,
                QuantityUnit = walletInfoQuantityUnit,
                ReservedAmount = walletInfoReservedAmount,
                ResourceId = walletInfoResourceId,
                StartTime = walletInfoStartTime,
                TemplateId = walletInfoTemplateId,
                ThresholdLimit = walletInfoThresholdLimit
            };

            //WalletInfoPeriodicCollection - WalletInfo
            var walletInfo2Amount = "Amount2";
            var walletInfo2AvailableAmount = "AvailableAmount2";
            var walletInfo2Category = 21;
            var walletInfo2ClassId = "ClassId2";
            var walletInfo2ClassName = "ClassName2";
            var walletInfo2CreditLimit = "CreditLimit2";
            var walletInfo2EndTime = DateTime.UtcNow.AddDays(19);
            var walletInfo2IsAggregate = true;
            var walletInfo2IsPeriodic = true;
            var walletInfo2IsPrepaid = true;
            var walletInfo2IsVirtual = true;
            var walletInfo2Name = "Name2";
            var walletInfo2QuantityUnit = "QuantityUnit2";
            var walletInfo2ReservedAmount = "ReservedAmount2";
            var walletInfo2ResourceId = 22;
            var walletInfo2StartTime = DateTime.UtcNow;
            var walletInfo2TemplateId = "TemplateId2";
            var walletInfo2ThresholdLimit = "ThresholdLimit2";

            var walletInfo2 = new Api.Client.Contracts.Model.Wallet.WalletInfo()
            {
                Amount = walletInfo2Amount,
                AvailableAmount = walletInfo2AvailableAmount,
                Category = walletInfo2Category,
                ClassId = walletInfo2ClassId,
                ClassName = walletInfo2ClassName,
                CreditLimit = walletInfo2CreditLimit,
                EndTime = walletInfo2EndTime,
                IsAggregate = walletInfo2IsAggregate,
                IsPeriodic = walletInfo2IsPeriodic,
                IsPrepaid = walletInfo2IsPrepaid,
                IsVirtual = walletInfo2IsVirtual,
                Name = walletInfo2Name,
                QuantityUnit = walletInfo2QuantityUnit,
                ReservedAmount = walletInfo2ReservedAmount,
                ResourceId = walletInfo2ResourceId,
                StartTime = walletInfo2StartTime,
                TemplateId = walletInfo2TemplateId,
                ThresholdLimit = walletInfo2ThresholdLimit
            };

            var subscriberWalletResponse = new Api.Client.Contracts.Response.Wallet.SubscriberWalletResponse()
            {
                Result = subscriberWalletResponseResult,
                ResultText = subscriberWalletResponseResultText,
                WalletInfoBillingCycleList = new Api.Client.Contracts.Model.Wallet.WalletInfoBillingCycleCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.BillCycleInfo>()
                            {
                                 new Api.Client.Contracts.Model.Wallet.BillCycleInfo()
                                 {
                                      BillingCycleId = billCycleInfoBillingCycleId,
                                      CurrentPeriodDuration = billCycleInfoCurrentPeriodDuration,
                                      CurrentPeriodEndTime = billCycleInfoCurrentPeriodEndTime,
                                      CurrentPeriodOffset = billCycleInfoCurrentPeriodOffset,
                                      CurrentPeriodStartTime = billCycleInfoCurrentPeriodStartTime,
                                      DateOffset = billCycleInfoDateOffset,
                                      DatePolicy = billCycleInfoDatePolicy,
                                      Period = billCycleInfoPeriod,
                                      PeriodInterval = billCycleInfoPeriodInterval
                                 }
                            }
                },
                WalletInfoList = new Api.Client.Contracts.Model.Wallet.WalletInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>() { walletInfo1 }
                },
                WalletInfoPeriodicList = new Api.Client.Contracts.Model.Wallet.WalletInfoPeriodicCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>() { walletInfo2 }
                }
            };


            _client.Setup(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(subscriberWalletResponse);

            //Act
            var matrixxResponse = await _agentUnderTest.QueryWallet(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Wallet.GetQueryWalletRequest(It.IsAny<GetWalletRequest>()), Times.Once);
            _client.Verify(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsInstanceOfType(matrixxResponse, typeof(QueryWalletResponse));
            Assert.AreEqual(subscriberWalletResponseResult.ToString(), matrixxResponse.ErrorCode);
            Assert.AreEqual(subscriberWalletResponseResultText, matrixxResponse.ErrorMessage);
            Assert.AreEqual(1, matrixxResponse.SimpleBalanceList.Count);
            var simpleBalance = matrixxResponse.SimpleBalanceList[0];
            Assert.AreEqual(walletInfoAvailableAmount, simpleBalance.Amount);
            Assert.AreEqual(walletInfoEndTime, simpleBalance.EndTime);
            Assert.AreEqual(walletInfoName, simpleBalance.Name);
            Assert.AreEqual(walletInfoReservedAmount, simpleBalance.ReservedAmount);
            Assert.AreEqual(walletInfoResourceId, simpleBalance.ResourceId);
            Assert.AreEqual(walletInfoStartTime, simpleBalance.StartTime);
            Assert.AreEqual(walletInfoTemplateId, simpleBalance.TemplateId);
            Assert.AreEqual(walletInfoThresholdLimit, simpleBalance.TresholdLimit);
            Assert.AreEqual(walletInfoQuantityUnit, simpleBalance.Unit);

            //PeriodicAggregateBalanceList
            Assert.AreEqual(1, matrixxResponse.PeriodicAggregateBalanceList.Count);
            var periodicAggregateBalance = matrixxResponse.PeriodicAggregateBalanceList[0];
            Assert.AreEqual(walletInfo2AvailableAmount, periodicAggregateBalance.Amount);
            Assert.AreEqual(walletInfo2EndTime, periodicAggregateBalance.EndTime);
            Assert.AreEqual(walletInfo2Name, periodicAggregateBalance.Name);
            Assert.AreEqual(walletInfo2ReservedAmount, periodicAggregateBalance.ReservedAmount);
            Assert.AreEqual(walletInfo2ResourceId, periodicAggregateBalance.ResourceId);
            Assert.AreEqual(walletInfo2StartTime, periodicAggregateBalance.StartTime);
            Assert.AreEqual(walletInfo2TemplateId, periodicAggregateBalance.TemplateId);
            Assert.AreEqual(walletInfo2ThresholdLimit, periodicAggregateBalance.TresholdLimit);
            Assert.AreEqual(walletInfo2QuantityUnit, periodicAggregateBalance.Unit);

            //WalletInfoBillingCycleList
            Assert.AreEqual(1, matrixxResponse.MatrixxBillCycleList.Count);
            var matrixxBillCycle = matrixxResponse.MatrixxBillCycleList[0];
            Assert.AreEqual(billCycleInfoBillingCycleId, matrixxBillCycle.BillingCycleId);
            Assert.AreEqual(billCycleInfoCurrentPeriodDuration, matrixxBillCycle.CurrentPeriodDuration);
            Assert.AreEqual(billCycleInfoCurrentPeriodEndTime, matrixxBillCycle.CurrentPeriodEndTime);
            Assert.AreEqual(billCycleInfoCurrentPeriodOffset, matrixxBillCycle.CurrentPeriodOffset);
            Assert.AreEqual(billCycleInfoCurrentPeriodStartTime, matrixxBillCycle.CurrentPeriodStartTime);
            Assert.AreEqual(billCycleInfoDateOffset, matrixxBillCycle.DateOffset);
            Assert.AreEqual(billCycleInfoPeriod, matrixxBillCycle.Period);
            Assert.AreEqual(billCycleInfoPeriodInterval, matrixxBillCycle.PeriodInterval);
        }

        [TestMethod]
        public async Task QueryWallet_ShouldReturnNull_WhenGetWalletRequestAndClientWalletProxyQueryWalletReturnsNull()
        {
            //Arrange
            string msisdn = "9012345678989";
            Guid productId = Guid.NewGuid();

            var request = new GetWalletRequest()
            {
                Endpoint = new Endpoint()
                {
                    Name = "Endpoint1"
                },
                MsIsdn = msisdn,
                ProductId = productId
            };

            var subscriberWalletResponse = new Api.Client.Contracts.Response.Wallet.SubscriberWalletResponse();

            _client.Setup(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(default(Api.Client.Contracts.Response.Wallet.SubscriberWalletResponse));

            //Act
            var matrixxResponse = await _agentUnderTest.QueryWallet(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Wallet.GetQueryWalletRequest(It.IsAny<GetWalletRequest>()), Times.Once);
            _client.Verify(x => x.WalletProxy.QueryWallet(It.IsAny<WalletQueryRequest>(), It.IsAny<Endpoint>()), Times.Once);
            Assert.AreEqual(matrixxResponse, default(QueryWalletResponse));
        }

        [TestMethod]
        public async Task QueryBalance_ShouldCallGetQueryBalanceParametersByProductIdAndReturnQueryGroupResponseMappedAndCall_WhenQueryBalanceRequestHasProductIdValue()
        {
            //Arrange
            Guid productId = Guid.NewGuid();
            var request = new QueryBalanceRequest()
            {
                Msisdn = "901234567890",
                ProductId = productId
            };

            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == productId)))
                .Returns(_queryParametersMocked.Object);

            string purchasedOfferInfo1_ProductOfferExternalId = "ProductOfferExternalId-1";
            DateTime purchasedOfferInfo1_purchaseTime = DateTime.UtcNow.AddSeconds(1);
            DateTime purchasedOfferInfo1_endTime = purchasedOfferInfo1_purchaseTime.AddDays(20);
            int? purchasedOfferInfo1_status = 1;
            int purchasedOfferInfo1_ResourceId = 1;

            string purchasedOfferInfo2_ProductOfferExternalId = "ProductOfferExternalId-2";
            DateTime purchasedOfferInfo2_purchaseTime = DateTime.UtcNow.AddSeconds(2);
            DateTime purchasedOfferInfo2_endTime = purchasedOfferInfo1_purchaseTime.AddDays(20);
            int? purchasedOfferInfo2_status = 1;
            int purchasedOfferInfo2_ResourceId = 2;

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
               .ReturnsAsync(new SubscriberQueryResponse()
               {
                   Result = 0,
                   ResultText = string.Empty,
                   PurchaseInfoList = new PurchasedOfferInfoCollection()
                   {
                       Values = new List<PurchasedOfferInfo>()
                       {
                           new PurchasedOfferInfo()
                           {
                               ProductOfferExternalId = purchasedOfferInfo1_ProductOfferExternalId,
                               ProductOfferId = 1,
                               ProductOfferVersion = 1,
                               PurchaseTime = purchasedOfferInfo1_purchaseTime,
                               ResourceId = purchasedOfferInfo1_ResourceId,
                               EndTime = purchasedOfferInfo1_endTime,
                               Status = purchasedOfferInfo1_status
                           },
                           new PurchasedOfferInfo()
                           {
                               ProductOfferExternalId = purchasedOfferInfo2_ProductOfferExternalId,
                               ProductOfferId = 2,
                               ProductOfferVersion = 2,
                               PurchaseTime = purchasedOfferInfo2_purchaseTime,
                               ResourceId = purchasedOfferInfo2_ResourceId,
                               EndTime = purchasedOfferInfo2_endTime,
                               Status = purchasedOfferInfo2_status
                           }
                       }
                   },
                   BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection
                   {
                       Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>
                        {
                                        new Api.Client.Contracts.Model.Balance.BalanceInfo
                                        {
                                            ResourceId = _resourceId,
                                            TemplateId = _templateId,
                                            Name = _name,
                                            AvailableAmount = _availableAmount,
                                            ThresholdLimit = _thresholdLimit,
                                            QuantityUnit = _quantityUnit,
                                            StartTime = _startTime,
                                            EndTime = _endTime
                                        }
                        }
                   }
               });

            //Act
            var matrixxResponse = await _agentUnderTest.QueryBalance(request).ConfigureAwait(false);

            //Assert
            Assert.IsInstanceOfType(matrixxResponse, typeof(QueryBalanceResponse));
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByProductId(productId), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<String>()), Times.Never);
            _client.Verify(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.AreEqual(1, matrixxResponse.BalanceList.Count);
            var balanceInfo = matrixxResponse.BalanceList[0];

            Assert.AreEqual(_resourceId, balanceInfo.ResourceId);
            Assert.AreEqual(_templateId, balanceInfo.TemplateId);
            Assert.AreEqual(_name, balanceInfo.Name);
            Assert.AreEqual(_availableAmount, balanceInfo.Amount);
            Assert.AreEqual(_quantityUnit, balanceInfo.Unit);
            Assert.AreEqual(_thresholdLimit, balanceInfo.TresholdLimit);
            Assert.AreEqual(_startTime, balanceInfo.StartTime);
            Assert.AreEqual(_endTime, balanceInfo.EndTime);
        }

        [TestMethod]
        public async Task QueryBalance_ShouldCallGetQueryBalanceParametersByMsisdnAndReturnQueryGroupResponseMappedAndCall_WhenQueryBalanceRequestHasNOTProductIdValue()
        {
            //Arrange
            var request = new QueryBalanceRequest()
            {
                Msisdn = "901234567890"
            };

            //Act
            var matrixxResponse = await _agentUnderTest.QueryBalance(request).ConfigureAwait(false);

            //Assert
            Assert.IsInstanceOfType(matrixxResponse, typeof(QueryBalanceResponse));
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<String>()), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()), Times.Never);
            _client.Verify(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()), Times.Once);


            Assert.AreEqual(1, matrixxResponse.BalanceList.Count);
            var balanceInfo = matrixxResponse.BalanceList[0];

            Assert.AreEqual(_resourceId, balanceInfo.ResourceId);
            Assert.AreEqual(_templateId, balanceInfo.TemplateId);
            Assert.AreEqual(_name, balanceInfo.Name);
            Assert.AreEqual(_availableAmount, balanceInfo.Amount);
            Assert.AreEqual(_quantityUnit, balanceInfo.Unit);
            Assert.AreEqual(_thresholdLimit, balanceInfo.TresholdLimit);
            Assert.AreEqual(_startTime, balanceInfo.StartTime);
            Assert.AreEqual(_endTime, balanceInfo.EndTime);
        }


        [TestMethod]
        public async Task AdjustBalanceSubscriber_ShouldReturnBasicResponseMapped_WhenAdjustBalanceForGroupRequestAndEndpointIsNull()
        {
            //Arrange
            var request = new AdjustBalanceForGroupRequest();

            //Act
            var response = await _agentUnderTest.AdjustBalanceGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetGroupAdjustBalanceRequest(It.IsAny<AdjustBalanceForGroupRequest>()), Times.Once);
            _client.Verify(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()), Times.Once);


            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task AdjustBalanceSubscriber_ShouldReturnBasicResponseMapped_WhenAdjustBalanceForGroupRequestAndEndpointIsNotNull()
        {
            //Arrange
            var request = new AdjustBalanceForGroupRequest()
            {
                Endpoint = new Endpoint()
                {
                    Name = "Endpoint name"
                }
            };

            //Act
            var response = await _agentUnderTest.AdjustBalanceGroup(request).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Balance.GetGroupAdjustBalanceRequest(It.IsAny<AdjustBalanceForGroupRequest>()), Times.Once);

            _client.Verify(x => x.BalanceProxy.GroupAdjustBalance(It.IsAny<GroupAdjustBalanceRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }


        [TestMethod]
        public async Task SetThresholdToSubscriberToInfinity_ShouldReturnBasicResponseMapped_WhenSetThresholdSubscriberToInfinityRequest()
        {
            //Act
            var response = await _agentUnderTest.SetThresholdToSubscriberToInfinity(new SetThresholdSubscriberToInfinityRequest()).ConfigureAwait(false);

            //Assert
            _messageBuilderUnitOfWork.Verify(x => x.Threshold.BuildSubscriberSetThresholdToInfinityRequest(It.IsAny<SetThresholdSubscriberToInfinityRequest>()), Times.Once);
            _client.Verify(x => x.BalanceProxy.SubscriberSetThresholdToInfinity(It.IsAny<SubscriberSetThresholdToInfinityRequest>(), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsInstanceOfType(response, typeof(BasicResponse));
            Assert.AreEqual(_code, response.Code);
            Assert.AreEqual(_text, response.Text);
        }

        [TestMethod]
        public async Task CancelOfferForGroup_ShouldUseBuilderToConstructRequestCallClientAndReturnResponse()
        {
            var builderReturn = new Api.Client.Contracts.Request.ProductOffer.CancelOfferForGroupRequest();
            var matrixxResponse = new MatrixxResponse()
            {
                Code = 123,
                Text = "bla"
            };
            var input = new Contracts.Requests.CancelOfferForGroupRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForGroupRequest(It.IsAny<Contracts.Requests.CancelOfferForGroupRequest>()))
                .Returns(builderReturn);
            _client.Setup(x => x.OfferProxy.CancelOfferForGroup(It.IsAny<Api.Client.Contracts.Request.ProductOffer.CancelOfferForGroupRequest>()))
                .ReturnsAsync(matrixxResponse);

            var result = await _agentUnderTest.CancelOfferForGroup(input).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Offer.BuildCancelOfferForGroupRequest(It.Is<Contracts.Requests.CancelOfferForGroupRequest>(i => i == input)), Times.Once);
            _client.Verify(x => x.OfferProxy.CancelOfferForGroup(It.Is<Api.Client.Contracts.Request.ProductOffer.CancelOfferForGroupRequest>(rq => rq == builderReturn)), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(matrixxResponse.Text, result.Text);
            Assert.AreEqual(matrixxResponse.Code, result.Code);
        }

        public class TestException : Exception { }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CancelOfferForGroup_ShouldThrowIfMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForGroupRequest(It.IsAny<Contracts.Requests.CancelOfferForGroupRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.CancelOfferForGroup(new Contracts.Requests.CancelOfferForGroupRequest())
                .ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CancelOfferForGroup_ShouldThrowIfClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForGroupRequest(It.IsAny<Contracts.Requests.CancelOfferForGroupRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.CancelOfferForGroupRequest());
            _client.Setup(x => x.OfferProxy.CancelOfferForGroup(It.IsAny<Api.Client.Contracts.Request.ProductOffer.CancelOfferForGroupRequest>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.CancelOfferForGroup(new Contracts.Requests.CancelOfferForGroupRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CancelOfferForSubscriber_ShouldUseBuilderToConstructRequestCallClientAndReturnResult()
        {
            var builderReturn = new Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest();
            var matrixxResponse = new MatrixxResponse()
            {
                Code = 456,
                Text = "bla123"
            };
            var input = new Contracts.Requests.CancelOfferForSubscriberRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForSubscriberRequest(It.IsAny<Contracts.Requests.CancelOfferForSubscriberRequest>()))
                .Returns(builderReturn);
            _client.Setup(x => x.OfferProxy.CancelOfferForSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(matrixxResponse);

            var result = await _agentUnderTest.CancelOfferForSubscriber(input).ConfigureAwait(false);

            _messageBuilderUnitOfWork.Verify(x => x.Offer.BuildCancelOfferForSubscriberRequest(It.Is<Contracts.Requests.CancelOfferForSubscriberRequest>(i => i == input)), Times.Once);
            _client.Verify(x => x.OfferProxy.CancelOfferForSubscriber(It.Is<Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest>(rq => rq == builderReturn), It.IsAny<Endpoint>()), Times.Once);

            Assert.IsNotNull(result);
            Assert.AreEqual(matrixxResponse.Text, result.Text);
            Assert.AreEqual(matrixxResponse.Code, result.Code);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CancelOfferForSubscriber_ShouldThrowWhenBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForSubscriberRequest(It.IsAny<Contracts.Requests.CancelOfferForSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.CancelOfferForSubscriber(new Contracts.Requests.CancelOfferForSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CancelOfferForSubscriber_ShouldThrowWhenApiClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildCancelOfferForSubscriberRequest(It.IsAny<Contracts.Requests.CancelOfferForSubscriberRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest());
            _client.Setup(x => x.OfferProxy.CancelOfferForSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.CancelOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.CancelOfferForSubscriber(new Contracts.Requests.CancelOfferForSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task DeleteSubscriberOnMatrixx_ShouldUseOverloadsThatSupportsSplitProvisioningIfInputContainsExplicitEndpoint()
        {
            var setSubscriberRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Returns(setSubscriberRequest);
            var detachImsiRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(detachImsiRequest);
            var deactivateRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDeleteSubscriberRequest(It.IsAny<Contracts.Subscriber.DeactiveSubscriberRequest>()))
                .Returns(deactivateRequest);

            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse() { Code = 789, Text = "bla", ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection() { ResponseList = new List<MatrixxObject>() } });

            var input = new Contracts.Subscriber.DeactiveSubscriberRequest()
            {
                Endpoint = new Endpoint(),
                CrmProductId = Guid.NewGuid(),
                Status = 123,
                Imsis = new List<string>() { "123456789" }
            };

            var result = await _agentUnderTest.DeleteSubscriberOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(789, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.BuildSetStatusRequest(It.Is<Contracts.Subscriber.SetSubscriberStatusRequest>(rq => rq.Status == input.Status && rq.CrmProductId == input.CrmProductId)), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.Is<Contracts.Imsi.DetachImsiFromSubscriberRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.Imsis == input.Imsis)), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.CreateDeleteSubscriberRequest(It.Is<Contracts.Subscriber.DeactiveSubscriberRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.Status == input.Status && rq.Imsis == input.Imsis)), Times.Once);

            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == setSubscriberRequest), It.Is<Endpoint>(e => e == input.Endpoint)), Times.Once);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == detachImsiRequest), It.Is<Endpoint>(e => e == input.Endpoint)), Times.Once);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deactivateRequest), It.Is<Endpoint>(e => e == input.Endpoint)), Times.Once);
        }

        [TestMethod]
        public async Task DeleteSubscriberOnMatrixx_ShouldUpdateSubscriberStatusDetachImsiFromSubscriberAndDeleteSubscriber()
        {
            var setSubscriberRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Returns(setSubscriberRequest);
            var detachImsiRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(detachImsiRequest);
            var deactivateRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDeleteSubscriberRequest(It.IsAny<Contracts.Subscriber.DeactiveSubscriberRequest>()))
                .Returns(deactivateRequest);

            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse() { Code = 789, Text = "bla", ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection() { ResponseList = new List<MatrixxObject>() } });

            var input = new Contracts.Subscriber.DeactiveSubscriberRequest()
            {
                Endpoint = null,
                CrmProductId = Guid.NewGuid(),
                Status = 123,
                Imsis = new List<string>() { "123456789" }
            };

            var result = await _agentUnderTest.DeleteSubscriberOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(789, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.BuildSetStatusRequest(It.Is<Contracts.Subscriber.SetSubscriberStatusRequest>(rq => rq.Status == input.Status && rq.CrmProductId == input.CrmProductId)), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.Is<Contracts.Imsi.DetachImsiFromSubscriberRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.Imsis == input.Imsis)), Times.Once);
            _messageBuilderUnitOfWork.Verify(x => x.Subscriber.CreateDeleteSubscriberRequest(It.Is<Contracts.Subscriber.DeactiveSubscriberRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.Status == input.Status && rq.Imsis == input.Imsis)), Times.Once);

            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == setSubscriberRequest), It.IsAny<Endpoint>()), Times.Once);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == detachImsiRequest), It.IsAny<Endpoint>()), Times.Once);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deactivateRequest), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task DeleteSubscriberOnMatrixx_ShouldThrowIfBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.DeleteSubscriberOnMatrixx(new Contracts.Subscriber.DeactiveSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task DeleteSubscriberOnMatrixx_ShouldThrowIfClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Returns(new MultiRequest());
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.DeleteSubscriberOnMatrixx(new Contracts.Subscriber.DeactiveSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task RemoveImsiFromSubscriber_ShouldSendDetatchImsiFromDeviceAndDeleteImsiRequestsToMatrixx()
        {
            var input = new Contracts.Imsi.RemoveImsiFromSubscriberRequest()
            {
                ProductId = Guid.NewGuid(),
                Imsi = "123456789"
            };
            var detachMessage = new MultiRequest();
            var deleteImsiMessage = new MultiRequest();

            var clientResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 123,
                Text = "bla",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
                {
                    ResponseList = new List<MatrixxObject>()
                }
            };

            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(detachMessage);
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateDeviceDeleteRequestList(It.IsAny<List<string>>()))
                .Returns(deleteImsiMessage);

            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(clientResponse);

            var result = await _agentUnderTest.RemoveImsiFromSubscriber(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify(
                x => x.Subscriber.CreateDetachImsiFromSubscriber(
                    It.Is<Contracts.Imsi.DetachImsiFromSubscriberRequest>(rq => rq.CrmProductId == input.ProductId && rq.Imsis.Count > 0 && rq.Imsis[0] == input.Imsi)),
                Times.Once);
            _messageBuilderUnitOfWork.Verify(
                x => x.Device.CreateDeviceDeleteRequestList(It.Is<List<string>>(lst => lst.Count > 0 && lst[0] == input.Imsi)),
                Times.Once);

            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == detachMessage || rq == deleteImsiMessage), It.IsAny<Endpoint>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task RemoveImsiFromSubscriber_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Throws(new TestException());

            var response = await _agentUnderTest.RemoveImsiFromSubscriber(new Contracts.Imsi.RemoveImsiFromSubscriberRequest());
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task RemoveImsiFromSubscriber_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(new MultiRequest());
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateDeviceDeleteRequestList(It.IsAny<List<string>>()))
                .Returns(new MultiRequest());

            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var response = await _agentUnderTest.RemoveImsiFromSubscriber(new Contracts.Imsi.RemoveImsiFromSubscriberRequest());
        }

        [TestMethod]
        public async Task AddImsiToSubscriber_ShouldCreateRequestUsingBuilderAndIssueItOnClient()
        {
            var builderResult = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateAddImsiToSubscriberRequest(It.IsAny<Contracts.Imsi.AddImsiToSubscriberRequest>()))
                .Returns(builderResult);

            var clientResult = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 123,
                Text = "bla",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(clientResult);

            var input = new Contracts.Imsi.AddImsiToSubscriberRequest()
            {
                SubscriberExternalId = Guid.NewGuid(),
                NewImsi = "987654321",
                MsisdnList = new List<string>() { "123456789" }
            };

            var result = await _agentUnderTest.AddImsiToSubscriber(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify(x => x.Device.CreateAddImsiToSubscriberRequest(
                    It.Is<Contracts.Imsi.AddImsiToSubscriberRequest>(rq => rq == input)), Times.Once);
            _client.Verify(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == builderResult), It.IsAny<Endpoint>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task AddImsiToSubscriber_ShouldThrowWhenBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateAddImsiToSubscriberRequest(It.IsAny<Contracts.Imsi.AddImsiToSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.AddImsiToSubscriber(new Contracts.Imsi.AddImsiToSubscriberRequest());
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task AddImsiToSubscriber_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateAddImsiToSubscriberRequest(It.IsAny<Contracts.Imsi.AddImsiToSubscriberRequest>()))
                .Returns(new MultiRequest());
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.AddImsiToSubscriber(new Contracts.Imsi.AddImsiToSubscriberRequest());
        }

        [TestMethod]
        public async Task SwapSim_ShouldUseMessageBuilderForEachStepToGetClientRequestAndThenIssueClientRequests()
        {
            var input = new Contracts.Sim.Swap.SwapSimRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Imsis = new List<string>() { "123456789" },
                OldImsis = new List<string>() { "987654321" }
            };

            var querySubscriberImsiRequest = new Api.Client.Contracts.Request.Query.ImsiDeviceQueryParameters("123456789");
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetImsiDeviceQueryParameters(It.IsAny<string>()))
                .Returns(querySubscriberImsiRequest)
                .Verifiable();

            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse()
            {
                MobileDeviceExtensionCollection = new Api.Client.Contracts.Model.Device.MobileDeviceExtensionCollection()
                {
                    MobileDeviceExtension = new Api.Client.Contracts.Model.Device.MobileDeviceExtension()
                    {
                        AccessNumberList = new Api.Client.Contracts.Model.StringValueCollection()
                        {
                            Values = new List<string>() { "147258369", "258369147" }
                        }
                    }
                }
            };
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.Is<IQueryParameters>(q => q == querySubscriberImsiRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var detachRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(detachRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == detachRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var deviceDeleteListRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateDeviceDeleteRequestList(It.Is<List<string>>(lst => lst.Count > 0 && lst[0] == "987654321")))
                .Returns(deviceDeleteListRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deviceDeleteListRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var deviceAddImsiRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateAddImsisRequest(
                    It.Is<Contracts.Sim.Swap.SwapSimRequest>(rq => rq == input),
                    It.Is<List<string>>(lst => lst.Count == 2 && lst.Contains("147258369") && lst.Contains("258369147"))))
                .Returns(deviceAddImsiRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deviceAddImsiRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse()
                {
                    Code = 123,
                    Text = "bla",
                    ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
                })
                .Verifiable();

            var modifySubscriberFirstNameRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberFirstNameRequest(
                    It.Is<Contracts.Subscriber.SetSubscriberFirstNameRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.FirstName == "123456789")))
                .Returns(modifySubscriberFirstNameRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == modifySubscriberFirstNameRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var result = await _agentUnderTest.SwapSim(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task SwapSim_ShouldUseSplitProvisioningOverloadWhenEndpointIsPresentInInput()
        {
            var input = new Contracts.Sim.Swap.SwapSimRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Imsis = new List<string>() { "123456789" },
                OldImsis = new List<string>() { "987654321" },
                Endpoint = new Endpoint()
            };

            var querySubscriberImsiRequest = new Api.Client.Contracts.Request.Query.ImsiDeviceQueryParameters("123456789");
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetImsiDeviceQueryParameters(It.IsAny<string>()))
                .Returns(querySubscriberImsiRequest)
                .Verifiable();

            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse()
            {
                MobileDeviceExtensionCollection = new Api.Client.Contracts.Model.Device.MobileDeviceExtensionCollection()
                {
                    MobileDeviceExtension = new Api.Client.Contracts.Model.Device.MobileDeviceExtension()
                    {
                        AccessNumberList = new Api.Client.Contracts.Model.StringValueCollection()
                        {
                            Values = new List<string>() { "147258369", "258369147" }
                        }
                    }
                }
            };
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.Is<IQueryParameters>(q => q == querySubscriberImsiRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var detachRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateDetachImsiFromSubscriber(It.IsAny<Contracts.Imsi.DetachImsiFromSubscriberRequest>()))
                .Returns(detachRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == detachRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var deviceDeleteListRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateDeviceDeleteRequestList(It.Is<List<string>>(lst => lst.Count > 0 && lst[0] == "987654321")))
                .Returns(deviceDeleteListRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deviceDeleteListRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var deviceAddImsiRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateAddImsisRequest(
                    It.Is<Contracts.Sim.Swap.SwapSimRequest>(rq => rq == input),
                    It.Is<List<string>>(lst => lst.Count == 2 && lst.Contains("147258369") && lst.Contains("258369147"))))
                .Returns(deviceAddImsiRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == deviceAddImsiRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse()
                {
                    Code = 123,
                    Text = "bla",
                    ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
                })
                .Verifiable();

            var modifySubscriberFirstNameRequest = new MultiRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberFirstNameRequest(
                    It.Is<Contracts.Subscriber.SetSubscriberFirstNameRequest>(rq => rq.CrmProductId == input.CrmProductId && rq.FirstName == "123456789")))
                .Returns(modifySubscriberFirstNameRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == modifySubscriberFirstNameRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(new Api.Client.Contracts.Response.Multi.MultiResponse())
                .Verifiable();

            var result = await _agentUnderTest.SwapSim(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SwapSim_ShouldThrowIfMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetImsiDeviceQueryParameters(It.IsAny<string>()))
                .Throws(new TestException());

            var response = await _agentUnderTest.SwapSim(new Contracts.Sim.Swap.SwapSimRequest() { OldImsis = new List<string>() { "123456789" } }).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SwapSim_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetImsiDeviceQueryParameters(It.IsAny<string>()))
                .Returns(new Api.Client.Contracts.Request.Query.ImsiDeviceQueryParameters("123456789"));
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var response = await _agentUnderTest.SwapSim(new Contracts.Sim.Swap.SwapSimRequest() { OldImsis = new List<string>() { "123456789" } }).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpdateContactPhoneNumber_ShouldUseMessageBuilderToCreateRequestAndIssueRequestWithClient()
        {
            var input = new Contracts.Msisdn.UpdateContactPhoneNumberRequest() { CrmProductId = Guid.NewGuid(), PrimaryMsisdn = "987654321" };

            var modifySubscriberRequest = new ModifySubscriberRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberContactPhoneNumberRequest(It.Is<Contracts.Msisdn.UpdateContactPhoneNumberRequest>(rq => rq == input)))
                .Returns(modifySubscriberRequest)
                .Verifiable();

            var clientResponse = new MatrixxResponse()
            {
                Text = "bla",
                Code = 123
            };
            _client.Setup(x => x.SubscriberProxy.SubscriberModify(It.Is<ModifySubscriberRequest>(rq => rq == modifySubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(clientResponse)
                .Verifiable();

            var result = await _agentUnderTest.UpdateContactPhoneNumber(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task UpdateContactPhoneNumber_ShouldUseOverloadThatSupportsSplitProvisioningIfInputHasEndpoint()
        {
            var input = new Contracts.Msisdn.UpdateContactPhoneNumberRequest() { CrmProductId = Guid.NewGuid(), PrimaryMsisdn = "987654321", Endpoint = new Endpoint() };

            var modifySubscriberRequest = new ModifySubscriberRequest();
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberContactPhoneNumberRequest(It.Is<Contracts.Msisdn.UpdateContactPhoneNumberRequest>(rq => rq == input)))
                .Returns(modifySubscriberRequest)
                .Verifiable();

            var clientResponse = new MatrixxResponse()
            {
                Text = "bla",
                Code = 123
            };
            _client.Setup(x => x.SubscriberProxy.SubscriberModify(
                    It.Is<ModifySubscriberRequest>(rq => rq == modifySubscriberRequest),
                    It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(clientResponse)
                .Verifiable();

            var result = await _agentUnderTest.UpdateContactPhoneNumber(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task UpdateContactPhoneNumber_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberContactPhoneNumberRequest(It.IsAny<Contracts.Msisdn.UpdateContactPhoneNumberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.UpdateContactPhoneNumber(new Contracts.Msisdn.UpdateContactPhoneNumberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task UpdateContactPhoneNumber_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.CreateModifySubscriberContactPhoneNumberRequest(It.IsAny<Contracts.Msisdn.UpdateContactPhoneNumberRequest>()))
                .Returns(new ModifySubscriberRequest());
            _client.Setup(x => x.SubscriberProxy.SubscriberModify(It.IsAny<ModifySubscriberRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.UpdateContactPhoneNumber(new Contracts.Msisdn.UpdateContactPhoneNumberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task UpdateMsisdnList_ShouldGetCurrentStateAndThenUpdateDeviceWithDataFromRequest()
        {
            var input = new Contracts.Msisdn.UpdateMsisdnListRequest()
            {
                PrimaryMsisdn = "123456789",
                NewMsIsdns = new List<string>() { "987654321" }
            };

            var msisdnQueryParametersRequest = new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("123456789");
            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.Is<string>(rq => rq == input.PrimaryMsisdn)))
                .Returns(msisdnQueryParametersRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.Is<IQueryParameters>(rq => rq == msisdnQueryParametersRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var updateMsisdnListRequest = new Api.Client.Contracts.Request.Device.DeviceModifyRequest();
            var updateMsisdnListResponse = new MatrixxResponse()
            {
                Code = 123,
                Text = "blabla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Device.BuildUpdateMsisdnListRequest(
                    It.Is<Api.Client.Contracts.Response.Device.DeviceQueryResponse>(rq => rq == deviceQueryResponse),
                    It.Is<Contracts.Msisdn.UpdateMsisdnListRequest>(rq => rq == input)))
                .Returns(updateMsisdnListRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceModify(It.Is<Api.Client.Contracts.Request.Device.DeviceModifyRequest>(rq => rq == updateMsisdnListRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(updateMsisdnListResponse)
                .Verifiable();

            var result = await _agentUnderTest.UpdateMsisdnList(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task UpdateMsisdnList_ShouldUseSplitProvisioningOverloadWhenEndpointIsPresentInInput()
        {
            var input = new Contracts.Msisdn.UpdateMsisdnListRequest()
            {
                PrimaryMsisdn = "123456789",
                NewMsIsdns = new List<string>() { "987654321" },
                Endpoint = new Endpoint()
            };

            var msisdnQueryParametersRequest = new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("123456789");
            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.Is<string>(rq => rq == input.PrimaryMsisdn)))
                .Returns(msisdnQueryParametersRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceQuery(
                    It.Is<IQueryParameters>(rq => rq == msisdnQueryParametersRequest),
                    It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var updateMsisdnListRequest = new Api.Client.Contracts.Request.Device.DeviceModifyRequest();
            var updateMsisdnListResponse = new MatrixxResponse()
            {
                Code = 123,
                Text = "blabla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Device.BuildUpdateMsisdnListRequest(
                    It.Is<Api.Client.Contracts.Response.Device.DeviceQueryResponse>(rq => rq == deviceQueryResponse),
                    It.Is<Contracts.Msisdn.UpdateMsisdnListRequest>(rq => rq == input)))
                .Returns(updateMsisdnListRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceModify(
                    It.Is<Api.Client.Contracts.Request.Device.DeviceModifyRequest>(rq => rq == updateMsisdnListRequest),
                    It.Is<Endpoint>(e => e == input.Endpoint) /*, // strange thing going on here, split provisioning endpoint is not used
                    It.Is<Endpoint>(e => e == input.Endpoint)*/))
                .ReturnsAsync(updateMsisdnListResponse)
                .Verifiable();

            var result = await _agentUnderTest.UpdateMsisdnList(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.Text);
            Assert.AreEqual(123, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task UpdateMsisdnList_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.IsAny<string>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.UpdateMsisdnList(new Contracts.Msisdn.UpdateMsisdnListRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task UpdateMsisdnList_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.IsAny<string>()))
                .Returns(new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("1234567489"));

            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.UpdateMsisdnList(new Contracts.Msisdn.UpdateMsisdnListRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SwapMsisdn_ShouldQueryDeviceParamtersBuildUpdateDeviceRequestAndExecuteIt()
        {
            var input = new Contracts.Msisdn.SwapMsIsdnRequest()
            {
                OldMsIsdn = "123456789",
                NewMsIsdn = "987654321"
            };

            var deviceQueryRequest = new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("123456789");
            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.Is<string>(msisdn => msisdn == input.OldMsIsdn)))
                .Returns(deviceQueryRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.Is<IQueryParameters>(rq => rq == deviceQueryRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var deviceModifyRequest = new Api.Client.Contracts.Request.Device.DeviceModifyRequest();
            var deviceModifyResponse = new MatrixxResponse()
            {
                Code = 789,
                Text = "test"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Device.BuildSwapMsIsdnRequest(It.Is<Api.Client.Contracts.Response.Device.DeviceQueryResponse>(rsp => rsp == deviceQueryResponse), It.Is<Contracts.Msisdn.SwapMsIsdnRequest>(rq => rq == input)))
                .Returns(deviceModifyRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceModify(It.Is<Api.Client.Contracts.Request.Device.DeviceModifyRequest>(rq => rq == deviceModifyRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(deviceModifyResponse)
                .Verifiable();

            var result = await _agentUnderTest.SwapMsisdn(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Text);
            Assert.AreEqual(789, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task SwapMsisdn_ShouldUseSplitProvisioningIfEnndpointInInputIsSet()
        {
            var input = new Contracts.Msisdn.SwapMsIsdnRequest()
            {
                OldMsIsdn = "123456789",
                NewMsIsdn = "987654321",
                Endpoint = new Endpoint()
            };

            var deviceQueryRequest = new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("123456789");
            var deviceQueryResponse = new Api.Client.Contracts.Response.Device.DeviceQueryResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.Is<string>(msisdn => msisdn == input.OldMsIsdn)))
                .Returns(deviceQueryRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.Is<IQueryParameters>(rq => rq == deviceQueryRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(deviceQueryResponse)
                .Verifiable();

            var deviceModifyRequest = new Api.Client.Contracts.Request.Device.DeviceModifyRequest();
            var deviceModifyResponse = new MatrixxResponse()
            {
                Code = 789,
                Text = "test"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Device.BuildSwapMsIsdnRequest(It.Is<Api.Client.Contracts.Response.Device.DeviceQueryResponse>(rsp => rsp == deviceQueryResponse), It.Is<Contracts.Msisdn.SwapMsIsdnRequest>(rq => rq == input)))
                .Returns(deviceModifyRequest)
                .Verifiable();
            _client.Setup(x => x.DeviceProxy.DeviceModify(It.Is<Api.Client.Contracts.Request.Device.DeviceModifyRequest>(rq => rq == deviceModifyRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(deviceModifyResponse)
                .Verifiable();

            var result = await _agentUnderTest.SwapMsisdn(input);

            Assert.IsNotNull(result);
            Assert.AreEqual("test", result.Text);
            Assert.AreEqual(789, result.Code);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SwapMsisdn_ShouldThrowIfMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.IsAny<string>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.SwapMsisdn(new Contracts.Msisdn.SwapMsIsdnRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SwapMsisdn_ShouldThrowIfClienntThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetMsisdnDeviceQueryParameters(It.IsAny<string>()))
                .Returns(new Api.Client.Contracts.Request.Query.MsisdnDeviceQueryParameters("bla"));
            _client.Setup(x => x.DeviceProxy.DeviceQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.SwapMsisdn(new Contracts.Msisdn.SwapMsIsdnRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateSessionForDeviceList_ShouldUseBuilderToCreateRequestAndExecuteItOnClient()
        {
            var input = new Contracts.Device.ValidateSessionForDeviceListRequest()
            {
                Imsis = new List<string>() { "123456789" },
                SessionType = 456,
                Endpoint = new Endpoint()
            };

            var validateRequest = new MultiRequest();
            var validateResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 147,
                Text = "blabla",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateValidateDeviceListSession(It.Is<Contracts.Device.ValidateSessionForDeviceListRequest>(rq => rq == input)))
                .Returns(validateRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == validateRequest), It.Is<Endpoint>(ep => ep == input.Endpoint)))
                .ReturnsAsync(validateResponse)
                .Verifiable();

            var result = await _agentUnderTest.ValidateSessionForDeviceList(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.Text);
            Assert.AreEqual(147, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task ValidateSessionForDeviceList_ShouldThrowWhenBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateValidateDeviceListSession(It.IsAny<Contracts.Device.ValidateSessionForDeviceListRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.ValidateSessionForDeviceList(new Contracts.Device.ValidateSessionForDeviceListRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task ValidateSessionForDeviceList_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.CreateValidateDeviceListSession(It.IsAny<Contracts.Device.ValidateSessionForDeviceListRequest>()))
                .Returns(new MultiRequest());

            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.ValidateSessionForDeviceList(new Contracts.Device.ValidateSessionForDeviceListRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SetSubscriberStatusOnMatrixx_ShouldUseMessageBuilderToConstructRequestAndIssueItToMatrixxUsingClient()
        {
            var input = new Contracts.Subscriber.SetSubscriberStatusRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Status = 456
            };
            var statusSetMessage = new MultiRequest();
            var statusMessageResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 123,
                Text = "blabla",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.Is<Contracts.Subscriber.SetSubscriberStatusRequest>(rq => rq == input)))
                .Returns(statusSetMessage);
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == statusSetMessage), It.IsAny<Endpoint>()))
                .ReturnsAsync(statusMessageResponse);

            var result = await _agentUnderTest.SetSubscriberStatusOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.Text);
            Assert.AreEqual(123, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task SetSubscriberStatusOnMatrixx_ShouldUseSplitProvisioningOverloadsWhenEndpointIsSetInInput()
        {
            var input = new Contracts.Subscriber.SetSubscriberStatusRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Status = 456,
                Endpoint = new Endpoint()
            };
            var statusSetMessage = new MultiRequest();
            var statusMessageResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 123,
                Text = "blabla",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.Is<Contracts.Subscriber.SetSubscriberStatusRequest>(rq => rq == input)))
                .Returns(statusSetMessage);
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == statusSetMessage), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(statusMessageResponse);

            var result = await _agentUnderTest.SetSubscriberStatusOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.Text);
            Assert.AreEqual(123, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SetSubscriberStatusOnMatrixx_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.SetSubscriberStatusOnMatrixx(new Contracts.Subscriber.SetSubscriberStatusRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SetSubscriberStatusOnMatrixx_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetStatusRequest(It.IsAny<Contracts.Subscriber.SetSubscriberStatusRequest>()))
                .Returns(new MultiRequest());
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.SetSubscriberStatusOnMatrixx(new Contracts.Subscriber.SetSubscriberStatusRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task CreateSubscriberOnMatrixx_ShouldUseMessageBuilderToGenerateTheRequestAndIssueItWithClient()
        {
            var input = new Contracts.Subscriber.CreateSubscriberRequest()
            {
                CrmProductId = Guid.NewGuid(),
            };
            var createSubscriberRequest = new MultiRequest();
            var createSubscriberResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 789,
                Text = "truc",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildCreateSubscriberRequest(It.Is<Contracts.Subscriber.CreateSubscriberRequest>(rq => rq == input)))
                .Returns(createSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == createSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(createSubscriberResponse)
                .Verifiable();

            var result = await _agentUnderTest.CreateSubscriberOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(789, result.Code);
            Assert.AreEqual("truc", result.Text);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task CreateSubscriberOnMatrixx_ShouldUseSplitProvisioningOverloadsIfEndpointIsSetInInputMessage()
        {
            var input = new Contracts.Subscriber.CreateSubscriberRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Endpoint = new Endpoint()
            };
            var createSubscriberRequest = new MultiRequest();
            var createSubscriberResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 799,
                Text = "truc",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildCreateSubscriberRequest(It.Is<Contracts.Subscriber.CreateSubscriberRequest>(rq => rq == input)))
                .Returns(createSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == createSubscriberRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(createSubscriberResponse)
                .Verifiable();

            var result = await _agentUnderTest.CreateSubscriberOnMatrixx(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(799, result.Code);
            Assert.AreEqual("truc", result.Text);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CreateSubscriberOnMatrixx_ShouldThrowIfMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildCreateSubscriberRequest(It.IsAny<Contracts.Subscriber.CreateSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.CreateSubscriberOnMatrixx(new Contracts.Subscriber.CreateSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task CreateSubscriberOnMatrixx_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildCreateSubscriberRequest(It.IsAny<Contracts.Subscriber.CreateSubscriberRequest>()))
                .Returns(new MultiRequest());
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.CreateSubscriberOnMatrixx(new Contracts.Subscriber.CreateSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task AddOfferToSubscriber_ShouldCreateRequestUsingMessageBuilderANdIssueItWithClient()
        {
            var input = new AddOfferToSubscriberRequest()
            {
                CrmProductId = Guid.NewGuid(),
            };
            var addOfferRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferResponse = new AddOfferToSubscriberResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(It.IsAny<AddOfferToSubscriberRequest>()))
                .Returns(addOfferRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferResponse)
                .Verifiable();

            var result = await _agentUnderTest.AddOfferToSubscriber(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(addOfferResponse, result);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task AddOfferToSubscriber_ShouldUseSplitProvisioningOverloadWhenEndpointIsSetInInput()
        {
            var input = new AddOfferToSubscriberRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Endpoint = new Endpoint()
            };
            var addOfferRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferResponse = new AddOfferToSubscriberResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(It.IsAny<AddOfferToSubscriberRequest>()))
                .Returns(addOfferRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(addOfferResponse)
                .Verifiable();

            var result = await _agentUnderTest.AddOfferToSubscriber(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(addOfferResponse, result);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task DeleteGroup_ShouldUseSplitProvisioningOverloadWhenEndpointIsSetInInput()
        {
            var input = new RemoveGroupRequest()
            {
                ExternalId = Guid.NewGuid().ToString().ToUpper(),
                Endpoint = new Endpoint()
            };
            var matrixxResponse = new MatrixxResponse()
            {
                Code = 123,
                Text = "bla"
            };

            var deleteGroupRequest = new Api.Client.Contracts.Request.Group.DeleteGroupRequest();
            var basicResponse = new BasicResponse();
            _messageBuilderUnitOfWork.Setup(x => x.Group.CreateDeleteGroupRequest(It.IsAny<RemoveGroupRequest>()))
                .Returns(deleteGroupRequest)
                .Verifiable();
            _client.Setup(x => x.GroupProxy.RemoveGroup(It.IsAny<Api.Client.Contracts.Request.Group.DeleteGroupRequest>(), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(matrixxResponse)
                .Verifiable();

            var result = await _agentUnderTest.DeleteGroup(input).ConfigureAwait(false);


            Assert.IsNotNull(result);
            Assert.AreEqual(matrixxResponse.Text, result.Text);
            Assert.AreEqual(matrixxResponse.Code, result.Code);

        }


        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task AddOfferToSubscriber_ShouldThrowWhenBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(It.IsAny<AddOfferToSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.AddOfferToSubscriber(new AddOfferToSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task AddOfferToSubscriber_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(It.IsAny<AddOfferToSubscriberRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest());
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(It.IsAny<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.AddOfferToSubscriber(new AddOfferToSubscriberRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task SetCustomSubscriberConfiguration_ShouldUseBuilderToCreateRequestAndIssueItWithClient()
        {
            var input = new Contracts.Subscriber.SetCustomConfigurationRequest()
            {
                CrmProductId = Guid.NewGuid(),
            };
            var setCustomSubscriberConfigRequest = new MultiRequest();
            var setCustomSubscriberConfigResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 159,
                Text = "blabla123",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetCustomSubscriberConfigurationRequest(It.Is<Contracts.Subscriber.SetCustomConfigurationRequest>(rq => rq == input)))
                .Returns(setCustomSubscriberConfigRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == setCustomSubscriberConfigRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(setCustomSubscriberConfigResponse)
                .Verifiable();

            var result = await _agentUnderTest.SetCustomSubscriberConfiguration(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla123", result.Text);
            Assert.AreEqual(159, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task SetCustomSubscriberConfiguration_ShouldUseSplitProvisioningOverloadIfEndpointIsPresentInInput()
        {
            var input = new Contracts.Subscriber.SetCustomConfigurationRequest()
            {
                CrmProductId = Guid.NewGuid(),
                Endpoint = new Endpoint()
            };
            var setCustomSubscriberConfigRequest = new MultiRequest();
            var setCustomSubscriberConfigResponse = new Api.Client.Contracts.Response.Multi.MultiResponse()
            {
                Code = 159,
                Text = "blabla123",
                ResponseCollection = new Api.Client.Contracts.Response.Multi.ResponseCollection()
            };
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetCustomSubscriberConfigurationRequest(It.Is<Contracts.Subscriber.SetCustomConfigurationRequest>(rq => rq == input)))
                .Returns(setCustomSubscriberConfigRequest)
                .Verifiable();
            _client.Setup(x => x.MultiProxy.RequestMulti(It.Is<MultiRequest>(rq => rq == setCustomSubscriberConfigRequest), It.Is<Endpoint>(e => e == input.Endpoint)))
                .ReturnsAsync(setCustomSubscriberConfigResponse)
                .Verifiable();

            var result = await _agentUnderTest.SetCustomSubscriberConfiguration(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla123", result.Text);
            Assert.AreEqual(159, result.Code);
            Assert.IsNotNull(result.RepsonseList);
            Assert.AreEqual(0, result.RepsonseList.Count);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SetCustomSubscriberConfiguration_ShouldThrowWhenBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetCustomSubscriberConfigurationRequest(It.IsAny<Contracts.Subscriber.SetCustomConfigurationRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.SetCustomSubscriberConfiguration(new Contracts.Subscriber.SetCustomConfigurationRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task SetCustomSubscriberConfiguration_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Subscriber.BuildSetCustomSubscriberConfigurationRequest(It.IsAny<Contracts.Subscriber.SetCustomConfigurationRequest>()))
                .Returns(new MultiRequest());
            _client.Setup(x => x.MultiProxy.RequestMulti(It.IsAny<MultiRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.SetCustomSubscriberConfiguration(new Contracts.Subscriber.SetCustomConfigurationRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPurchasedOffers_ShouldCreateSubscriberQueryAndUseClientToFetchSubscriberDataByProductIdWhenItsSet()
        {
            var input = new GetPurchasedOffersRequest()
            {
                ProductId = Guid.NewGuid()
            };
            var productIdQuery = new Mock<IQueryParameters>().Object;
            var msisdnQuery = new Mock<IQueryParameters>().Object;

            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<string>()))
                .Returns(msisdnQuery);
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(productIdQuery)
                .Verifiable();

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == productIdQuery), It.IsAny<Endpoint>()))
                .ReturnsAsync(new SubscriberQueryResponse())
                .Verifiable();

            var result = await _agentUnderTest.GetPurchasedOffers(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task GetPurchasedOffers_ShouldCreateSubscriberQueryAndUseClientToFetchSubscriberDataByMsisdnWhenProductIdIsNotSet()
        {
            var input = new GetPurchasedOffersRequest()
            {
                MsIsdn = "123456789"
            };
            var productIdQuery = new Mock<IQueryParameters>().Object;
            var msisdnQuery = new Mock<IQueryParameters>().Object;

            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.Is<string>(msisdn => msisdn == input.MsIsdn)))
                .Returns(msisdnQuery)
                .Verifiable();
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Returns(productIdQuery);

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == msisdnQuery),It.IsAny<Endpoint>()))
                .ReturnsAsync(new SubscriberQueryResponse())
                .Verifiable();

            var result = await _agentUnderTest.GetPurchasedOffers(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task GetPurchasedOffers_ShouldReturnClientResponseWithCorrectData()
        {
            var input = new GetPurchasedOffersRequest()
            {
                ProductId = Guid.NewGuid()
            };
            var productIdQuery = new Mock<IQueryParameters>().Object;
            var subscriberQueryResponse = new SubscriberQueryResponse()
            {
                ResultText = "bla",
                Result = 123,
                PurchaseInfoList = new PurchasedOfferInfoCollection()
                {
                    Values = new List<PurchasedOfferInfo>()
                    {
                        new PurchasedOfferInfo()
                        {
                            ProductOfferExternalId = "test123",
                            ProductOfferId = 456,
                            ProductOfferVersion = 789,
                            PurchaseTime = DateTime.Now,
                            ResourceId = 147,
                            Status = 258
                        }
                    }
                }
            };

            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(productIdQuery);

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == productIdQuery), It.IsAny<Endpoint>()))
                .ReturnsAsync(subscriberQueryResponse);

            var result = await _agentUnderTest.GetPurchasedOffers(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.ErrorMessage);
            Assert.AreEqual("123", result.ErrorCode);
            Assert.IsNotNull(result.PurchasedOfferCollection);
            Assert.AreEqual(1, result.PurchasedOfferCollection.Count);
            Assert.AreEqual("test123", result.PurchasedOfferCollection[0].ProductOfferExternalId);
            Assert.AreEqual(456, result.PurchasedOfferCollection[0].ProductOfferId);
            Assert.AreEqual(789, result.PurchasedOfferCollection[0].ProductOfferVersion);
            Assert.AreEqual(subscriberQueryResponse.PurchaseInfoList.Values[0].PurchaseTime, result.PurchasedOfferCollection[0].PurchaseTime);
            Assert.AreEqual(147, result.PurchasedOfferCollection[0].ResourceId);
            Assert.AreEqual(258, result.PurchasedOfferCollection[0].Status);
        }

        [TestMethod]
        public async Task GetPurchasedOffers_ShouldReturnNullWhenClientReturnsNull()
        {
            var input = new GetPurchasedOffersRequest()
            {
                ProductId = Guid.NewGuid()
            };
            var productIdQuery = new Mock<IQueryParameters>().Object;

            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(productIdQuery);

            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == productIdQuery), It.IsAny<Endpoint>()))
                .ReturnsAsync(default(SubscriberQueryResponse));

            var result = await _agentUnderTest.GetPurchasedOffers(input).ConfigureAwait(false);

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task GetPurchasedOffers_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<string>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.GetPurchasedOffers(new GetPurchasedOffersRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task GetPurchasedOffers_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByMsisdn(It.IsAny<string>()))
                .Returns(new Mock<IQueryParameters>().Object);
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.GetPurchasedOffers(new GetPurchasedOffersRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetSubscriber_ShouldCallClientToGetResults()
        {
            var input = new Mock<IQueryParameters>().Object;
            var queryResult = new SubscriberQueryResponse();
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == input), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryResult)
                .Verifiable();

            var result = await _agentUnderTest.GetSubscriber(input,null).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual(queryResult, result);
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task GetSubscriber_ShouldThrowIfClientThrows()
        {
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.GetSubscriber(new Mock<IQueryParameters>().Object,null).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FlexTopup_ShouldAddThresholdIfRequestIsForQuota()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                TemplateId = 234,
                IsQuota = true
            };
            var addThresholdRequest = new SubscriberAddThresholdRequest();
            var addThresholdResult = new MatrixxResponse()
            {
                Code = 456,
                Text = "blabla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Threshold.BuildSubscriberAddThresholdRequest(It.Is<AddThresholdToSubscriberRequest>(
                rq => rq.Amount == input.Amount && rq.CrmProductId == input.ProductId && rq.ThresholdId == input.TemplateId)))
                .Returns(addThresholdRequest)
                .Verifiable();
            _client.Setup(x => x.BalanceProxy.SubscriberAddThreshold(It.Is<SubscriberAddThresholdRequest>(rq => rq == addThresholdRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addThresholdResult)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("blabla", result.ErrorMessage);
            Assert.AreEqual("456", result.ErrorCode);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenMessageBuilderThrowsForRequestForQuotaTopup()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                TemplateId = 234,
                IsQuota = true
            };
            _messageBuilderUnitOfWork.Setup(x => x.Threshold.BuildSubscriberAddThresholdRequest(It.IsAny<AddThresholdToSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenClientThrowsForRequestsForQuotaTopup()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                TemplateId = 234,
                IsQuota = true
            };
            _messageBuilderUnitOfWork.Setup(x => x.Threshold.BuildSubscriberAddThresholdRequest(It.IsAny<AddThresholdToSubscriberRequest>()))
                .Returns(new SubscriberAddThresholdRequest());
            _client.Setup(x => x.BalanceProxy.SubscriberAddThreshold(It.IsAny<SubscriberAddThresholdRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FlexTopup_ShouldPurchaseNewOfferWhenOfferIsSetInInputAndAdjustBalanceOnnPurchasedOffer()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 0, // success
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>()
                    {
                        new PurchaseInfo()
                        {
                            ProductOfferExternalId = "test offer",
                            Balances = new RequiredBalances()
                            {
                                Values = new List<RequiredBalanceInfo>()
                                {
                                    new RequiredBalanceInfo()
                                    {
                                        ResourceId = 369,
                                        TemplateId = 357 // must match template id from request
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse)
                .Verifiable();

            var adjustBalancesRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanncesResponse = new MatrixxResponse()
            {
                Code = 321,
                Text = "bla result"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(
                    It.Is<FlexTopupRequest>(rq => rq == input),
                    It.Is<int>(resourceid => resourceid == addOfferToSubscriberResponse.PurchaseInfoList.Values[0].Balances.Values[0].ResourceId)))
                .Returns(adjustBalancesRequest)
                .Verifiable();
            _client.Setup(x => x.BalanceProxy.SubscriberAdjustBalance(It.Is<SubscriberAdjustBalanceRequest>(rq => rq == adjustBalancesRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(adjustBalanncesResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("321", result.ErrorCode);
            Assert.AreEqual("bla result", result.ErrorMessage);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();

        }

        [TestMethod]
        public async Task FlexTopup_ShouldReturnErrorResponseIfAddOfferToSubscriberFailForTopupDoneOverPurchasedOffer()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 123, // error
                ResultText = "test error"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.ErrorCode);
            Assert.AreEqual("test error", result.ErrorMessage);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task FlexTopup_ShouldReturnErrorWhenAddOfferResponseDoesNotContainRequiredBalance()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 0, // success
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>()
                    {
                        new PurchaseInfo()
                        {
                            ProductOfferExternalId = "other test offer",
                            Balances = new RequiredBalances()
                            {
                                Values = new List<RequiredBalanceInfo>()
                                {
                                    new RequiredBalanceInfo()
                                    {
                                        ResourceId = 369,
                                        TemplateId = 357 // must match template id from request
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("404", result.ErrorCode);
            StringAssert.Contains(result.ErrorMessage, "Missing balance");

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task FlexTopup_ShouldReturnErrorWhenAddedOfferBalanceDoesNotHaveAssignedResourceId()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 0, // success
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>()
                    {
                        new PurchaseInfo()
                        {
                            ProductOfferExternalId = "test offer",
                            Balances = new RequiredBalances()
                            {
                                Values = new List<RequiredBalanceInfo>()
                                {
                                    new RequiredBalanceInfo()
                                    {
                                        ResourceId = 369,
                                        TemplateId = 333 // does not match template id from request
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest)
                .Verifiable();
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("404", result.ErrorCode);
            StringAssert.Contains(result.ErrorMessage, "ResourceId");

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShoulThrowWhenBuilderThrowsWhenConstructingRequestToPurchaseOfferForSubscriber()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.IsAny<AddOfferToSubscriberRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenClientThrowsWhenTryingToPurchaseOfferForSubscriber()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.IsAny<AddOfferToSubscriberRequest>()))
                .Returns(new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest());
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.IsAny<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenMessageBuilderThrowsWhileConstructingMessageForAdjustBalanceForPurchaseOfferCase()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 0, // success
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>()
                    {
                        new PurchaseInfo()
                        {
                            ProductOfferExternalId = "test offer",
                            Balances = new RequiredBalances()
                            {
                                Values = new List<RequiredBalanceInfo>()
                                {
                                    new RequiredBalanceInfo()
                                    {
                                        ResourceId = 369,
                                        TemplateId = 357 // must match template id from request
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest);
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse);

            var adjustBalancesRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanncesResponse = new MatrixxResponse()
            {
                Code = 321,
                Text = "bla result"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(
                    It.Is<FlexTopupRequest>(rq => rq == input),
                    It.Is<int>(resourceid => resourceid == addOfferToSubscriberResponse.PurchaseInfoList.Values[0].Balances.Values[0].ResourceId)))
                .Throws(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenClientThrowsWhileAdjustingBalanceForPurchaseOfferCase()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = true,
                OfferName = "test offer",
                TemplateId = 357
            };
            var addOfferToSubscriberRequest = new Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest();
            var addOfferToSubscriberResponse = new AddOfferToSubscriberResponse()
            {
                Result = 0, // success
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>()
                    {
                        new PurchaseInfo()
                        {
                            ProductOfferExternalId = "test offer",
                            Balances = new RequiredBalances()
                            {
                                Values = new List<RequiredBalanceInfo>()
                                {
                                    new RequiredBalanceInfo()
                                    {
                                        ResourceId = 369,
                                        TemplateId = 357 // must match template id from request
                                    }
                                }
                            }
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Offer.BuildPurchaseOfferForSubscriberRequest(
                    It.Is<AddOfferToSubscriberRequest>(rq =>
                        rq.CrmProductId == input.ProductId
                        && rq.OffersToBePurchased != null
                        && rq.OffersToBePurchased.Count == 1
                        && rq.OffersToBePurchased[0] == input.OfferName)))
                .Returns(addOfferToSubscriberRequest);
            _client.Setup(x => x.OfferProxy.AddOfferToSubscriber(
                    It.Is<Api.Client.Contracts.Request.ProductOffer.PurchaseOfferForSubscriberRequest>(rq => rq == addOfferToSubscriberRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(addOfferToSubscriberResponse);

            var adjustBalancesRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanncesResponse = new MatrixxResponse()
            {
                Code = 321,
                Text = "bla result"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(
                    It.Is<FlexTopupRequest>(rq => rq == input),
                    It.Is<int>(resourceid => resourceid == addOfferToSubscriberResponse.PurchaseInfoList.Values[0].Balances.Values[0].ResourceId)))
                .Returns(adjustBalancesRequest);
            _client.Setup(x => x.BalanceProxy.SubscriberAdjustBalance(It.Is<SubscriberAdjustBalanceRequest>(rq => rq == adjustBalancesRequest), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task FlexTopup_ShouldAdjustSubscriberBalanceIfNeitherQuotaNorOfferIsSetInInputButProductIdIs()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type",
                Endpoint = new Endpoint()
                {
                    Name = "endpoint",
                    Id = 1,
                    Enabled = 0
                }
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some balance type",
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(queryBalanceRequest)
                .Verifiable();
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == queryBalanceRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryBalanceResponse)
                .Verifiable();

            var adjustBalanceRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanceResponse = new MatrixxResponse()
            {
                Code = 654,
                Text = "vla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(It.Is<FlexTopupRequest>(rq => rq == input), It.Is<int>(resourceid => resourceid == 951)))
                .Returns(adjustBalanceRequest)
                .Verifiable();
            _client.Setup(x => x.BalanceProxy.SubscriberAdjustBalance(It.Is<SubscriberAdjustBalanceRequest>(rq => rq == adjustBalanceRequest), It.IsAny<Endpoint>()))
                 .ReturnsAsync(adjustBalanceResponse)
                 .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("654", result.ErrorCode);
            Assert.AreEqual("vla", result.ErrorMessage);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task FlexTopup_ShouldReturnErrorWhenQueryBalanceFailsIfNeitherQuotaNorOfferIsSetInInput()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 123,
                ResultText = "bla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(queryBalanceRequest)
                .Verifiable();
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == queryBalanceRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryBalanceResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.ErrorCode);
            Assert.AreEqual("bla", result.ErrorMessage);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task FlexTopup_ShouldReturnErrorWhenQueryBalanceDoesNotReturnRequestedBalanceType()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some other balance type", // does not match balance type in request
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.Is<Guid>(id => id == input.ProductId)))
                .Returns(queryBalanceRequest)
                .Verifiable();
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.Is<IQueryParameters>(rq => rq == queryBalanceRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryBalanceResponse)
                .Verifiable();

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("404", result.ErrorCode);
            Assert.AreEqual("Balance not found", result.ErrorMessage);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenBuilderThrowsWhileConstructingQueryBalanceRequest()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some balance type",
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenClientThrowsWhileQueryingBalances()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some balance type",
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Returns(queryBalanceRequest);
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhenMessageBuilderThrowsWhileCreatingRequestForAdjustBalance()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some balance type",
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Returns(queryBalanceRequest);
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryBalanceResponse);

            var adjustBalanceRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanceResponse = new MatrixxResponse()
            {
                Code = 654,
                Text = "vla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(It.IsAny<FlexTopupRequest>(), It.IsAny<int>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task FlexTopup_ShouldThrowWhhenClientThrowsWhileAdjustingBalance()
        {
            var input = new FlexTopupRequest()
            {
                Amount = 123m,
                ProductId = Guid.NewGuid(),
                PurchaseOffer = false,
                IsQuota = false,
                BalanceType = "some balance type"
            };

            var queryBalanceRequest = new Mock<IQueryParameters>().Object;
            var queryBalanceResponse = new SubscriberQueryResponse()
            {
                Result = 0,
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            Name = "some balance type",
                            ResourceId = 951
                        }
                    }
                }
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetQueryBalanceParametersByProductId(It.IsAny<Guid>()))
                .Returns(queryBalanceRequest);
            _client.Setup(x => x.SubscriberProxy.SubscriberQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryBalanceResponse);

            var adjustBalanceRequest = new SubscriberAdjustBalanceRequest();
            var adjustBalanceResponse = new MatrixxResponse()
            {
                Code = 654,
                Text = "vla"
            };
            _messageBuilderUnitOfWork.Setup(x => x.Balance.GetAdjustBalanceRequest(It.IsAny<FlexTopupRequest>(), It.IsAny<int>()))
                .Returns(adjustBalanceRequest);
            _client.Setup(x => x.BalanceProxy.SubscriberAdjustBalance(It.IsAny<SubscriberAdjustBalanceRequest>(), It.IsAny<Endpoint>()))
                 .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.FlexTopup(input).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryGroupWallet_ShouldUseMessageBuilderToConstructQueryRequestUseClientToPerformQueryAndReturnTranslatedResults()
        {
            var input = new Contracts.Wallet.GetGroupWalletRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };
            var queryWalletRequest = new Api.Client.Contracts.Request.Wallet.GroupWalletQueryRequest();
            var queryWalletResponse = new Api.Client.Contracts.Response.Wallet.SubscriberWalletResponse()
            {
                Result = 123,
                ResultText = "bla",
                WalletInfoList = new Api.Client.Contracts.Model.Wallet.WalletInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>()
                    {
                        new Api.Client.Contracts.Model.Wallet.WalletInfo()
                        {
                            ResourceId = 111,
                            TemplateId = "222",
                            Name = "bla balance",
                            ReservedAmount = "123",
                            AvailableAmount = "456",
                            ThresholdLimit = "14",
                            QuantityUnit = "SMS",
                            StartTime = DateTime.Now.AddDays(-5),
                            EndTime = DateTime.Now.AddHours(15)
                        }
                    }
                },
                WalletInfoPeriodicList = new Api.Client.Contracts.Model.Wallet.WalletInfoPeriodicCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Wallet.WalletInfo>()
                    {
                        new Api.Client.Contracts.Model.Wallet.WalletInfo()
                        {
                            ResourceId = 333,
                            TemplateId = "444",
                            Name = "bla recurring balance",
                            ReservedAmount = "147",
                            AvailableAmount = "258",
                            ThresholdLimit = "32",
                            QuantityUnit = "MIN",
                            StartTime = DateTime.Now.AddDays(-8),
                            EndTime = DateTime.Now.AddMinutes(3)
                        }
                    }
                }
            };

            _messageBuilderUnitOfWork.Setup(x => x.Wallet.GetQueryGroupWalletRequest(It.Is<Contracts.Wallet.GetGroupWalletRequest>(rq => rq == input)))
                .Returns(queryWalletRequest)
                .Verifiable();
            _client.Setup(x => x.WalletProxy.QueryGroupWallet(It.Is<Api.Client.Contracts.Request.Wallet.GroupWalletQueryRequest>(rq => rq == queryWalletRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryWalletResponse)
                .Verifiable();

            var result = await _agentUnderTest.QueryGroupWallet(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.ErrorCode);
            Assert.AreEqual("bla", result.ErrorMessage);
            Assert.IsNotNull(result.SimpleBalanceList);
            Assert.AreEqual(1, result.SimpleBalanceList.Count);
            Assert.AreEqual(111, result.SimpleBalanceList[0].ResourceId);
            Assert.AreEqual("222", result.SimpleBalanceList[0].TemplateId);
            Assert.AreEqual("bla balance", result.SimpleBalanceList[0].Name);
            Assert.AreEqual("123", result.SimpleBalanceList[0].ReservedAmount);
            Assert.AreEqual("456", result.SimpleBalanceList[0].Amount);
            Assert.AreEqual("14", result.SimpleBalanceList[0].TresholdLimit);
            Assert.AreEqual("SMS", result.SimpleBalanceList[0].Unit);
            Assert.AreEqual(queryWalletResponse.WalletInfoList.Values[0].StartTime, result.SimpleBalanceList[0].StartTime);
            Assert.AreEqual(queryWalletResponse.WalletInfoList.Values[0].EndTime, result.SimpleBalanceList[0].EndTime);
            Assert.IsNotNull(result.PeriodicBalanceList);
            Assert.AreEqual(1, result.PeriodicBalanceList.Count);
            Assert.AreEqual(333, result.PeriodicBalanceList[0].ResourceId);
            Assert.AreEqual("444", result.PeriodicBalanceList[0].TemplateId);
            Assert.AreEqual("bla recurring balance", result.PeriodicBalanceList[0].Name);
            Assert.AreEqual("147", result.PeriodicBalanceList[0].ReservedAmount);
            Assert.AreEqual("258", result.PeriodicBalanceList[0].Amount);
            Assert.AreEqual("32", result.PeriodicBalanceList[0].TresholdLimit);
            Assert.AreEqual("MIN", result.PeriodicBalanceList[0].Unit);
            Assert.AreEqual(queryWalletResponse.WalletInfoPeriodicList.Values[0].StartTime, result.PeriodicBalanceList[0].StartTime);
            Assert.AreEqual(queryWalletResponse.WalletInfoPeriodicList.Values[0].EndTime, result.PeriodicBalanceList[0].EndTime);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task QueryGroupWallet_ShouldThrowWhenMessageBuilderThrows()
        {
            var input = new Contracts.Wallet.GetGroupWalletRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };

            _messageBuilderUnitOfWork.Setup(x => x.Wallet.GetQueryGroupWalletRequest(It.IsAny<Contracts.Wallet.GetGroupWalletRequest>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.QueryGroupWallet(input).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task QueryGroupWallet_ShouldThrowWhenClientThrows()
        {
            var input = new Contracts.Wallet.GetGroupWalletRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };

            _messageBuilderUnitOfWork.Setup(x => x.Wallet.GetQueryGroupWalletRequest(It.IsAny<Contracts.Wallet.GetGroupWalletRequest>()))
                .Returns(new Api.Client.Contracts.Request.Wallet.GroupWalletQueryRequest());
            _client.Setup(x => x.WalletProxy.QueryGroupWallet(It.IsAny<Api.Client.Contracts.Request.Wallet.GroupWalletQueryRequest>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.QueryGroupWallet(input).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryGroupBalance_ShouldUseMessageBuilderTOCreateRequestIssueItUsingClientAndReturnResults()
        {
            var input = new QueryGroupBalanceRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };
            var queryGroupRequest = new GroupIdQueryParameters("bla");
            var queryGroupResult = new GroupQueryResponse()
            {
                Result = 123,
                ResultText = "bla",
                BalanceInfoList = new Api.Client.Contracts.Model.Balance.BalanceInfoCollection()
                {
                    Values = new List<Api.Client.Contracts.Model.Balance.BalanceInfo>()
                    {
                        new Api.Client.Contracts.Model.Balance.BalanceInfo()
                        {
                            ResourceId = 369,
                            TemplateId = "258",
                            Name = "bla balance",
                            AvailableAmount = "456",
                            ThresholdLimit = "23",
                            QuantityUnit = "MB",
                            StartTime = DateTime.Now.AddDays(-10),
                            EndTime = DateTime.Now.AddDays(5)
                        }
                    }
                }
            };

            _messageBuilderUnitOfWork.Setup(x => x.Group.GetGroupRequest(It.Is<string>(externalId => externalId == input.ExternalId)))
                .Returns(queryGroupRequest)
                .Verifiable();
            _client.Setup(x => x.GroupProxy.GroupQuery(It.Is<IQueryParameters>(rq => rq == queryGroupRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(queryGroupResult)
                .Verifiable();

            var result = await _agentUnderTest.QueryGroupBalance(input).ConfigureAwait(false);

            Assert.IsNotNull(result);
            Assert.AreEqual("bla", result.ErrorMessage);
            Assert.AreEqual("123", result.ErrorCode);
            Assert.IsNotNull(result.BalanceList);
            Assert.AreEqual(1, result.BalanceList.Count);
            Assert.AreEqual(369, result.BalanceList[0].ResourceId);
            Assert.AreEqual("258", result.BalanceList[0].TemplateId);
            Assert.AreEqual("bla balance", result.BalanceList[0].Name);
            Assert.AreEqual("456", result.BalanceList[0].Amount);
            Assert.AreEqual("23", result.BalanceList[0].TresholdLimit);
            Assert.AreEqual("MB", result.BalanceList[0].Unit);
            Assert.AreEqual(queryGroupResult.BalanceInfoList.Values[0].StartTime, result.BalanceList[0].StartTime);
            Assert.AreEqual(queryGroupResult.BalanceInfoList.Values[0].EndTime, result.BalanceList[0].EndTime);

            _messageBuilderUnitOfWork.Verify();
            _client.Verify();
        }

        [TestMethod]
        public async Task QueryGroupBalance_ShouldReturnNullWhenClientResultIsNull()
        {
            var input = new QueryGroupBalanceRequest()
            {
                ExternalId = Guid.NewGuid().ToString()
            };
            var queryGroupRequest = new GroupIdQueryParameters("bla");

            _messageBuilderUnitOfWork.Setup(x => x.Group.GetGroupRequest(It.Is<string>(externalId => externalId == input.ExternalId)))
                .Returns(queryGroupRequest)
                .Verifiable();
            _client.Setup(x => x.GroupProxy.GroupQuery(It.Is<IQueryParameters>(rq => rq == queryGroupRequest), It.IsAny<Endpoint>()))
                .ReturnsAsync(default(GroupQueryResponse))
                .Verifiable();

            var result = await _agentUnderTest.QueryGroupBalance(input).ConfigureAwait(false);

            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task QueryGroupBalance_ShouldThrowWhenMessageBuilderThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Group.GetGroupRequest(It.IsAny<string>()))
                .Throws(new TestException());

            var result = await _agentUnderTest.QueryGroupBalance(new QueryGroupBalanceRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        [ExpectedException(typeof(TestException))]
        public async Task QueryGroupBalance_ShouldThrowWhenClientThrows()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Group.GetGroupRequest(It.IsAny<string>()))
                .Returns(new GroupIdQueryParameters("bla"));
            _client.Setup(x => x.GroupProxy.GroupQuery(It.IsAny<IQueryParameters>(), It.IsAny<Endpoint>()))
                .ThrowsAsync(new TestException());

            var result = await _agentUnderTest.QueryGroupBalance(new QueryGroupBalanceRequest()).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task QueryDeviceSessions_ShouldMapp_DeviceProxy_DeviceSessionQuery_Result_To_DeviceSessionResponse()
        {
            _messageBuilderUnitOfWork.Setup(x => x.Device.GetDeviceSessionsQueryParameters(It.IsAny<string>()))
                .Returns(_deviceSessionIdParameters);

            var result = await _agentUnderTest.QueryDeviceSessions(new Contracts.Device.QueryDeviceSessions()).ConfigureAwait(false);

            var authquantity1 = _responseDeviceSession.ChargingSessionCollection.Values[0].ContextArray.Values[0].AuthQuantity;
            var authquantity2 = _responseDeviceSession.ChargingSessionCollection.Values[0].ContextArray.Values[1].AuthQuantity;
            var authquantity3 = _responseDeviceSession.ChargingSessionCollection.Values[1].ContextArray.Values[0].AuthQuantity;
            var authquantity4 = _responseDeviceSession.ChargingSessionCollection.Values[1].ContextArray.Values[1].AuthQuantity;

            Assert.AreEqual(15, authquantity1);
            Assert.AreEqual(10, authquantity2);
            Assert.AreEqual(14, authquantity3);
            Assert.AreEqual(106, authquantity4);
        }

        [TestMethod]
        public async Task QueryDeviceSessions_ShouldReturn_Empty_DeviceSessionResponse_List_If_DeviceProxy_DeviceSessionQuery_Result_ChargingSessionCollection_Is_Null()
        {
            //Arrange
            _responseDeviceSession.ChargingSessionCollection = null;
            _client.Setup(x => x.DeviceProxy.DeviceSessionQuery(It.IsAny<DeviceSessionIdParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(_responseDeviceSession);

            _messageBuilderUnitOfWork.Setup(x => x.Device.GetDeviceSessionsQueryParameters(It.IsAny<string>()))
                .Returns(_deviceSessionIdParameters);

            //Act
            var result = await _agentUnderTest.QueryDeviceSessions(new Contracts.Device.QueryDeviceSessions()).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task QueryDeviceSessions_ShouldReturn_DeviceSessionResponse_WithErrorCode_If_DeviceProxy_DeviceSessionQuery_Result_Is_Not_Zero()
        {
            //Arrange
            _responseDeviceSession.Result = -2;
            _client.Setup(x => x.DeviceProxy.DeviceSessionQuery(It.IsAny<DeviceSessionIdParameters>(), It.IsAny<Endpoint>()))
                .ReturnsAsync(_responseDeviceSession);

            _messageBuilderUnitOfWork.Setup(x => x.Device.GetDeviceSessionsQueryParameters(It.IsAny<string>()))
                .Returns(_deviceSessionIdParameters);

            //Act
            var result = await _agentUnderTest.QueryDeviceSessions(new Contracts.Device.QueryDeviceSessions()).ConfigureAwait(false);

            //Assert
            Assert.AreEqual(_responseDeviceSession.Result, result[0].Code);
            Assert.AreEqual(1, result.Count);
        }
    }
}
