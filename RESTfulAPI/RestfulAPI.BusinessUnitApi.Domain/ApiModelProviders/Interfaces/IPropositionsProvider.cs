using RestfulAPI.BusinessUnitApi.Domain.Models.PropositionsModels;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Business Unit Propositions provider
    /// </summary>
    public interface IPropositionsProvider
    {
        /// <summary>
        /// Gets propositions for requested business unit
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        Task<PropositionsResponseModel> GetPropositionsAsync(Guid businessUnitId);
    }
}
