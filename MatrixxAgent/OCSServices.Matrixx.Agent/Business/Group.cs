using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Group : BaseMessageBuilder, IGroup
    {
        public GroupIdQueryParameters GetGroupRequest(string identifier)
        {
            return new GroupIdQueryParameters(identifier);
        }

        public MultiRequest BuildCreateGroupRequest(AddGroupRequest request)
        {
            var result = new MultiRequest();
            result.RequestCollection = new RequestCollection();


            var groupAdmin = !string.IsNullOrEmpty(request.AdminExternalId) ? new GroupAdmin()
            {
                Subscriber = new Api.Client.Contracts.Request.Search.SearchCollection()
                {
                    ExternalId = request.AdminExternalId
                }
            } : null;

            var createGroupRequest = new CreateGroupRequest
            {
                ExternalId = request.ExternalId,
                Name = request.GroupCode,
                GroupAdmin = groupAdmin,
                NotificationPreference = request.NotificationPreference,
                GroupReAuthPreference = request.GroupReAuthPreference
            };


            var modifyGroupRequest = new ModifyGroupRequest()
            {
                BillingCycleCollection = new Api.Client.Contracts.Model.Subscriber.BillingCycleCollection()
                {
                    BillingCycle = new Api.Client.Contracts.Model.Subscriber.BillingCycle()
                    {
                        BillingCycleId = request.BillingCycleId,
                        DateOffset = request.BillCycleOffset ?? 1
                    }
                },
                GroupSearchData = new Api.Client.Contracts.Request.Search.GroupSearchData()
                {
                    SearchCollection = new Api.Client.Contracts.Request.Search.SearchCollection()
                    {
                        MultiRequestIndex = "0"
                    }
                }
            };

            result.RequestCollection.Values.Add(createGroupRequest);
            result.RequestCollection.Values.Add(modifyGroupRequest);

            return result;
        }

        public ModifyGroupRequest BuildModifyGroupRequest(UpdateGroupRequest request)
        {
            var result = new ModifyGroupRequest
            {
                Name = request.Name,
                ExternalId = request.NewExternalId,
                TierName = request.TierName,
                GroupSearchData = new Api.Client.Contracts.Request.Search.GroupSearchData
                {
                    SearchCollection = new Api.Client.Contracts.Request.Search.SearchCollection { ExternalId = request.ExternalId }
                }
            };
            return result;
        }

        public PurchaseGroupOfferRequest BuidPurchaseGroupOfferRequest(AddOfferToGroupRequest request)
        {
            var result = new PurchaseGroupOfferRequest()
            {
                GroupSearchData = new GroupSearchData
                {
                    SearchCollection = new SearchCollection { ExternalId = request.ExternalId }
                },
                PurchaseInfoList = new OfferRequestArray()
                {
                    Values = new System.Collections.Generic.List<PurchasedOfferData>()
                    {
                        new PurchasedOfferData()
                        {
                            ExternalId = request.OfferCode,
                            CustomPurchaseOfferConfiguration = new CustomPurchaseOfferConfigurationCollection()
                            {
                                CustomPurchaseOfferConfiguration = new CustomPurchaseOfferConfiguration()
                                {
                                    Configuration = request.CustomPurchaseOfferConfigurationParameters
                                }
                            },
                            StartTime  = request.StartTime,
                            EndTime = request.EndTime
                        }
                    }
                }
            };

            return result;
        }

        public DeleteGroupRequest CreateDeleteGroupRequest(RemoveGroupRequest request)
        {
            var result = new DeleteGroupRequest
            {
                GroupSearchData = new Api.Client.Contracts.Request.Search.GroupSearchData
                {
                    SearchCollection = new Api.Client.Contracts.Request.Search.SearchCollection { ExternalId = request.ExternalId }
                }
            };
            return result;
        }

    }
}
