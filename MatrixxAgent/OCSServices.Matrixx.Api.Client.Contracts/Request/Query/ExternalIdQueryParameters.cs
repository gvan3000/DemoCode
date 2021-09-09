using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/subscriber/query/ExternalId/#ExternalId#?querySize=2")]
    public class ExternalIdQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "ExternalId")]
        public string ExternalId { get; private set; }

        public ExternalIdQueryParameters(string externalId)
        {
            ExternalId = externalId;
        }
    }
    [ApiMethodInfo(UrlTemplate = "/group/query/ExternalId/#ExternalId#?querySize=2")]
    public class GroupIdQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "ExternalId")]
        public string ExternalId { get; private set; }

        public GroupIdQueryParameters(string externalId)
        {
            ExternalId = externalId;
        }
    }
}