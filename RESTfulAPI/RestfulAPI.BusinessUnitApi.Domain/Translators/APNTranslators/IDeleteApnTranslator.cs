using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    /// <summary>
    /// Delete apn translator
    /// </summary>
    public interface IDeleteApnTranslator
    {
        /// <summary>
        /// Translates apns linked to the buisness unit to SetApnDetailsForAccountRequestContract 
        /// Removes apn from apns linked to the business unit 
        /// </summary>
        /// <param name="apns">apns linked to the business unit</param>
        /// <param name="apn">apn to be removed</param>
        /// <param name="businessUnitId">id of the buinsess unit</param>
        /// <returns><see cref="SetApnDetailsForAccountRequestContract"/></returns>
        SetApnDetailsForAccountRequestContract Translate(ApnDetailContract[] apns, Guid apn, Guid businessUnitId);       
    }
}
