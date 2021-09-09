using Newtonsoft.Json;
using OTAServices.Business.Entities.Common.SubscriberListLeaseRequestEnrichment;
using OTAServices.Business.Entities.LeaseRequest;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using Provisioning.Bus.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersLeaseImsi : IOTACampaignSubscribersLeaseImsi
    {
        private readonly IJsonLogger _logger;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;
        private readonly IProvisioningServicesBusQueueClient _queueClient;
        private int _campaignId;

        private readonly Dictionary<string, int> _simImsisCount;
        private readonly Dictionary<int, List<string>> _simProfilePrefixes;
        private const string ProvisioningBusSagaStarterType = "Provisioning.Bus.Commands.TriggerLeaseImsiesForOTACampaignSaga";

        public OTACampaignSubscribersLeaseImsi(IProvisioningDbUnitOfWork provisioningDbUnitOfWork, IProvisioningServicesBusQueueClient queueClient, IJsonLogger logger)
        {
            _logger = logger;
            _provisioningDbUnitOfWork = provisioningDbUnitOfWork;
            _queueClient = queueClient;
            _simProfilePrefixes = new Dictionary<int, List<string>>();
            _simImsisCount = new Dictionary<string, int>();
        }

        public async Task<OTACampaignSubscribersLeaseImsiResult> LeaseImsisAsync(OTACampaignSubscribersValidateResult input)
        {
            _logger.LogEntry(input);
            try
            {
                var subscriberListId = Guid.NewGuid();

                if (input.ValidatedOasisRequests == null || !input.ValidatedOasisRequests.Any())
                {
                    throw new InvalidOperationException("List of validated Oasis requests is empty.");
                }

                var validOasisRequestsDictionary = PrepareValidOasisRequestsDictionary(input.ValidatedOasisRequests);

                _campaignId = input.ValidatedOasisRequests.FirstOrDefault().CampaignId;
               
                var iccids = validOasisRequestsDictionary.Select(x => x.Key).ToList();
                var simProfileIds = validOasisRequestsDictionary.Select(x => x.Value.TargetSimProfileId).ToList();

                var leaseRequests = PrepareLeaseRequest(iccids, simProfileIds, validOasisRequestsDictionary);
                
                input.ValidatedOasisRequests.ForEach(c => { c.SubscriberListId = subscriberListId; });

                _provisioningDbUnitOfWork.LeaseRequestRepository.AddSubscriberListLeaseRequest(CreateLeaseRequest(leaseRequests, subscriberListId));
                await _provisioningDbUnitOfWork.CommitTransactionAsync();

                var sagaTrigger = new TriggerLeaseImsiesForOTACampaignSaga
                {
                    InstanceId = input.InstanceId,
                    SubscribersListId = subscriberListId
                };

                await _queueClient.SendToNserviceBus(JsonConvert.SerializeObject(sagaTrigger), ProvisioningBusSagaStarterType);
                
                _logger.LogMessage("Leasing will be continued on Provisioning Bus. Timeout for this Workflow step is 4h.");
                var result = new OTACampaignSubscribersLeaseImsiResult(input.FileName, input.ValidatedOasisRequests, subscriberListId, _simImsisCount, input.OTASubscribersListProcessingOperationType);

                _logger.LogExit(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to lease imsis {input.FileName}.");
                throw;
            }
        }

        private SubscriberListLeaseRequest CreateLeaseRequest(List<LeaseRequest> leaseRequests, Guid subscriberListId)
        {
            return new SubscriberListLeaseRequest
            {
                CampaignId = _campaignId,
                SubscriberListId = subscriberListId,
                LeaseRequests = JsonConvert.SerializeObject(leaseRequests)
            };
        }

        private List<LeaseRequest> PrepareLeaseRequest(List<string> iccids, List<int> simProfileIds, Dictionary<string, OasisRequest> validOasisRequestsDictionary)
        {
            
            var simProfileSponsors = _provisioningDbUnitOfWork.SimProfileSponsorRepository.GetSimProfileSponsorList(simProfileIds);
            foreach (var simProfileSponsor in simProfileSponsors)
            {
                if (_simProfilePrefixes.ContainsKey(simProfileSponsor.SimProfileId))
                {
                    _simProfilePrefixes[simProfileSponsor.SimProfileId].Add(simProfileSponsor.SponsorPrefix);
                }
                else
                {
                    _simProfilePrefixes.Add(simProfileSponsor.SimProfileId, new List<string> { simProfileSponsor.SponsorPrefix });
                }
            }

            var simContentDictionary = PrepareSimContentDictionaryAndCalculateImsisCount(iccids);
            
            var leaseRequests = new List<LeaseRequest>();

            foreach (var iccid in iccids)
            {
                // Create LeaseRequests for Sponsor Prefixes prescribed by Sim Profile and not found on Sim
                foreach (var sponsorPrefix in _simProfilePrefixes[validOasisRequestsDictionary[iccid].TargetSimProfileId])
                {
                    if (!simContentDictionary[iccid].Any(x => !string.IsNullOrWhiteSpace(x.ImsiSponsorPrefix) && x.ImsiSponsorPrefix == sponsorPrefix)) // new imsi
                    {
                        leaseRequests.Add(new LeaseRequest { Uiccid = iccid, SponsorPrefix = sponsorPrefix });
                    }
                }
            }

            return leaseRequests;
        }

        private Dictionary<string, List<SimContent>> PrepareSimContentDictionaryAndCalculateImsisCount(List<string> batch)
        {
            var simContentList = _provisioningDbUnitOfWork.SimContentRepository.GetSimContentBatch(batch, _campaignId);

            var simContentDictionary = new Dictionary<string, List<SimContent>>();

            foreach (var simContent in simContentList)
            {
                //Collect Count of Imsis that were on Sim before this OTA Campaign
                if (_simImsisCount.ContainsKey(simContent.Uiccid))
                {
                    if (!simContent.IsLeasedForOngoingCampaign)
                    {
                        _simImsisCount[simContent.Uiccid] = _simImsisCount[simContent.Uiccid] + 1;
                    }
                }
                else
                {
                    if (!simContent.IsLeasedForOngoingCampaign)
                    {
                        _simImsisCount.Add(simContent.Uiccid, 1);
                    }
                    else
                    {
                        _simImsisCount.Add(simContent.Uiccid, 0);
                    }
                }

                //Collect info about Imsis related to Sim in provisioning Db for future creation of Lease Requests
                if (simContentDictionary.ContainsKey(simContent.Uiccid))
                {
                    simContentDictionary[simContent.Uiccid].Add(simContent);
                }
                else
                {
                    simContentDictionary.Add(simContent.Uiccid, new List<SimContent> { simContent });
                }
            }

            return simContentDictionary;
        }

        private static Dictionary<string, OasisRequest> PrepareValidOasisRequestsDictionary(List<OasisRequest> validatedOasisRequests)
        {
            var validOasisRequestsDictionary = new Dictionary<string, OasisRequest>();

            foreach (var oasisRequest in validatedOasisRequests)
            {
                if (validOasisRequestsDictionary.ContainsKey(oasisRequest.Iccid))
                {
                    validOasisRequestsDictionary[oasisRequest.Iccid] = oasisRequest;
                }
                else
                {
                    validOasisRequestsDictionary.Add(oasisRequest.Iccid, oasisRequest);
                }
            }

            return validOasisRequestsDictionary;
        }
    }
}