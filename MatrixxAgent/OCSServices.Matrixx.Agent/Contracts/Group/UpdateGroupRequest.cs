namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class UpdateGroupRequest
    {
        public string ExternalId { get; set; }

        public string NewExternalId { get; set; }

        public string Name { get; set; }

        public string TierName { get; set; }
    }
}
