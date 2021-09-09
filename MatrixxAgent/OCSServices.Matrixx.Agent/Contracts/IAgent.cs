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
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber;
using SplitProvisioning.Base.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent
{
    public interface IAgent : System.IDisposable
    {
        Task<BasicResponse> CreateGroupAdmin(CreateGroupAdminRequest request);
        Task<BasicResponse> AddThresholdToSubscriber(AddThresholdToSubscriberRequest request);
        Task<BasicResponse> AddThresholdToGroup(AddThresholdToGroupRequest request);
        Task<MultiResponse> AddGroup(AddGroupRequest request);
        Task<BasicResponse> ModifyGroup(UpdateGroupRequest request);
        Task<AddOfferToSubscriberResponse> AddOfferToGroup(AddOfferToGroupRequest request);
        Task<AddOfferToSubscriberResponse> AddOfferToSubscriber(AddOfferToSubscriberRequest request);
        Task<MultiResponse> CreateSubscriberOnMatrixx(CreateSubscriberRequest request);
        Task<FlexTopupResponse> FlexTopup(FlexTopupRequest request);
        Task<QueryBalanceResponse> QueryBalance(QueryBalanceRequest request);
        Task<QueryBalanceResponse> QueryGroupBalance(QueryGroupBalanceRequest request);
        Task<MultiResponse> SetCustomSubscriberConfiguration(SetCustomConfigurationRequest request);
        Task<MultiResponse> AddImsiToSubscriber(AddImsiToSubscriberRequest request);
        Task<MultiResponse> RemoveImsiFromSubscriber(RemoveImsiFromSubscriberRequest request);
        Task<MultiResponse> SetSubscriberStatusOnMatrixx(SetSubscriberStatusRequest request);
        Task<MultiResponse> ValidateSessionForDeviceList(ValidateSessionForDeviceListRequest validateSessionForDeviceListRequest);
        Task<BasicResponse> SwapMsisdn(SwapMsIsdnRequest request);
        Task<MultiResponse> SwapSim(SwapSimRequest request);
        Task<MultiResponse> DeleteSubscriberOnMatrixx(DeactiveSubscriberRequest request);
        Task<BasicResponse> DeleteGroup(RemoveGroupRequest request);
        Task<BasicResponse> CancelOfferForSubscriber(Contracts.Requests.CancelOfferForSubscriberRequest request);
        Task<GetPurchasedOffersResponse> GetPurchasedOffers(GetPurchasedOffersRequest request);
        Task<SubscriberQueryResponse> GetSubscriber(IQueryParameters parameters, Endpoint endpoint);
  
        //SubscriberWalletResponse GetSubscriberWallet(GetWalletRequest request);
        Task<QueryWalletResponse> QueryWallet(GetWalletRequest request);
        Task<QueryWalletResponse> QueryGroupWallet(GetGroupWalletRequest request);
        Task<BasicResponse> CancelOfferForGroup(Contracts.Requests.CancelOfferForGroupRequest request);
        Task<BasicResponse> AdjustBalanceSubscriber(AdjustBalanceForSubscriberRequest request);
        Task<BasicResponse> AdjustBalanceGroup(AdjustBalanceForGroupRequest request);
        Task<BasicResponse> SetThresholdToSubscriberToInfinity(SetThresholdSubscriberToInfinityRequest request);
        Task<QueryBalanceResponse> QueryBalanceSharedSubscriber(QueryBalanceRequest request);
        Task<BasicResponse> ModifyOfferSubscriber(ModifyOfferForSubscriberRequest request);
        Task<BasicResponse> ModifyOfferGroup(ModifyOfferForGroupRequest request);
        Task<BasicResponse> UpdateMsisdnList(UpdateMsisdnListRequest updateMsisdnListRequest);
        Task<BasicResponse> UpdateContactPhoneNumber(UpdateContactPhoneNumberRequest updateContactPhoneNumberRequest);
        Task<List<DeviceSessionResponse>> QueryDeviceSessions(QueryDeviceSessions queryDeviseSessions);
    }
}