using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class RemoveGroupRequest
    {
        public string ExternalId { get; set; }

        public Endpoint Endpoint { get; set; }

    }
}