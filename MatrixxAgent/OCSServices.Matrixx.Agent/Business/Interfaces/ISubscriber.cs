using OCSServices.Matrixx.Agent.Contracts.Group;
using OCSServices.Matrixx.Agent.Contracts.Imsi;
using OCSServices.Matrixx.Agent.Contracts.Msisdn;
using OCSServices.Matrixx.Agent.Contracts.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface ISubscriber
    {
        ExternalIdQueryParameters GetSubscriberRequest(string identifier);
        MultiRequest BuildCreateSubscriberRequest(CreateSubscriberRequest request);
        MultiRequest BuildSetCustomSubscriberConfigurationRequest(SetCustomConfigurationRequest request);
        MultiRequest BuildSetStatusRequest(SetSubscriberStatusRequest request);
        api.CreateSubscriberRequest BuildCreateGroupAdmin(CreateGroupAdminRequest request);
        MultiRequest CreateDetachImsiFromSubscriber(DetachImsiFromSubscriberRequest request);
        MultiRequest CreateModifySubscriberFirstNameRequest(SetSubscriberFirstNameRequest request);
        api.ModifySubscriberRequest CreateModifySubscriberContactPhoneNumberRequest(UpdateContactPhoneNumberRequest request);
        MultiRequest CreateDeleteSubscriberRequest(DeactiveSubscriberRequest request);
    }
}
