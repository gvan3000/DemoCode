using System;
using OCSServices.Matrixx.Agent.Business.Interfaces;
using OCSServices.Matrixx.Agent.Contracts.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;
using OCSServices.Matrixx.Api.Client.Contracts.Types;

namespace OCSServices.Matrixx.Agent.Business
{
    public class Balance : BaseMessageBuilder, IBalance
    {
        public IQueryParameters GetQueryBalanceParametersByProductId(Guid productId)
        {
            return new ExternalIdQueryParameters(productId.ToString().ToUpper());
        }

        public IQueryParameters GetQueryBalanceParametersByMsisdn(string msisdn)
        {
            return new AccessNumberQueryParameters(msisdn);
        }

        public SubscriberAdjustBalanceRequest GetAdjustBalanceRequest(FlexTopupRequest request, int resourceId)
        {
            var result = new SubscriberAdjustBalanceRequest
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection()
                },
                AdjustType = (int)(request.Amount < 0 ? BalanceAdjustType.Debit : BalanceAdjustType.Credit),
                Amount = (request.Amount < 0)? request.Amount * -1 : request.Amount,
                BalanceResourceId = resourceId,
                EndTime = request.EndTime,
                Reason = request.Reason
            };
            if (request.ProductId.HasValue)
            {
                result.SearchData.SearchCollection.ExternalId = request.ProductId.Value.ToString().ToUpper();
            }
            else
            {
                result.SearchData.SearchCollection.AccessNumber = request.Msisdn;
            }
            return result;
        }

        public SubscriberAdjustBalanceRequest GetAdjustBalanceRequest(AdjustBalanceForSubscriberRequest request)
        {
            var result = new SubscriberAdjustBalanceRequest
            {
                SearchData = new SubscriberSearchData
                {
                    SearchCollection = new SearchCollection {ExternalId = request.ProductId.ToString().ToUpper()}
                },
                AdjustType = request.AdjustType,
                Amount = (request.Amount < 0) ? request.Amount * -1 : request.Amount,
                BalanceResourceId = request.BalanceResourceId,
                EndTime = request.EndTime,
                StartTime = request.StartTime,
                Reason = request.Reason
            };
            return result;
        }

        public GroupAdjustBalanceRequest GetGroupAdjustBalanceRequest(AdjustBalanceForGroupRequest request)
        {
            var result = new GroupAdjustBalanceRequest
            {
                SearchData = new GroupSearchData()
                {
                    SearchCollection = new SearchCollection { ExternalId = request.BusinessUnitId.ToString().ToUpper() }
                },
                AdjustType = request.AdjustType,
                Amount = (request.Amount < 0) ? request.Amount * -1 : request.Amount,
                BalanceResourceId = request.BalanceResourceId,
                EndTime = request.EndTime,
                StartTime = request.StartTime,
                Reason = request.Reason
            };
            return result;
        }
    }
}
