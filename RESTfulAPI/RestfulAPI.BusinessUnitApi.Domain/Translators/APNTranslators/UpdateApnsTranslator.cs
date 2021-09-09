using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    public class UpdateApnsTranslator : IUpdateApnsTranslator
    {
        public SetApnDetailsForAccountRequestContract Translate(UpdateBusinessUnitApnsModel input, ApnSetWithDetailsContract[] availableApns)
        {
            if (input == null)
                return null;

            if ((availableApns == null || !availableApns.Any()) && (input.Apns != null || input.Apns.Count > 0))
                throw new ArgumentNullException(nameof(availableApns));

            var result = new SetApnDetailsForAccountRequestContract();

            var flatList = availableApns.SelectMany(x => x.ApnSetDetails);

            if (input.Apns != null)
            {
                result.ApnDetails = input.Apns
                        .Select(apn => TranslateSingle(apn, flatList, input.DefaultApn))
                        .ToArray();
            }

            return result;
        }

        private ApnDetailContract TranslateSingle(APNRequestDetail input, IEnumerable<ApnDetailContract> available, Guid? defaultApn)
        {
            var matched = available.First(x => x.ApnSetDetailId == input.Id);

            return new ApnDetailContract()
            {
                Id = matched.Id,
                IsDefault = defaultApn == input.Id,
                ApnSetId = matched.ApnSetId,
                ApnSetDetailId = matched.ApnSetDetailId
            };
        }
    }
}
