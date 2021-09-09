using log4net;
using Newtonsoft.Json;
using OCSServices.Matrixx.Agent.Business;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts;
using OCSServices.Matrixx.Agent.Contracts.Balance;
using OCSServices.Matrixx.Agent.Contracts.Device;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Offer;
using OCSServices.Matrixx.Agent.Contracts.Sim.Swap;
using OCSServices.Matrixx.Agent.Contracts.Subscriber;
using OCSServices.Matrixx.Agent.Contracts.Threshold;
using OCSServices.Matrixx.Agent.Contracts.Wallet;
using OCSServices.Matrixx.Api.Client.ApiClient;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Wallet;
using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AddImsiToSubscriberRequest = OCSServices.Matrixx.Agent.Contracts.Imsi.AddImsiToSubscriberRequest;
using BalanceInfo = OCSServices.Matrixx.Agent.Contracts.Balance.BalanceInfo;
using CreateSubscriberRequest = OCSServices.Matrixx.Agent.Contracts.Subscriber.CreateSubscriberRequest;
using MultiResponse = OCSServices.Matrixx.Api.Client.Contracts.Response.Multi.MultiResponse;

namespace OCSServices.Matrixx.Agent
{
    public class Agent : IAgent
    {
        private IMessageBuilderUnitOfWork _messageBuilderUnitOfWork;

        private ILog _logger;
        protected ILog Logger => _logger ?? (_logger = LogManager.GetLogger(GetType().Name));

        private readonly IClient _client;

        public Agent(IClient client, IMessageBuilderUnitOfWork messageBuilderUnitOfWork)
        {
            _client = client;
            _messageBuilderUnitOfWork = messageBuilderUnitOfWork;
        }

        public async Task<BasicResponse> AddThresholdToSubscriber(AddThresholdToSubscriberRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Threshold.BuildSubscriberAddThresholdRequest(request);

            var matrixxResponse = await _client.BalanceProxy.SubscriberAddThreshold(matrixxRequest, request.Endpoint).ConfigureAwait(false);


            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> AddThresholdToGroup(AddThresholdToGroupRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Threshold.BuildGroupAddThresholdRequest(request);

            var matrixxResponse = await _client.BalanceProxy.GroupAddThreshold(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<Contracts.MultiResponse> AddGroup(AddGroupRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Group.BuildCreateGroupRequest(request);


            var matrixxResponse = await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            var result = CheckMultiResult(matrixxResponse);

            Logger.Debug($"Agent CreateSubscriberOnMatrixx result: {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public async Task<BasicResponse> CreateGroupAdmin(CreateGroupAdminRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Subscriber.BuildCreateGroupAdmin(request);

            var matrixxResponse = await _client.GroupProxy.CreateGroupAdmin(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> ModifyGroup(UpdateGroupRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Group.BuildModifyGroupRequest(request);
            var matrixxResponse = await _client.GroupProxy.GroupModify(matrixxRequest).ConfigureAwait(false);
            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<AddOfferToSubscriberResponse> AddOfferToGroup(AddOfferToGroupRequest request)
        {

            var purchaseRequest = _messageBuilderUnitOfWork.Group.BuidPurchaseGroupOfferRequest(
                new AddOfferToGroupRequest()
                {
                    ExternalId = request.ExternalId,
                    OfferCode = request.OfferCode,
                    CustomPurchaseOfferConfigurationParameters = request.CustomPurchaseOfferConfigurationParameters,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Endpoint = request.Endpoint
                });

            var result = await _client.GroupProxy.PurchaseGroupOffer(purchaseRequest, request.Endpoint).ConfigureAwait(false);

            return result;
        }

        public async Task<QueryBalanceResponse> QueryBalance(QueryBalanceRequest request)
        {
            Logger.Debug($"Agent QueryBalance: {JsonConvert.SerializeObject(request) }");

            var parameters =
                request.ProductId.HasValue
                ? _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByProductId(request.ProductId.Value)
                : _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByMsisdn(request.Msisdn);

            var result = await GetSubscriber(parameters, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent QueryBalance result: {JsonConvert.SerializeObject(result) }");

            return GetBalanceResponse(result);
        }

        public async Task<QueryWalletResponse> QueryWallet(GetWalletRequest request)
        {
            Logger.Debug($"Agent QueryWallet: {JsonConvert.SerializeObject(request)}");

            var queryRequest = _messageBuilderUnitOfWork.Wallet.GetQueryWalletRequest(request);

            var result = await _client.WalletProxy.QueryWallet(queryRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent QueryWallet result: {JsonConvert.SerializeObject(result)}");

            return GetWalletResponse(result);

        }

        public async Task<QueryBalanceResponse> QueryGroupBalance(QueryGroupBalanceRequest request)
        {
            string methodName = nameof(QueryGroupBalance);
            Logger.Debug($"Agent {methodName}: {JsonConvert.SerializeObject(request) }");

            var parameters = _messageBuilderUnitOfWork.Group.GetGroupRequest(request.ExternalId);

            var response = await _client.GroupProxy.GroupQuery(parameters, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent {methodName} result: {JsonConvert.SerializeObject(response) }");

            var result = GetBalanceResponse(response);
            return result;
        }

        public async Task<QueryWalletResponse> QueryGroupWallet(GetGroupWalletRequest request)
        {

            string methodName = nameof(QueryGroupBalance);

            Logger.Debug($"Agent {methodName}: {JsonConvert.SerializeObject(request) }");

            var parameters = _messageBuilderUnitOfWork.Wallet.GetQueryGroupWalletRequest(request);

            var response = await _client.WalletProxy.QueryGroupWallet(parameters, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent {methodName} result: {JsonConvert.SerializeObject(response) }");

            var result = GetGroupWalletResponse(response);

            return result;
        }

        private async Task<FlexTopupResponse> FlexTopup(FlexTopupRequest request, BalanceInfo balance)
        {
            var balanceAdjustmentRequest = _messageBuilderUnitOfWork.Balance.GetAdjustBalanceRequest(request, balance.ResourceId);

            var result = await _client.BalanceProxy.SubscriberAdjustBalance(balanceAdjustmentRequest, request.Endpoint).ConfigureAwait(false);

            return new FlexTopupResponse
            {
                ErrorCode = result.Code.ToString(),
                ErrorMessage = result.Text
            };
        }

        private async Task<FlexTopupResponse> FlexTopup(FlexTopupRequest request, QueryBalanceResponse balances)
        {
            var balance = balances.BalanceList.FirstOrDefault(b => b.Name == request.BalanceType);
            return
                (balance != null)
                ? await FlexTopup(request, balance).ConfigureAwait(false)
                : new FlexTopupResponse
                {
                    ErrorCode = "404",
                    ErrorMessage = "Balance not found"
                };
        }

        public async Task<FlexTopupResponse> FlexTopup(FlexTopupRequest request)
        {
            Logger.Debug($"Agent FlexTopup: {JsonConvert.SerializeObject(request) }");

            FlexTopupResponse result = null;

            if (request.IsQuota)
            {
                return await FlexTopupViaAddThreshold(request).ConfigureAwait(false);
            }
            if (request.PurchaseOffer)
            {
                return await FlexTopupWithNewOffer(request).ConfigureAwait(false);
            }
            var balances = await QueryBalance(new QueryBalanceRequest
            {
                Msisdn = request.Msisdn,
                ProductId = request.ProductId,
                Endpoint = request.Endpoint
            }).ConfigureAwait(false);

            result =
                (balances.ErrorCode == "0")
                    ? await FlexTopup(request, balances).ConfigureAwait(false)
                    : new FlexTopupResponse
                    {
                        ErrorCode = balances.ErrorCode,
                        ErrorMessage = balances.ErrorMessage
                    };

            Logger.Debug($"Agent FlexTopup result: {JsonConvert.SerializeObject(result) }");

            return result;
        }

        private async Task<FlexTopupResponse> FlexTopupWithNewOffer(FlexTopupRequest request)
        {
            AddOfferToSubscriberResponse addOfferResult = await AddOfferToSubscriber(
                new AddOfferToSubscriberRequest()
                {
                    CrmProductId = (Guid)request.ProductId,
                    OffersToBePurchased = new System.Collections.Generic.List<string>() { request.OfferName },
                    Endpoint = request.Endpoint
                }
                ).ConfigureAwait(false);

            if (addOfferResult.Result == 0)
            {
                int? resourceId = null;

                var balances = addOfferResult.PurchaseInfoList?.Values.FirstOrDefault(p => p.ProductOfferExternalId == request.OfferName)?.Balances;
                if (balances == null || balances.Values == null || balances.Values.Count < 1)
                {
                    return new FlexTopupResponse() { ErrorCode = "404", ErrorMessage = $"Missing {nameof(balances)} - could not be found" };
                }

                resourceId = balances.Values.FirstOrDefault(b => b.TemplateId == request.TemplateId)?.ResourceId;
                if (resourceId == null)
                {
                    return new FlexTopupResponse() { ErrorCode = "404", ErrorMessage = "ResourceId could not be found" };
                }

                var balance = new BalanceInfo() { Amount = request.Amount.ToString(), EndTime = request.EndTime, Name = request.BalanceType, ResourceId = resourceId.Value };
                return await FlexTopup(request, balance).ConfigureAwait(false);
            }
            return new FlexTopupResponse() { ErrorCode = addOfferResult.Result.ToString(), ErrorMessage = addOfferResult.ResultText };
        }

        private async Task<FlexTopupResponse> FlexTopupViaAddThreshold(FlexTopupRequest request)
        {

            var req = new AddThresholdToSubscriberRequest();
            req.Amount = request.Amount;
            req.CrmProductId = request.ProductId.Value;
            req.ThresholdId = request.TemplateId;
            req.ResourceId = 12345; //TODO: lookup resourceid
            req.Endpoint = request.Endpoint;

            var addThresHoldResult = await AddThresholdToSubscriber(req).ConfigureAwait(false);

            return new FlexTopupResponse() { ErrorCode = addThresHoldResult.Code.ToString(), ErrorMessage = addThresHoldResult.Text };
        }

        public async Task<SubscriberQueryResponse> GetSubscriber(IQueryParameters parameters, Endpoint endpoint)
        {
            Logger.Debug($"Agent GetSubscriber");

            var result = await _client.SubscriberProxy.SubscriberQuery(parameters, endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent GetSubscriber result: {JsonConvert.SerializeObject(result)}");

            return result;
        }
        public async Task<GetPurchasedOffersResponse> GetPurchasedOffers(GetPurchasedOffersRequest request)
        {
            Logger.Debug($"Agent QueryBalance: {JsonConvert.SerializeObject(request) }");

            var parameters =
               request.ProductId.HasValue
               ? _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByProductId(request.ProductId.Value)
               : _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByMsisdn(request.MsIsdn);

            //Added null because of endpoints.
            var result = await GetSubscriber(parameters,null).ConfigureAwait(false);

            Logger.Debug($"Agent QueryBalance result: {JsonConvert.SerializeObject(result) }");

            return GetPurchasedOffersResponse(result);
        }

        public async Task<Contracts.MultiResponse> SetCustomSubscriberConfiguration(SetCustomConfigurationRequest request)
        {
            Logger.Debug($"Agent SetCustomSubscriberConfiguration: {JsonConvert.SerializeObject(request)}");

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Subscriber.BuildSetCustomSubscriberConfigurationRequest(request);

            var result = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent SetCustomSubscriberConfiguration result: {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public async Task<List<DeviceSessionResponse>> QueryDeviceSessions(QueryDeviceSessions queryDeviseSessions)
        {
            Logger.Debug($"Agent QueryDeviceSessions: {JsonConvert.SerializeObject(queryDeviseSessions)}");

            var queryDeviceSessionsParams = _messageBuilderUnitOfWork.Device.GetDeviceSessionsQueryParameters(queryDeviseSessions.DeviceId);

            var result = await _client.DeviceProxy.DeviceSessionQuery(queryDeviceSessionsParams, queryDeviseSessions.Endpoint);

            return TranslateDeviceQueryResponse(result);
        }

        private List<DeviceSessionResponse> TranslateDeviceQueryResponse(ResponseDeviceSession result)
        {
            var translatedSessions = new List<DeviceSessionResponse>();

            if (result.Result != 0)
            {
                translatedSessions.Add(new DeviceSessionResponse { Code = result.Result });
                return translatedSessions;
            }

            if (result.Result == 0 && (result?.ChargingSessionCollection?.Values == null || !result.ChargingSessionCollection.Values.Any()))
            {
                return translatedSessions;
            }
            else
            {
                foreach (var chargingSessionCollection in result.ChargingSessionCollection.Values)
                {
                    foreach (var contextArray in chargingSessionCollection.ContextArray.Values)
                    {
                        translatedSessions.Add(new DeviceSessionResponse
                        {
                            AuthQuantity = contextArray.AuthQuantity,
                            Code = result.Result,
                            QuantityUnit = contextArray.QuantityUnit,
                            SessionStartTime = chargingSessionCollection.SessionStartTime
                        });
                    }
                }
            }

            return translatedSessions;
        }

        public async Task<AddOfferToSubscriberResponse> AddOfferToSubscriber(AddOfferToSubscriberRequest request)
        {
            Logger.Debug($"Agent AddOfferToSubscriber: {JsonConvert.SerializeObject(request)}");

            PurchaseOfferForSubscriberRequest purchaseRequest = _messageBuilderUnitOfWork.Offer.BuildPurchaseOfferForSubscriberRequest(request);

            var addOfferToSubscriberResponse = await _client.OfferProxy.AddOfferToSubscriber(purchaseRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent AddOfferToSubscriberResponse result: {JsonConvert.SerializeObject(addOfferToSubscriberResponse)}");

            return addOfferToSubscriberResponse;
        }

        public async Task<Contracts.MultiResponse> CreateSubscriberOnMatrixx(CreateSubscriberRequest request)
        {
            Logger.Debug($"Agent CreateSubscriberOnMatrixx: {JsonConvert.SerializeObject(request)}");

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Subscriber.BuildCreateSubscriberRequest(request);

            var result = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent CreateSubscriberOnMatrixx result: {JsonConvert.SerializeObject(result)}");

            return result;
        }


        public async Task<Contracts.MultiResponse> SetSubscriberStatusOnMatrixx(SetSubscriberStatusRequest request)
        {
            Logger.Debug($"Agent SetSubscriberStatusOnMatrixx: {JsonConvert.SerializeObject(request)}");

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Subscriber.BuildSetStatusRequest(request);

            var result = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent SetSubscriberStatusOnMatrixx result: {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public async Task<Contracts.MultiResponse> ValidateSessionForDeviceList(ValidateSessionForDeviceListRequest request)
        {
            Logger.Debug($"Agent ValidateSessionForDeviceList: {JsonConvert.SerializeObject(request)}");

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Device.CreateValidateDeviceListSession(request);

            var result = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent ValidateSessionForDeviceList result: {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public async Task<BasicResponse> SwapMsisdn(SwapMsIsdnRequest request)
        {
            Logger.Debug($"Agent SwapMsisdn: {JsonConvert.SerializeObject(request)}");

            var parameters = _messageBuilderUnitOfWork.Device.GetMsisdnDeviceQueryParameters(request.OldMsIsdn);

            var deviceQueryResponse = await _client.DeviceProxy.DeviceQuery(parameters, request.Endpoint).ConfigureAwait(false);

            Api.Client.Contracts.Request.Device.DeviceModifyRequest matrixxRequest = _messageBuilderUnitOfWork.Device.BuildSwapMsIsdnRequest(deviceQueryResponse, request);

            var matrixxResponse = await _client.DeviceProxy.DeviceModify(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent SwapMsisdn result: {JsonConvert.SerializeObject(matrixxResponse)}");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }
        public async Task<BasicResponse> UpdateMsisdnList(UpdateMsisdnListRequest request)
        {
            Logger.Debug($"Agent UpdateMsisdnList: {JsonConvert.SerializeObject(request)}");

            var parameters = _messageBuilderUnitOfWork.Device.GetMsisdnDeviceQueryParameters(request.PrimaryMsisdn);

            var deviceQueryResponse = await _client.DeviceProxy.DeviceQuery(parameters, request.Endpoint).ConfigureAwait(false);

            Api.Client.Contracts.Request.Device.DeviceModifyRequest matrixxRequest = _messageBuilderUnitOfWork.Device.BuildUpdateMsisdnListRequest(deviceQueryResponse, request);


            var matrixxResponse = await _client.DeviceProxy.DeviceModify(matrixxRequest, request.Endpoint).ConfigureAwait(false);


            Logger.Debug($"Agent UpdateMsisdnList result: {JsonConvert.SerializeObject(matrixxResponse)}");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> UpdateContactPhoneNumber(UpdateContactPhoneNumberRequest request)
        {
            Logger.Debug($"Agent UpdateContactPhoneNumber: {JsonConvert.SerializeObject(request)}");

            var modifySubscriberRequest = _messageBuilderUnitOfWork.Subscriber.CreateModifySubscriberContactPhoneNumberRequest(request);

            var subscriberModifyResponse = await _client.SubscriberProxy.SubscriberModify(modifySubscriberRequest, request.Endpoint);



            return new BasicResponse
            {
                Code = subscriberModifyResponse.Code,
                Text = subscriberModifyResponse.Text
            };

        }

        public async Task<Contracts.MultiResponse> SwapSim(SwapSimRequest request)
        {
            Logger.Debug($"Agent SwapSim: {JsonConvert.SerializeObject(request)}");

            List<string> msisdns = new List<string>();

            foreach (var imsi in request.OldImsis)
            {
                var parameters = _messageBuilderUnitOfWork.Device.GetImsiDeviceQueryParameters(imsi);

                var deviceQueryResponse = await _client.DeviceProxy.DeviceQuery(parameters, request.Endpoint).ConfigureAwait(false);

                if (deviceQueryResponse.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList != null)
                {
                    msisdns.AddRange(deviceQueryResponse.MobileDeviceExtensionCollection.MobileDeviceExtension.AccessNumberList.Values.Except(msisdns));
                }
            }

            DetachImsiFromSubscriberRequest detachImsiRequest = new DetachImsiFromSubscriberRequest
            {
                CrmProductId = request.CrmProductId,
                Imsis = request.OldImsis
            };

            Contracts.MultiResponse createResult;

            SetSubscriberFirstNameRequest setSubscriberFirstNameRequest = new SetSubscriberFirstNameRequest
            {
                CrmProductId = request.CrmProductId,
                FirstName = request.Imsis.FirstOrDefault()
            };


            MultiRequest detachRequest = _messageBuilderUnitOfWork.Subscriber.CreateDetachImsiFromSubscriber(detachImsiRequest);
            var detachResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(detachRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Detach IMSI/Device from subscriber result: {JsonConvert.SerializeObject(detachResult)}");

            MultiRequest deleteRequest = _messageBuilderUnitOfWork.Device.CreateDeviceDeleteRequestList(request.OldImsis);
            var deleteResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(deleteRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Delete IMSI/Device result: {JsonConvert.SerializeObject(deleteResult)}");

            MultiRequest createRequest = _messageBuilderUnitOfWork.Device.CreateAddImsisRequest(request, msisdns);
            createResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(createRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Create IMSI/Device result: {JsonConvert.SerializeObject(createResult)}");

            MultiRequest modifyRequest = _messageBuilderUnitOfWork.Subscriber.CreateModifySubscriberFirstNameRequest(setSubscriberFirstNameRequest);
            var modifyResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(modifyRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Modify Subscriber First Name result: {JsonConvert.SerializeObject(modifyResult)}");


            return createResult;
        }

        public async Task<Contracts.MultiResponse> AddImsiToSubscriber(AddImsiToSubscriberRequest request)
        {
            Logger.Debug($"Agent AddImsiToSubscriber: {JsonConvert.SerializeObject(request)}");

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Device.CreateAddImsiToSubscriberRequest(request);

            var result = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent AddImsiToSubscriber result: {JsonConvert.SerializeObject(result)}");

            return result;
        }

        public async Task<Contracts.MultiResponse> RemoveImsiFromSubscriber(RemoveImsiFromSubscriberRequest request)
        {
            Logger.Debug($"Agent RemoveImsiFromSubscriber: {JsonConvert.SerializeObject(request)}");

            DetachImsiFromSubscriberRequest detachImsiRequest = new DetachImsiFromSubscriberRequest
            {
                CrmProductId = request.ProductId,
                Imsis = new List<string>() { request.Imsi }
            };

            MultiRequest detachRequest = _messageBuilderUnitOfWork.Subscriber.CreateDetachImsiFromSubscriber(detachImsiRequest);

            var detachResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(detachRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent Detach IMSI/Device from subscriber result: {JsonConvert.SerializeObject(detachResult)}");

            MultiRequest deleteRequest = _messageBuilderUnitOfWork.Device.CreateDeviceDeleteRequestList(new List<string>() { request.Imsi });

            var deleteResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(deleteRequest, request.Endpoint).ConfigureAwait(false));

            Logger.Debug($"Agent Delete IMSI/Device result: {JsonConvert.SerializeObject(deleteResult)}");

            return deleteResult;
        }

        public async Task<Contracts.MultiResponse> DeleteSubscriberOnMatrixx(DeactiveSubscriberRequest request)
        {
            var setSubscriberStatusRequest = new SetSubscriberStatusRequest
            {
                CrmProductId = request.CrmProductId,
                Status = request.Status
            };

            DetachImsiFromSubscriberRequest detachImsiRequest = new DetachImsiFromSubscriberRequest
            {
                CrmProductId = request.CrmProductId,
                Imsis = request.Imsis
            };

            MultiRequest matrixxRequest = _messageBuilderUnitOfWork.Subscriber.BuildSetStatusRequest(setSubscriberStatusRequest);
            var updateSubscriberStatus = CheckMultiResult(await _client.MultiProxy.RequestMulti(matrixxRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent SetSubscriberStatusOnMatrixx result: {JsonConvert.SerializeObject(updateSubscriberStatus)}");

            MultiRequest detachRequest = _messageBuilderUnitOfWork.Subscriber.CreateDetachImsiFromSubscriber(detachImsiRequest);
            var detachResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(detachRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Detach IMSI/Device from subscriber result: {JsonConvert.SerializeObject(detachResult)}");

            MultiRequest deleteRequest = _messageBuilderUnitOfWork.Subscriber.CreateDeleteSubscriberRequest(request);
            var deleteResult = CheckMultiResult(await _client.MultiProxy.RequestMulti(deleteRequest, request.Endpoint).ConfigureAwait(false));
            Logger.Debug($"Agent Detach IMSI/Device from subscriber result: {JsonConvert.SerializeObject(deleteResult)}");

            return deleteResult;

        }

        public async Task<BasicResponse> DeleteGroup(RemoveGroupRequest request)
        {
            var matrixxRequest = _messageBuilderUnitOfWork.Group.CreateDeleteGroupRequest(request);
            var matrixxResponse = await _client.GroupProxy.RemoveGroup(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        private Contracts.MultiResponse CheckMultiResult(MultiResponse result)
        {
            return new Contracts.MultiResponse
            {
                Code = result.Code,
                Text = result.Text,
                RepsonseList = result.ResponseCollection?.ResponseList?.Select(t => new Contracts.BasicResponse
                {
                    Code = (t as MatrixxResponse).Code,
                    Text = (t as MatrixxResponse).Text
                }).ToList()
            };
        }

        public async Task<BasicResponse> CancelOfferForSubscriber(Contracts.Requests.CancelOfferForSubscriberRequest request)
        {
            Logger.Debug($"Agent CancelOfferForSubscriber: {JsonConvert.SerializeObject(request) }");

            var matrixxRequest = _messageBuilderUnitOfWork.Offer.BuildCancelOfferForSubscriberRequest(request);

            var matrixxResponse = await _client.OfferProxy.CancelOfferForSubscriber(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent CancelOfferForSubscriber result: {JsonConvert.SerializeObject(matrixxResponse) }");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> CancelOfferForGroup(Contracts.Requests.CancelOfferForGroupRequest request)
        {
            Logger.Debug($"Agent CancelOfferForGroup: {JsonConvert.SerializeObject(request) }");

            var matrixxRequest = _messageBuilderUnitOfWork.Offer.BuildCancelOfferForGroupRequest(request);

            var matrixxResponse = await _client.OfferProxy.CancelOfferForGroup(matrixxRequest).ConfigureAwait(false);

            Logger.Debug($"Agent CancelOfferForGroup result: {JsonConvert.SerializeObject(matrixxResponse) }");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> AdjustBalanceSubscriber(AdjustBalanceForSubscriberRequest request)
        {
            Logger.Debug($"Agent AdjustBalanceSubscriber: {JsonConvert.SerializeObject(request) }");

            var balanceAdjustmentRequest = _messageBuilderUnitOfWork.Balance.GetAdjustBalanceRequest(request);

            var result = await _client.BalanceProxy.SubscriberAdjustBalance(balanceAdjustmentRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent AdjustBalanceSubscriber result: {JsonConvert.SerializeObject(result) }");

            return new BasicResponse
            {
                Code = result.Code,
                Text = result.Text
            };
        }

        public async Task<BasicResponse> AdjustBalanceGroup(AdjustBalanceForGroupRequest request)
        {
            Logger.Debug($"Agent AdjustBalanceGroup: {JsonConvert.SerializeObject(request) }");

            var balanceAdjustmentRequest = _messageBuilderUnitOfWork.Balance.GetGroupAdjustBalanceRequest(request);

            var result = await _client.BalanceProxy.GroupAdjustBalance(balanceAdjustmentRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent AdjustBalanceGroup result: {JsonConvert.SerializeObject(result) }");

            return new BasicResponse
            {
                Code = result.Code,
                Text = result.Text
            };
        }

        public async Task<BasicResponse> SetThresholdToSubscriberToInfinity(SetThresholdSubscriberToInfinityRequest request)
        {
            Logger.Debug($"Agent SetThresholdToSubscriberToInfinity: {JsonConvert.SerializeObject(request) }");

            var matrixxRequest = _messageBuilderUnitOfWork.Threshold.BuildSubscriberSetThresholdToInfinityRequest(request);

            var matrixxResponse = await _client.BalanceProxy.SubscriberSetThresholdToInfinity(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent SetThresholdToSubscriberToInfinity result: {JsonConvert.SerializeObject(matrixxResponse) }");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<QueryBalanceResponse> QueryBalanceSharedSubscriber(QueryBalanceRequest request)
        {
            Logger.Debug($"Agent QueryBalance: {JsonConvert.SerializeObject(request) }");

            var parameters =
                request.ProductId.HasValue
                ? _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByProductId(request.ProductId.Value)
                : _messageBuilderUnitOfWork.Balance.GetQueryBalanceParametersByMsisdn(request.Msisdn);

            var result = await GetSubscriber(parameters, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent QueryBalance result: {JsonConvert.SerializeObject(result) }");

            return GetBalanceResponseForSharedSubscriber(result);
        }

        public async Task<BasicResponse> ModifyOfferSubscriber(Contracts.Offer.ModifyOfferForSubscriberRequest request)
        {
            Logger.Debug($"Agent ModifyOfferSubscriber: {JsonConvert.SerializeObject(request) }");

            var matrixxRequest = _messageBuilderUnitOfWork.Offer.BuildModifyOfferForSubscriberRequest(request);

            var matrixxResponse = await _client.OfferProxy.ModifyOfferForSubscriber(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent ModifyOfferSubscriber result: {JsonConvert.SerializeObject(matrixxResponse)}");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        public async Task<BasicResponse> ModifyOfferGroup(Contracts.Offer.ModifyOfferForGroupRequest request)
        {
            Logger.Debug($"Agent ModifyOfferGroup: {JsonConvert.SerializeObject(request) }");

            var matrixxRequest = _messageBuilderUnitOfWork.Offer.BuildModifyOfferForGroupRequest(request);

            var matrixxResponse = await _client.OfferProxy.ModifyOfferForGroup(matrixxRequest, request.Endpoint).ConfigureAwait(false);

            Logger.Debug($"Agent ModifyOfferGroup result: {JsonConvert.SerializeObject(matrixxResponse)}");

            return new BasicResponse
            {
                Code = matrixxResponse.Code,
                Text = matrixxResponse.Text
            };
        }

        #region Translators

        private QueryBalanceResponse GetBalanceResponse(GroupQueryResponse result)
        {
            if (result == null)
            {
                return null;
            }

            return
                new QueryBalanceResponse
                {
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = result.ResultText,
                    BalanceList =
                        result.BalanceInfoList?.Values
                            .Select(b => new BalanceInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList()
                };
        }

        private QueryBalanceResponse GetBalanceResponse(SubscriberQueryResponse result)
        {
            if (result == null)
            {
                return null;
            }

            return
                new QueryBalanceResponse
                {
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = result.ResultText,
                    BalanceList =
                        result.BalanceInfoList?.Values
                            .Where(t => !t.IsAggregate)
                            .Select(b => new BalanceInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList()
                };
        }

        private QueryWalletResponse GetWalletResponse(SubscriberWalletResponse result)
        {
            if (result == null)
            {
                return null;
            }

            var response =
                new QueryWalletResponse
                {
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = result.ResultText,
                    SimpleBalanceList =
                        result.WalletInfoList?.Values
                            .Where(t => !t.IsAggregate)
                            .Select(b => new WalletInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                ReservedAmount = b.ReservedAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList(),
                    PeriodicBalanceList =
                        result.WalletInfoPeriodicList?.Values
                            .Where(t => !t.IsAggregate)
                            .Select(b => new WalletInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                ReservedAmount = b.ReservedAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList(),
                    SimpleAggregatedBalanceList =
                        result.WalletInfoList?.Values
                            .Where(t => t.IsAggregate)
                            .Select(a => new WalletInfo
                            {
                                ResourceId = a.ResourceId,
                                TemplateId = a.TemplateId,
                                Name = a.Name,
                                Amount = a.AvailableAmount,
                                ReservedAmount = a.ReservedAmount,
                                TresholdLimit = a.ThresholdLimit,
                                Unit = a.QuantityUnit,
                                StartTime = a.StartTime,
                                EndTime = a.EndTime
                            }).ToList(),
                    PeriodicAggregateBalanceList =
                        result.WalletInfoPeriodicList?.Values
                            .Where(t => t.IsAggregate)
                            .Select(b => new WalletInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                ReservedAmount = b.ReservedAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList()
                };
            response.MatrixxBillCycleList = new List<BillCycleInfo>();

            if (result.WalletInfoBillingCycleList != null && result.WalletInfoBillingCycleList.Values != null)
            {
                foreach (var billcyleInfo in result.WalletInfoBillingCycleList.Values)
                {
                    if (billcyleInfo == null)
                    {
                        continue;
                    }

                    response.MatrixxBillCycleList.Add(new BillCycleInfo
                    {
                        BillingCycleId = billcyleInfo.BillingCycleId,
                        CurrentPeriodDuration = billcyleInfo.CurrentPeriodDuration,
                        CurrentPeriodEndTime = billcyleInfo.CurrentPeriodEndTime,
                        CurrentPeriodOffset = billcyleInfo.CurrentPeriodOffset,
                        CurrentPeriodStartTime = billcyleInfo.CurrentPeriodStartTime,
                        Period = billcyleInfo.Period,
                        PeriodInterval = billcyleInfo.PeriodInterval,
                        DateOffset = billcyleInfo.DateOffset
                    });
                }
            }

            return response;
        }

        private QueryWalletResponse GetGroupWalletResponse(SubscriberWalletResponse result)
        {
            if (result == null)
            {
                return null;
            }

            return
                new QueryWalletResponse
                {
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = result.ResultText,
                    SimpleBalanceList =
                        result.WalletInfoList?.Values
                            .Select(b => new WalletInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                ReservedAmount = b.ReservedAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList(),
                    PeriodicBalanceList =
                        result.WalletInfoPeriodicList?.Values
                            .Select(b => new WalletInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                ReservedAmount = b.ReservedAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList()
                };
        }

        private GetPurchasedOffersResponse GetPurchasedOffersResponse(SubscriberQueryResponse result)
        {
            if (result == null)
            {
                return null;
            }

            return new GetPurchasedOffersResponse
            {
                ErrorCode = result.Result.ToString(),
                ErrorMessage = result.ResultText,
                PurchasedOfferCollection = result
                    ?.PurchaseInfoList
                    ?.Values
                    ?.Select(x => new PurchaseOfferInfo
                    {
                        ProductOfferExternalId = x.ProductOfferExternalId,
                        ProductOfferId = x.ProductOfferId,
                        ProductOfferVersion = x.ProductOfferVersion,
                        PurchaseTime = x.PurchaseTime,
                        ResourceId = x.ResourceId,
                        Status = x.Status
                    })?.ToList()
            };
        }

        private QueryGroupResponse GetGroupResponse(GroupQueryResponse response)
        {
            if (response == null)
            {
                return null;
            }

            return new QueryGroupResponse
            {
                Name = response.Name,
                CustomReference = response.ExternalId,
                ErrorMessage = response.ResultText,
                ErrorCode = response.Result,
                Status = response.Status,
                ObjectId = response.ObjectId,
                AdminCount = response.AdminCount,
                AdminCursor = response.AdminCursor,
                GroupMemberCount = response.GroupMemberCount,
                GroupMemberCursor = response.GroupMemberCursor,
                NotificationPreference = response.NotificationPreference,
                StatusDescription = response.StatusDescription,
                SubscriberCount = response.SubscriberCount,
                SubscriberMemberCount = response.SubscriberMemberCount,
                SubscriberMemberCursor = response.SubscriberMemberCursor
            };
        }

        private QueryBalanceResponse GetBalanceResponseForSharedSubscriber(SubscriberQueryResponse result)
        {
            if (result == null)
            {
                return null;
            }

            return
                new QueryBalanceResponse
                {
                    ErrorCode = result.Result.ToString(),
                    ErrorMessage = result.ResultText,
                    BalanceList =
                        result.BalanceInfoList?.Values
                            .Select(b => new BalanceInfo
                            {
                                ResourceId = b.ResourceId,
                                TemplateId = b.TemplateId,
                                Name = b.Name,
                                Amount = b.AvailableAmount,
                                TresholdLimit = b.ThresholdLimit,
                                Unit = b.QuantityUnit,
                                StartTime = b.StartTime,
                                EndTime = b.EndTime
                            }).ToList()
                };
        }
        #endregion

        #region disposable
        private bool _disposed;

        ~Agent()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _messageBuilderUnitOfWork.Dispose();
                _client.Dispose();
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

        #endregion
    }
}
