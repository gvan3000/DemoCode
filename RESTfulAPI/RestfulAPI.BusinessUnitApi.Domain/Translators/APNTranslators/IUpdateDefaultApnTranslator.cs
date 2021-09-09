using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    public interface IUpdateDefaultApnTranslator
    {
        SetApnDetailsForAccountRequestContract Translate(UpdateBusinessUnitDefaultApnModel input, ApnDetailContract[] apnsOfBusinessUnit);
    }
}
