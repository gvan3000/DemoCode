using System.Collections.Generic;
using System.Linq;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Offer;
using OCSServices.Matrixx.Agent.Contracts.Requests;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Offer : BaseMessageBuilder, IOffer
    {
        public api.PurchaseOfferForSubscriberRequest BuildPurchaseOfferForSubscriberRequest(AddOfferToSubscriberRequest request)
        {
            var result = new api.PurchaseOfferForSubscriberRequest
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.CrmProductId.ToString().ToUpper()
                    }
                }
            };

            if (request.CustomPurchaseOfferConfigurationParameters != null &&
                request.CustomPurchaseOfferConfigurationParameters.Any())
            {
                result.PurchaseInfoList = new OfferRequestArray
                {
                    Values = request.OffersToBePurchased.Select(t => new PurchasedOfferData
                    {
                        ExternalId = t,
                        StartTime = request.StartTime,
                        EndTime = request.EndTime,
                        CustomPurchaseOfferConfiguration = new CustomPurchaseOfferConfigurationCollection
                        {
                            CustomPurchaseOfferConfiguration =
                                SetCustomPurchaseOfferConfiguration(
                                    request.CustomPurchaseOfferConfigurationParameters)
                        }
                    }).ToList()
                };
            }
            else
            {
                result.PurchaseInfoList = new OfferRequestArray
                {
                    Values = request.OffersToBePurchased.Select(t => new PurchasedOfferData
                    {
                        ExternalId = t,
                        StartTime = request.StartTime,
                        EndTime = request.EndTime,
                    }).ToList()
                };
            }
            return result;
        }

        private CustomPurchaseOfferConfiguration SetCustomPurchaseOfferConfiguration(
            Dictionary<string, string> customConfigurationParameters)
        {
            if (customConfigurationParameters == null || customConfigurationParameters.Count < 1)
                return null;

            var customPurchaseConfiguration = new CustomPurchaseOfferConfiguration();

            customPurchaseConfiguration.Configuration = customConfigurationParameters;
            return customPurchaseConfiguration;
        }

        public api.CancelOfferForSubscriberRequest BuildCancelOfferForSubscriberRequest(Contracts.Requests.CancelOfferForSubscriberRequest request)
        {
            return new api.CancelOfferForSubscriberRequest
            {
                ObjectId = request.ObjectId,
                ResourceIdArray = string.Join(",", request.OfferIds)
            };
        }

        public api.CancelOfferForGroupRequest BuildCancelOfferForGroupRequest(Contracts.Requests.CancelOfferForGroupRequest request)
        {
            return new api.CancelOfferForGroupRequest()
            {
                ObjectId = request.ObjectId,
                ResourceIdArray = string.Join(",", request.OfferIds)
            };
        }

        public api.ModifyOfferForSubscriberRequest BuildModifyOfferForSubscriberRequest(
            Contracts.Offer.ModifyOfferForSubscriberRequest request)
        {
            return new api.ModifyOfferForSubscriberRequest()
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection
                    {
                        ExternalId = request.ProductId.ToString().ToUpper()
                    }
                },
                OfferResourceId = request.OfferResourceId,
                EndTime = request.EndTime,
                StartTime = request.StartTime
            };
        }

        public api.ModifyOfferForGroupRequest BuildModifyOfferForGroupRequest(Contracts.Offer.ModifyOfferForGroupRequest request)
        {
            return new api.ModifyOfferForGroupRequest()
            {
                GroupSearchData = new GroupSearchData
                {
                    SearchCollection = new SearchCollection()
                    {
                        ExternalId = request.BusinessUnitId.ToString().ToUpper()
                    }
                },
                OfferResourceId = request.OfferResourceId,
                EndTime = request.EndTime,
                StartTime = request.StartTime
            };
        }     
    }
}
