using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IGroup
    {
        GroupIdQueryParameters GetGroupRequest(string identifier);
        MultiRequest BuildCreateGroupRequest(AddGroupRequest request);
        ModifyGroupRequest BuildModifyGroupRequest(UpdateGroupRequest request);
        PurchaseGroupOfferRequest BuidPurchaseGroupOfferRequest(AddOfferToGroupRequest request);
        DeleteGroupRequest CreateDeleteGroupRequest(RemoveGroupRequest request);

    }
}
