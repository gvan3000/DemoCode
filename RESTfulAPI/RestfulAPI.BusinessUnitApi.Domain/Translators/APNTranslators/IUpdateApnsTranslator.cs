using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    /// <summary>
    /// Converts between rest mdoel and wcf servicecontract in order to update apns for business unit
    /// </summary>
    public interface IUpdateApnsTranslator
    {
        /// <summary>
        /// Performs the translation
        /// </summary>
        /// <param name="input">rest contract being converted</param>
        /// <param name="availableApns">list of all availalble apns for company</param>
        /// <returns>wcf service contract used to call wcf service</returns>
        SetApnDetailsForAccountRequestContract Translate(UpdateBusinessUnitApnsModel input, ApnSetWithDetailsContract[] availableApns);
    }
}
