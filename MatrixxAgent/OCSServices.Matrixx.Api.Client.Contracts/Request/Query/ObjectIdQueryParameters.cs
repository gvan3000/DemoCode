using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/subscriber/query/ObjectId/#ObjectId#?querySize=2")]
    public class ObjectIdQueryParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "ObjectId")]
        public string ObjectId { get; private set; }

        public ObjectIdQueryParameters(string objectId)
        {
            ObjectId = objectId;
        }
    }
}