using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Group;
using OCSServices.Matrixx.Api.Client.Internal.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IGroupProxy : IDisposable
    {

        Task<GroupQueryResponse> GroupQuery(IQueryParameters queryParameters, Endpoint endpoint);
        Task<MatrixxResponse> GroupModify(ModifyGroupRequest request);
        Task<CreateObjectResponse> GroupCreate(CreateGroupRequest request);
        Task<AddOfferToSubscriberResponse> PurchaseGroupOffer(PurchaseGroupOfferRequest request, Endpoint endpoint);
        Task<MatrixxResponse> CreateGroupAdmin(CreateSubscriberRequest request, Endpoint endpoint);
        Task<MatrixxResponse> RemoveGroup(DeleteGroupRequest request, Endpoint endpoint);
    }

    public class GroupProxy : BaseProxy, IGroupProxy
    {
        
        public Task<GroupQueryResponse> GroupQuery(IQueryParameters queryParameters, Endpoint endpoint)
        {
            using (var query = new GroupQuery(endpoint))
            {
                return query.Execute(queryParameters);
            }
        }
        public Task<CreateObjectResponse> GroupCreate(CreateGroupRequest request)
        {
            using (var groupCreate = new GroupCreate())
            {
                return groupCreate.Execute(request);
            }
        }

        public Task<MatrixxResponse> GroupModify(ModifyGroupRequest request)
        {
            using (var groupModify = new GroupModify())
            {
                return groupModify.Execute(request);
            }
        }


        public Task<AddOfferToSubscriberResponse> PurchaseGroupOffer(PurchaseGroupOfferRequest request, Endpoint endpoint)
        {
            using (var groupPurchaseOffer = new GroupPurchaseOffer(endpoint))
            {
                return groupPurchaseOffer.Execute(request);
            }
        }



        /// <summary>
        /// Create Group Admin
        /// </summary>
        /// <param name="request"></param>
        /// <param name="endpoint"></param>
        /// <returns>Task<MatrixxResponse></returns>
        public Task<MatrixxResponse> CreateGroupAdmin(CreateSubscriberRequest request, Endpoint endpoint)
        {
            using (var groupAdminCreate = new GroupAdminCreate(endpoint))
            {
                return groupAdminCreate.Execute(request);
            }
        }

        /// <summary>
        /// Remove Group
        /// </summary>
        /// <param name="request"></param>
        /// <param name="endpoint"></param>
        /// <returns>Task<MatrixxResponse></returns>
        public Task<MatrixxResponse> RemoveGroup(DeleteGroupRequest request, Endpoint endpoint)
        {
            using (var groupRemove = new GroupRemove(endpoint))
            {
                return groupRemove.Execute(request);
            }
        }
    }
}
