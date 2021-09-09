using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators
{
    public class BusinessUnitAPNsTranslator : ITranslate<ApnSetWithDetailsContract[], APNSetList>
    {
        /// <summary>
        /// Translation to list of APN sets
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public APNSetList Translate(ApnSetWithDetailsContract[] input)
        {
            var result = new APNSetList();
            if (input == null)
                return null;
            result.APNSets = input
                .Select(TranslateIndividual)
                .ToList();
            return result;
        }

        private APNSet TranslateIndividual(ApnSetWithDetailsContract contract)
        {
            var apnSet = new APNSet();

            if(contract == null)
            {
                return apnSet;
            }

            apnSet.Id = contract.ApnSet.Id;
            apnSet.Name = contract.ApnSet.Name;

            if (contract.ApnSetDetails.Select(x => x.Name) != null)
            {
                apnSet.APNs = contract.ApnSetDetails.Select(x => new APNResponseDetail() { Name = x.Name, Id = x.ApnSetDetailId }).ToList();
            }
            
            return apnSet;
        }
    }
}
