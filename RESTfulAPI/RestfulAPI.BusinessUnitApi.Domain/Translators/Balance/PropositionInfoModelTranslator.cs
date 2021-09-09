using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System.Collections.Generic;
using System.Linq;
using CommercialOfferDefinition = RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance.CommercialOfferDefinition;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators.Balance
{
    /// <summary>
    /// Translate PropositionsContract to list of PropositionInfoModel
    /// </summary>
    public class PropositionInfoModelTranslator : ITranslate<PropositionsContract, List<PropositionInfoModel>>
    {
        /// <summary>
        /// Translates
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<PropositionInfoModel> Translate(PropositionsContract input)
        {
            List<PropositionInfoModel> propositionInfoModels = new List<PropositionInfoModel>();

            if (input?.PropositionContracts == null || input.PropositionContracts.Count() < 1)
            {
                return null;
            }

            propositionInfoModels = input.PropositionContracts.Select(x => new PropositionInfoModel
            {
                PropositionId = x.Id,
                CommercialOfferPropositionCode = x.CommercialOfferPropositionCode,
                CommercialOfferDefinitions = x.CommercialOfferConfigurationsContract?.CommercialOfferConfigurationContracts?.Select(c => new CommercialOfferDefinition
                {
                    ServiceTypeCode = c.ServiceLevelTypeCode,
                    SubscriptionTypeCode = c.SubscriptionTypeCode,
                    CommercialOfferDefinitionCode = c.Code

                }).ToList()
            }).ToList();

            return propositionInfoModels;
        }
    }
}
