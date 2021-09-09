using OTAServices.Business.Functions.FunctionResults.OTACampaign;
using OTAServices.Business.Functions.WorkflowStarters.OTACampaign;

namespace OTAServices.Business.Functions.Interfaces.OTACampaign
{
    public interface IOTACampaignFinalizeProcessing
    {
        /// <summary>
        /// Finalization of importing of file, with moving to succeeded folder.
        /// </summary>
        /// <param name="input">Successfully imported file.</param>
        void FinalizeWithSuccess(OTACampaignSaveCampaingResult input);

        /// <summary>
        /// Finalization of importing of file, with moving to error folder.
        /// </summary>
        /// <param name="input">Starting file that failed import.</param>
        void FinalizeWithError(OTACampaignStarter input);
    }
}
