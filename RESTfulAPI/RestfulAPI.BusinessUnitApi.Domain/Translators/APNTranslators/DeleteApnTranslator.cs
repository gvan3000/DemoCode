using System;
using System.Linq;
using System.Collections.Generic;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    /// <summary>
    /// Deleet apn translator
    /// </summary>
    public class DeleteApnTranslator : IDeleteApnTranslator
    {
        /// <summary>
        /// Translates apns linked to the buisness unit to SetApnDetailsForAccountRequestContract
        /// Removes apn from apns linked to the business unit 
        /// </summary>
        /// <param name="apns">apns linked to the business unit</param>
        /// <param name="apnSetDetailId">apn to be removed</param>
        /// <param name="businessUnitId">id of the business unit</param>
        /// <returns></returns>
        public SetApnDetailsForAccountRequestContract Translate(ApnDetailContract[] apns, Guid apnSetDetailId, Guid businessUnitId)
        {            
            if (apns == null)
            {
                throw new ArgumentNullException(nameof(apns));
            }

            var result = new SetApnDetailsForAccountRequestContract
            {
                AccountId = businessUnitId
            };

            var apnsUpdated = apns.Where(x => x.ApnSetDetailId != apnSetDetailId).ToList();

            var apnDetailsContract = new List<ApnDetailContract>();
            apnsUpdated.ForEach(x => apnDetailsContract.Add(x));

            result.ApnDetails = apnDetailsContract.ToArray();

            return result;
        }       
    }
}
