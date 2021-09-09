using Newtonsoft.Json;
using OTAServices.Business.Entities.Common.OasisRequestEnrichment;
using OTAServices.Business.Entities.Helpers;
using OTAServices.Business.Entities.SimManagement;
using OTAServices.Business.Functions.Common;
using OTAServices.Business.Functions.FunctionResults.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using OTAServices.Shared.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeleenaFileLogging.Interfaces;

namespace OTAServices.Business.Functions.Implementations.OTACampaignSubscribers
{
    public class OTACampaignSubscribersEnrichOasisRequest : IOTACampaignSubscribersEnrichOasisRequest
    {
        private const int SubscribersImportBatchSize = 10000;
        private readonly IMaximityDbUnitOfWork _maximityDbUnitOfWork;
        private readonly IProvisioningDbUnitOfWork _provisioningDbUnitOfWork;
        private Dictionary<string, int> _simImsisCount = new Dictionary<string, int>();
        private readonly IJsonLogger _logger;
        private OTASubscribersListProcessingOperationType _otaSubscribersListProcessingOperationType;

        public OTACampaignSubscribersEnrichOasisRequest(IMaximityDbUnitOfWork maximityDbUnitOfWork, IProvisioningDbUnitOfWork provisioningDbUnitOfWork, IJsonLogger logger)
        {
            _maximityDbUnitOfWork = maximityDbUnitOfWork;
            _provisioningDbUnitOfWork = provisioningDbUnitOfWork;
            _logger = logger;
        }

        public async Task<OTACampaignSubscribersEnrichOasisRequestResult> EnrichOasisRequestAsync(OTACampaignSubscribersLeaseImsiResult input)
        {
            _logger.LogEntry(input);

            try
            {
                _otaSubscribersListProcessingOperationType = input.OTASubscribersListProcessingOperationType;

                _simImsisCount = input.SimImsiesCount;
                var batches = input.ValidatedOasisRequests.BatchBy(SubscribersImportBatchSize);

                var iccidOriginalTargetSimProfile = new Dictionary<string, OriginalTargetSimProfilePair>();

                foreach (var oasisRequest in input.ValidatedOasisRequests)
                {
                    iccidOriginalTargetSimProfile.Add(oasisRequest.Iccid, new OriginalTargetSimProfilePair { OriginalSimProfileId = oasisRequest.OriginalSimProfileId, TargetSimProfileId = oasisRequest.TargetSimProfileId });
                }

                foreach (var batch in batches)
                {
                    var iccidList = batch.Where(x => string.IsNullOrEmpty(x.ErrorMessage)).Select(x => x.Iccid).ToList();

                    var productInfosDictionary = GetProductInfoDictionary(iccidList);

                    var provisioningDataInfosDictionary = GetProvisioningDataInfoDictionary(iccidList);

                    var imsiInfosDictionary = GetImsiInfoDictionary(iccidList, batch.First().CampaignId);

                    var targetAndOriginalSimProfileIds = new List<int>();
                    foreach (var oasisRequest in batch)
                    {
                        targetAndOriginalSimProfileIds.Add(oasisRequest.TargetSimProfileId);
                        targetAndOriginalSimProfileIds.Add(oasisRequest.OriginalSimProfileId ?? default(int));
                    }

                    var simProfileSponsors = _provisioningDbUnitOfWork.SimProfileSponsorRepository.GetSimProfileSponsorList(targetAndOriginalSimProfileIds.Distinct().ToList());

                    foreach (var request in batch)
                    {
                        //Check if OasisRequest do not have ErrorMessage, if it has - it is in Failed state already
                        if (!string.IsNullOrEmpty(request.ErrorMessage))
                        {
                            continue;
                        }

                        if (!productInfosDictionary.ContainsKey(request.Iccid))
                        {
                            throw new InvalidOperationException($"Could not find Product Info Data for Uiccid {request.Iccid} and Campaign Id {batch.First().CampaignId}");
                        }

                        var product = productInfosDictionary[request.Iccid];

                        if (!provisioningDataInfosDictionary.ContainsKey(request.Iccid))
                        {
                            throw new InvalidOperationException($"Could not find Provisioning Info Data for Uiccid {request.Iccid}");
                        }

                        var provisioningDataInfo = provisioningDataInfosDictionary[request.Iccid];

                        if (string.IsNullOrEmpty(provisioningDataInfo.KIC) || string.IsNullOrEmpty(provisioningDataInfo.KID) || string.IsNullOrEmpty(provisioningDataInfo.KIK))
                        {
                            throw new InvalidOperationException($"Could not find KIC, KID and KIK for Uiccid {request.Iccid}");
                        }

                        if (!imsiInfosDictionary.ContainsKey(request.Iccid))
                        {
                            throw new InvalidOperationException($"Could not find Imsi Info Data for Uiccid {request.Iccid} and Campaign Id {batch.First().CampaignId}");
                        }

                        var imsiSponsorInfoList = GetImsiSponsorInfosForUiccid(request.Iccid, simProfileSponsors, imsiInfosDictionary, iccidOriginalTargetSimProfile);

                        request.ProductInfo = JsonConvert.SerializeObject(product);
                        request.ProvisioningDataInfo = JsonConvert.SerializeObject(provisioningDataInfo);

                        if (imsiSponsorInfoList.Count > 0)
                        {
                            request.ImsiInfo = JsonConvert.SerializeObject(imsiSponsorInfoList);
                        }

                        request.Status = OasisRequestState.InitialRequestReceived.ToString();
                    }
                }

                var result = new OTACampaignSubscribersEnrichOasisRequestResult(input.FileName, input.ValidatedOasisRequests, input.SubscriberListId, input.OTASubscribersListProcessingOperationType);

                _logger.LogExit(result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Failed to enrich {input.FileName}.");
                throw;
            }
        }

        private Dictionary<string, ProductInfo> GetProductInfoDictionary(List<string> uiccidBatch)
        {
            var products = _maximityDbUnitOfWork.ProductInfoRepository.GetProductInfos(uiccidBatch);

            var productInfoDictionary = new Dictionary<string, ProductInfo>();

            foreach (var product in products)
            {
                if (productInfoDictionary.ContainsKey(product.Uiccid))
                {
                    productInfoDictionary[product.Uiccid] = product;
                }
                else
                {
                    productInfoDictionary.Add(product.Uiccid, product);
                }
            }

            return productInfoDictionary;
        }
        
        private Dictionary<string, ProvisioningDataInfo> GetProvisioningDataInfoDictionary(List<string> iccidList)
        {
            var provisioningDataInfos = _provisioningDbUnitOfWork.ProvisioningDataInfoRepository.GetProvisioningDataInfos(iccidList);

            var provisioningDataInfoDictionary = new Dictionary<string, ProvisioningDataInfo>();

            foreach (var provisioningDataInfo in provisioningDataInfos)
            {
                if (provisioningDataInfoDictionary.ContainsKey(provisioningDataInfo.Uiccid))
                {
                    provisioningDataInfoDictionary[provisioningDataInfo.Uiccid] = provisioningDataInfo;
                }
                else
                {
                    provisioningDataInfoDictionary.Add(provisioningDataInfo.Uiccid, provisioningDataInfo);
                }
            }

            return provisioningDataInfoDictionary;
        }

        private Dictionary<string, List<ImsiInfo>> GetImsiInfoDictionary(List<string> uiccidBatch, int campaignId)
        {
            var imsiInfos = _provisioningDbUnitOfWork.ImsiInfoRepository.GetImsiInfos(uiccidBatch, campaignId);

            var imsiInfoDictionary = new Dictionary<string, List<ImsiInfo>>();

            foreach (var imsiInfo in imsiInfos)
            {
                if (string.IsNullOrEmpty(imsiInfo.ImsiSponsorExternalId) || string.IsNullOrEmpty(imsiInfo.ImsiPrefix))
                {
                    throw new InvalidOperationException($"Invalid data. Imsi {imsiInfo.Imsi} related to Sim with Uiccid {imsiInfo.Iccid} does not have ImsiSponsor with same prefix.");
                }

                if (imsiInfoDictionary.ContainsKey(imsiInfo.Iccid))
                {
                    imsiInfoDictionary[imsiInfo.Iccid].Add(imsiInfo);
                }
                else
                {
                    imsiInfoDictionary.Add(imsiInfo.Iccid, new List<ImsiInfo> { imsiInfo });
                }
            }

            return imsiInfoDictionary;
        }

        private List<ImsiSponsorInfo> GetImsiSponsorInfosForUiccid(string uiccid, List<SimProfileSponsor> simProfileSponsors, Dictionary<string, List<ImsiInfo>> imsiInfosDictionary, Dictionary<string, OriginalTargetSimProfilePair> iccidOriginalTargetDictionary)
        { 
            var imsiSponsorInfos = new List<ImsiSponsorInfo>();

            switch (_otaSubscribersListProcessingOperationType)
            {
                case OTASubscribersListProcessingOperationType.AddImsies:
                    {
                        var lastIndex = _simImsisCount[uiccid];

                        foreach (var imsiInfo in imsiInfosDictionary[uiccid])
                        {
                            var originalSimProfileSponsor = simProfileSponsors.FirstOrDefault(x => x.SponsorPrefix == imsiInfo.ImsiPrefix && x.SimProfileId == iccidOriginalTargetDictionary[uiccid].OriginalSimProfileId);

                            //Imsi is leased already in Provisioning DB, check IsFromOngoingCampaign, and it should not be on original Sim Profile
                            if (imsiInfo.IsFromOngoingCampaign && originalSimProfileSponsor == null)
                            {
                                var mccList = simProfileSponsors.FirstOrDefault(x => x.SponsorPrefix == imsiInfo.ImsiPrefix).MCC;

                                imsiSponsorInfos.Add(new ImsiSponsorInfo
                                {
                                    Iccid = uiccid,
                                    Imsi = imsiInfo.Imsi.ToString(),
                                    MccList = mccList,
                                    SponsorName = imsiInfo.ImsiSponsorExternalId,
                                    ImsiIndex = (++lastIndex).ToString(),
                                    ImsiOperation = ImsiOperation.ADD
                                });
                            }
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.DeleteImsies:
                    {
                        foreach (var imsiInfo in imsiInfosDictionary[uiccid])
                        {
                            var targetSimProfileSponsor = simProfileSponsors.FirstOrDefault(x => x.SponsorPrefix == imsiInfo.ImsiPrefix && x.SimProfileId == iccidOriginalTargetDictionary[uiccid].TargetSimProfileId);

                            //Imsi will be removed if target Sim Profile does not reference Imsi Sponsor, and his Prefix
                            if (targetSimProfileSponsor == null)
                            {
                                imsiSponsorInfos.Add(new ImsiSponsorInfo
                                {
                                    Iccid = uiccid,
                                    Imsi = imsiInfo.Imsi.ToString(),
                                    SponsorName = imsiInfo.ImsiSponsorExternalId,
                                    ImsiOperation = ImsiOperation.DELETE
                                });
                            } 
                        }
                        break;
                    }
                case OTASubscribersListProcessingOperationType.UpdatePlmnLists:
                    {
                        foreach (var imsiInfo in imsiInfosDictionary[uiccid])
                        {
                            var targetSimProfileSponsor = simProfileSponsors.FirstOrDefault(x => x.SponsorPrefix == imsiInfo.ImsiPrefix && x.SimProfileId == iccidOriginalTargetDictionary[uiccid].TargetSimProfileId);
                            var originalSimProfileSponsor = simProfileSponsors.FirstOrDefault(x => x.SponsorPrefix == imsiInfo.ImsiPrefix && x.SimProfileId == iccidOriginalTargetDictionary[uiccid].OriginalSimProfileId);

                            //Imsi will need update of PLMN list if it`s Sponsor exist in both Sim Profile definitions, but they define different MCC lists for same Sponsors
                            if (targetSimProfileSponsor != null && originalSimProfileSponsor != null && targetSimProfileSponsor.MCC != originalSimProfileSponsor.MCC)
                            {
                                var targetMccList = Enumerable.Range(0, targetSimProfileSponsor.MCC.Length / 3).Select(i => targetSimProfileSponsor.MCC.Substring(i * 3, 3)).ToList();
                                var originalMccList = Enumerable.Range(0, originalSimProfileSponsor.MCC.Length / 3).Select(i => originalSimProfileSponsor.MCC.Substring(i * 3, 3)).ToList();
                                var result = targetMccList.Except(originalMccList).ToList();
                                var mccList = string.Join(string.Empty, result.ToArray());

                                imsiSponsorInfos.Add(new ImsiSponsorInfo
                                {
                                    Iccid = uiccid,
                                    Imsi = imsiInfo.Imsi.ToString(),
                                    MccList = mccList,
                                    SponsorName = imsiInfo.ImsiSponsorExternalId,
                                    ImsiOperation = ImsiOperation.UPDATE
                                });
                            }
                            
                        }
                        break;
                    }
            }

            return imsiSponsorInfos;
        }
    }
}