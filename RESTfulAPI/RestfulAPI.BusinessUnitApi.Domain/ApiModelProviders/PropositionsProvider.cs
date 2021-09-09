using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.TeleenaServiceReferences;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <summary>
    /// Proposition Provider
    /// </summary>
    public class PropositionsProvider : IPropositionsProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _businessUnitApiTranslators;

        /// <summary>
        /// Initialize Proposition Provider
        /// </summary>
        /// <param name="serviceUnitOfWork">Teleena wcf service unit of work</param>
        /// <param name="businessUnitApiTranslators">translator for the BusinessUnit Api</param>
        public PropositionsProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators businessUnitApiTranslators)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _businessUnitApiTranslators = businessUnitApiTranslators;
        }

        /// <summary>
        /// Get a list of all available propositions for the business unit
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public async Task<PropositionsResponseModel> GetPropositionsAsync(Guid businessUnitId)
        {
            var availablePropositions = await _serviceUnitOfWork.PropositionService.GetActivePropositionsByBusinessUnitAsync(businessUnitId);

            var productCreationPropositions = await _serviceUnitOfWork.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(businessUnitId);

            var translatedResult = _businessUnitApiTranslators.PropositionsContractTranslator.Translate(availablePropositions, productCreationPropositions);

            return translatedResult;
        }
    }
}
