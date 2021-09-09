using System;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    public class UpdateDefaultApnTranslator : IUpdateDefaultApnTranslator
    {
        public SetApnDetailsForAccountRequestContract Translate(UpdateBusinessUnitDefaultApnModel input, ApnDetailContract[] apnsOfBusinessUnit)
        {
            if (input == null)
                return null;

            if (apnsOfBusinessUnit == null || !apnsOfBusinessUnit.Any() || (input.Id == Guid.Empty))
                throw new ArgumentNullException(nameof(apnsOfBusinessUnit));


            var result = new SetApnDetailsForAccountRequestContract
            {
                ApnDetails = apnsOfBusinessUnit
                    .Select(apn => new ApnDetailContract()
                    {
                        Name = apn.Name,
                        Id = apn.Id,
                        IsDefault = apn.ApnSetDetailId == input.Id,
                        ApnSetId = apn.ApnSetId
                    })
                    .ToArray()
            };

            return result;
        }
    }
}
