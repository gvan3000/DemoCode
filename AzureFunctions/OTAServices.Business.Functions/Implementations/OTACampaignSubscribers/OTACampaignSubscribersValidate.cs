using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Entities.OTACampaignSubscribers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using OTAServices.Shared.Contracts.Enums;
using SimProfileServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersValidate : IOTACampaignSubscribersValidate
    {
        private const int SubscribersValidationBatchSize = 10000;
        private static readonly Guid SimStatusActive = new Guid("48489470-3FEE-452D-A919-20EF6DE1A261");
        private static readonly Guid ProductStatusActive = new Guid("F8679E2B-2D23-4D5A-BEFD-6385E4CB4014");
        private const string SimTypeOasis = "OASIS_TATA";

        private readonly SimProfileService _simProfileService;
        private readonly IOtaDbUnitOfWork _otaDbUnitOfWork;
        private readonly IMaximityDbUnitOfWork _simInfoUnitOfWork;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;
        private readonly IJsonLogger _logger;
        private readonly List<OasisRequest> _validatedOasisRequests;
        private readonly Dictionary<int, List<string>> _simProfilePrefixes;
        private int _campaignId;
        private OTASubscribersListProcessingOperationType _otaSubscribersListProcessingOperationType;
        private bool _isOtaSubscribersListProcessingOperationTypeSet;

        public OTACampaignSubscribersValidate(
            SimProfileService simProfileService,
            IProvisioningDbUnitOfWork provisioningUnitOfWork,
            IMaximityDbUnitOfWork simInfoUnitOfWork,
            IOtaDbUnitOfWork otaCampaignSubscribersUnitOfWork,
            IJsonLogger logger)
        {
            _simProfileService = simProfileService;
            _provisioningDbUnitOfWork = provisioningUnitOfWork;
            _simInfoUnitOfWork = simInfoUnitOfWork;
            _otaDbUnitOfWork = otaCampaignSubscribersUnitOfWork;
            _logger = logger;
            _validatedOasisRequests = new List<OasisRequest>();
            _simProfilePrefixes = new Dictionary<int, List<string>>();
        }

        public async Task<OTACampaignSubscribersValidateResult> ValidateAsync(OTACampaignSubscribersParseDataResult input)
        {
            _logger.LogEntry(input);

            try
            {
                _otaDbUnitOfWork.BeginTransaction();

                ValidateCampaign(input);
                
                await ValidateTargetSimProfilesAsync(input);

                ValidateIccids(input);

                if (_validatedOasisRequests.Count(x => string.IsNullOrEmpty(x.ErrorMessage)) == 0)
                {
                    throw new InvalidOperationException($"Validation failed, there is not valid Uiccid`s in list. First error message is: {_validatedOasisRequests.First().ErrorMessage}.");
                }

                await _otaDbUnitOfWork.CommitTransactionAsync();

                var result = new OTACampaignSubscribersValidateResult(input.FileName, _validatedOasisRequests, _otaSubscribersListProcessingOperationType);

                _logger.LogExit(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to validate {input.FileName}.");
                throw;
            }
        }

        private void ValidateCampaign(OTACampaignSubscribersParseDataResult input)
        {
            var campaignIds = input.ParsedOasisRequests.Select(x => x.CampaignId).Distinct().ToArray();

            _campaignId = campaignIds.First();

            foreach (var campaignId in campaignIds)
            {
                if (campaignId != _campaignId)
                {
                    throw new InvalidOperationException("Provided list has multiple Campaign Id's provided. All ICCIDs should belong to same Campaign.");
                }

                var campaign = _otaDbUnitOfWork.OTACampaignRepository.GetCampaign(campaignId);

                if (campaign == null)
                {
                    throw new InvalidOperationException($"Provided OTA Campaign Id {campaignId} does not exist in OTA DB.");
                }

                if (campaign.EndDate < DateTime.Now)
                {
                    throw new InvalidOperationException($"Provided OTA Campaign Id {campaignId} has expired.");
                }

                if (campaign.OriginalSimProfile == 0)
                {
                    if (!input.ParsedOasisRequests.First().OriginalSimProfileId.HasValue)
                    {
                        throw new InvalidOperationException($"Provided list has ICCID {input.ParsedOasisRequests.First().Iccid} without Original Sim Profile.");
                    }

                    campaign.OriginalSimProfile = input.ParsedOasisRequests.First().OriginalSimProfileId.Value;
                }

                foreach (var oasisRequest in input.ParsedOasisRequests)
                {
                    if (!oasisRequest.OriginalSimProfileId.HasValue || oasisRequest.OriginalSimProfileId != campaign.OriginalSimProfile)
                    {
                        throw new InvalidOperationException($"Provided list has ICCID {oasisRequest.Iccid} without proper Original Sim Profile. Campaign defined that Original Sim Profile is {campaign.OriginalSimProfile}");
                    }

                    if (oasisRequest.TargetSimProfileId != campaign.TargetSimProfile)
                    {
                        throw new InvalidOperationException($"Provided list has ICCID {oasisRequest.Iccid} without proper Target Sim Profile. Campaign defined that Target Sim Profile is {campaign.TargetSimProfile}");
                    }
                }
            }
        }

        private async Task ValidateTargetSimProfilesAsync(OTACampaignSubscribersParseDataResult input)
        {
            var validationRequest = new ValidateSimProfilesContract
            {
                SimProfileIds = input.ParsedOasisRequests.Select(x => x.TargetSimProfileId).Distinct().ToArray()
            };


            var res = await _simProfileService.ValidateSimProfilesAsync(validationRequest);

            if (!res.AreSimProfilesValid)
            {
                var errors = Environment.NewLine;

                for (var i = 0; i < res.InvalidSimProfileIds.Length; i++)
                {
                    errors += $"Validation of Sim Profile failed with message {res.ValidationErrorMessages[i]}" + Environment.NewLine;
                }

                throw new InvalidOperationException(string.Concat($"Referenced Sim Profiles are not valid, error messages are:", errors));
            }

            CollectSimProfilesSponsorsInfo(validationRequest.SimProfileIds.ToList());
        }

        private void CollectSimProfilesSponsorsInfo(List<int> simProfileIds)
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
        }

        private void ValidateIccids(OTACampaignSubscribersParseDataResult input)
        {
            var uiccids = input.ParsedOasisRequests.Select(x => x.Iccid).Distinct().ToList();

            var targetAndOriginalSimProfileIds = new List<int>();
            foreach (var oasisRequest in input.ParsedOasisRequests)
            {
                targetAndOriginalSimProfileIds.Add(oasisRequest.TargetSimProfileId);
                targetAndOriginalSimProfileIds.Add(oasisRequest.OriginalSimProfileId ?? default(int));
            }

            var simProfileSponsors = _provisioningDbUnitOfWork
                .SimProfileSponsorRepository.GetSimProfileSponsorList(targetAndOriginalSimProfileIds.Distinct().ToList());

            var parsedOasisRequestsDictionary = PrepareParsedOasisRequestsDictionary(input);

            var batches = uiccids.BatchBy(SubscribersValidationBatchSize);

            foreach (var batch in batches)
            {
                var simInfoDictionary = PrepareSimInfoDictionary(batch);

                var simContentDictionary = PrepareSimContentDictionary(batch);

                foreach (var iccid in batch)
                {
                    //Add ValidatedOasis requests
                    _validatedOasisRequests.Add(parsedOasisRequestsDictionary[iccid]);

                    if (!ValidateSimAndProduct(iccid, simInfoDictionary, parsedOasisRequestsDictionary))
                    {
                        continue;
                    }

                    if (!ValidateSimProfileChange(iccid, parsedOasisRequestsDictionary, simContentDictionary))
                    {
                        continue;
                    }

                    if (!ValidateIfChangesExistsWithTargetSimProfile(
                        iccid, parsedOasisRequestsDictionary, simContentDictionary, simProfileSponsors))
                    {
                        continue;
                    }
                }
            }
        }

        private bool ValidateSimAndProduct(string iccid, Dictionary<string, SimInfo> simInfoDictionary, Dictionary<string, OasisRequest> parsedOasisRequestsDictionary)
        {
            //Validate against data from MaximityDB
            if (!simInfoDictionary.ContainsKey(iccid))
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Iccid not found in MaximityDb.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            var simInfo = simInfoDictionary[iccid];

            if (simInfo.SimType != SimTypeOasis)
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Sim does not have proper SimTypeCode (OASIS_TATA).";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            if (simInfo.SimStatus != SimStatusActive)
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Sim is not in Active status.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            if (simInfo.ProductStatus == null)
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Sim does not have related Product.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            if (simInfo.ProductStatus != ProductStatusActive)
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Product relate to Sim is not in Active status.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            return true;
        }

        private bool ValidateSimProfileChange(string iccid, Dictionary<string, OasisRequest> parsedOasisRequestsDictionary, Dictionary<string, List<SimContent>> simContentDictionary)
        {
            // Validate against data from ProvisioningDb
            if (!simContentDictionary.ContainsKey(iccid))
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Iccid not found in ProvisioningDb.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            if (simContentDictionary[iccid].Any(s => string.IsNullOrWhiteSpace(s.ImsiSponsorPrefix)))
            {
                var message = $"Validation for Uiccid {iccid} failed with message: Imsi related to Sim does not have ImsiSponsor with same prefix.";
                parsedOasisRequestsDictionary[iccid].Status = OasisRequestState.Failed.ToString();
                parsedOasisRequestsDictionary[iccid].ErrorMessage = message;
                return false;
            }

            if (!_simProfilePrefixes.ContainsKey(parsedOasisRequestsDictionary[iccid].TargetSimProfileId))
            {
                throw new InvalidOperationException($"Related ImsiSponsors are missing for SimProfile with ID {parsedOasisRequestsDictionary[iccid].TargetSimProfileId} in Provisioning DB.");
            }

            DetermineOTASubscribersListProcessingOperationType(iccid, parsedOasisRequestsDictionary, simContentDictionary);

            switch (_otaSubscribersListProcessingOperationType)
            {
                case OTASubscribersListProcessingOperationType.AddImsies:
                    {
                        foreach (var simContent in simContentDictionary[iccid])
                        {
                            if (!_simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId].Contains(simContent.ImsiSponsorPrefix))
                            {
                                throw new InvalidOperationException($"Add Imsis use-case is detected, and subscriber from list with uiccid {iccid} has Sim with Imsi from sponsor with prefix {simContent.ImsiSponsorPrefix}, which is not defined by target Sim Profile.");
                            }
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.DeleteImsies:
                    {
                        foreach (var sponsorPrefix in _simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId])
                        {
                            if (!simContentDictionary[iccid].Any(x => x.ImsiSponsorPrefix == sponsorPrefix))
                            {
                                throw new InvalidOperationException($"Delete Imsis use-case detected, and subscriber from list with uiccid {iccid} has Sim with missing Imsi for sponsor with prefix {sponsorPrefix}, which is defined by target Sim Profile.");
                            }
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.UpdatePlmnLists:
                    {
                        foreach (var simContent in simContentDictionary[iccid])
                        {
                            if (!_simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId].Contains(simContent.ImsiSponsorPrefix))
                            {
                                throw new InvalidOperationException($"Update PLMN list use-case detected, and subscriber from list with uiccid {iccid} has Sim with Imsi from sponsor with prefix {simContent.ImsiSponsorPrefix}, which is not defined by target Sim Profile.");
                            }
                        }
                        foreach (var sponsorPrefix in _simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId])
                        {
                            if (!simContentDictionary[iccid].Any(x => x.ImsiSponsorPrefix == sponsorPrefix))
                            {
                                throw new InvalidOperationException($"Update PLMN list use-case detected, and subscriber from list with uiccid {iccid} has Sim with missing Imsi for sponsor with prefix {sponsorPrefix}, which is defined by target Sim Profile.");
                            }
                        }
                        break;
                    }
            }

            return true;
        }

        private void DetermineOTASubscribersListProcessingOperationType(string iccid, Dictionary<string, OasisRequest> parsedOasisRequestsDictionary, Dictionary<string, List<SimContent>> simContentDictionary)
        {
            if (!_isOtaSubscribersListProcessingOperationTypeSet)
            {
                //Determine do we add new Imsi to Sim
                foreach (var sponsorPrefix in _simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId])
                {
                    if (!simContentDictionary[iccid].Any(x => x.ImsiSponsorPrefix == sponsorPrefix))
                    {
                        _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.AddImsies;
                        _isOtaSubscribersListProcessingOperationTypeSet = true;
                        _logger.LogMessage($"Add Imsis use-case detected, based on first ICCID in list ({iccid}).");
                        return;
                    }
                }

                //Determine do we remove Imsi from Sim
                foreach (var simContent in simContentDictionary[iccid])
                {
                    if (!_simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId].Any(x => x == simContent.ImsiSponsorPrefix))
                    {
                        _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.DeleteImsies;
                        _isOtaSubscribersListProcessingOperationTypeSet = true;
                        _logger.LogMessage($"Delete Imsis use-case detected, based on first ICCID in list ({iccid}).");
                        return;
                    }
                }

                //If we are not adding or removing - this is update of PLMN list use-case
                _otaSubscribersListProcessingOperationType = OTASubscribersListProcessingOperationType.UpdatePlmnLists;
                _isOtaSubscribersListProcessingOperationTypeSet = true;
                _logger.LogMessage($"Update PLMN list use-case detected, based on first ICCID in list ({iccid}).");
                return;
            }
        }

        private bool ValidateIfChangesExistsWithTargetSimProfile(string iccid,
            Dictionary<string, OasisRequest> parsedOasisRequestsDictionary,
            Dictionary<string, List<SimContent>> simContentDictionary, 
            List<SimProfileSponsor> simProfileSponsors
        )
        {
            switch (_otaSubscribersListProcessingOperationType)
            {
                case OTASubscribersListProcessingOperationType.AddImsies:
                    {
                        // is any imsi leased for ongoing campain
                        if (simContentDictionary[iccid].Any(x => x.IsLeasedForOngoingCampaign))
                        {
                            return true;
                        }

                        // added imsi
                        foreach (var sponsorPrefix in _simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId])
                        {
                            if (!simContentDictionary[iccid].Any(x => x.ImsiSponsorPrefix == sponsorPrefix)) // add imsi operation
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.DeleteImsies:
                    {
                        // removed imsi
                        foreach (var simContent in simContentDictionary[iccid])
                        {
                            if (!_simProfilePrefixes[parsedOasisRequestsDictionary[iccid].TargetSimProfileId].Any(x => x == simContent.ImsiSponsorPrefix)) // remove imsi operation
                            {
                                return true;
                            }
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.UpdatePlmnLists:
                    {
                        var oasisRequest = parsedOasisRequestsDictionary[iccid];

                        if (oasisRequest.OriginalSimProfileId.HasValue)
                        {
                            // is plmn list updated
                            foreach (var simContent in simContentDictionary[iccid])
                            {
                                // none sim content here IsLeasedForOngoingCampaign

                                var original = simProfileSponsors
                                    .FirstOrDefault(x => x.SimProfileId == oasisRequest.OriginalSimProfileId
                                             && x.SponsorPrefix == simContent.ImsiSponsorPrefix);

                                if (original == null)
                                {
                                    throw new InvalidOperationException(
                                        $"Failed to {nameof(ValidateIfChangesExistsWithTargetSimProfile)}. " +
                                        $"Cannot find original sponsor with prefix {simContent.ImsiSponsorPrefix} and " +
                                        $"sim profile id {oasisRequest.OriginalSimProfileId}"
                                        );
                                }

                                var target = simProfileSponsors
                                    .FirstOrDefault(x => x.SimProfileId == oasisRequest.TargetSimProfileId
                                             && x.SponsorPrefix == simContent.ImsiSponsorPrefix);

                                if (target == null)
                                {
                                    throw new InvalidOperationException(
                                        $"Failed to {nameof(ValidateIfChangesExistsWithTargetSimProfile)}. " +
                                        $"Cannot find target sponsor with sponsor prefix {simContent.ImsiSponsorPrefix} and " +
                                        $"sim profile id {oasisRequest.OriginalSimProfileId}"
                                    );
                                }

                                if (original.MCC != target.MCC)
                                {
                                    return true;
                                }
                            }
                        }
                        break;
                    }
            }

            // no changes, validation error

            var request = parsedOasisRequestsDictionary[iccid];
            request.Status = OasisRequestState.Failed.ToString();
            request.ErrorMessage =
            $"Validation for Uiccid {iccid} failed with message: Sim card {iccid} already adhere to target Sim Profile.";

            return false;
        }

        private Dictionary<string, List<SimContent>> PrepareSimContentDictionary(List<string> batch)
        {
            var simContentList = _provisioningDbUnitOfWork.SimContentRepository.GetSimContentBatch(batch, _campaignId);

            var simContentDictionary = new Dictionary<string, List<SimContent>>();

            foreach (var simContent in simContentList)
            {
                //Collect info about Imsies related to Sim in provisioning Db for future creation of Lease Reqeusts
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

        private Dictionary<string, SimInfo> PrepareSimInfoDictionary(List<string> batch)
        {
            var simInfoList = _simInfoUnitOfWork.SimInfoRepository.GetSimInfoBatch(batch);

            var simInfoDictionary = new Dictionary<string, SimInfo>();

            foreach (var simInfo in simInfoList)
            {
                if (simInfoDictionary.ContainsKey(simInfo.Uiccid))
                {
                    simInfoDictionary[simInfo.Uiccid] = simInfo;
                }
                else
                {
                    simInfoDictionary.Add(simInfo.Uiccid, simInfo);
                }
            }

            return simInfoDictionary;
        }

        private static Dictionary<string, OasisRequest> PrepareParsedOasisRequestsDictionary(OTACampaignSubscribersParseDataResult input)
        {
            var parsedOasisRequestsDictionary = new Dictionary<string, OasisRequest>();

            foreach (var oasisRequest in input.ParsedOasisRequests)
            {
                if (parsedOasisRequestsDictionary.ContainsKey(oasisRequest.Iccid))
                {
                    parsedOasisRequestsDictionary[oasisRequest.Iccid] = oasisRequest;
                }
                else
                {
                    parsedOasisRequestsDictionary.Add(oasisRequest.Iccid, oasisRequest);
                }
            }

            return parsedOasisRequestsDictionary;
        }
    }
}
