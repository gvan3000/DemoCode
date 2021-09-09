using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.AddressService;
using RestfulAPI.TeleenaServiceReferences.PersonService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class BusinessUnitProvider : LoggingBase, IBusinessUnitProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;
        private readonly IBusinessUnitProducerFactory _producerFactory;
        private readonly ICustomAppConfiguration _appConfiguration;

        public BusinessUnitProvider(IJsonRestApiLogger logger,
            ITeleenaServiceUnitOfWork serviceUnitOfWork,
            IBusinessUnitApiTranslators translators,
            IBusinessUnitProducerFactory producerFactory,
            ICustomAppConfiguration appConfiguration)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
            _producerFactory = producerFactory;
            _appConfiguration = appConfiguration;
        }

        private async Task<LoadAddressData> LoadAddresses(Guid personId)
        {
            // this is needed after business unit is created but is fetched here in case of errors
            var person = await _serviceUnitOfWork.PersonService.GetPersonByIdAsync(personId);
            if (person == null)
            {
                Logger.Log(LogSeverity.Warning, $"Could not find person with id of {personId}, service returned null", nameof(LoadAddresses));
                return new LoadAddressData() { ErrorMessage = "Person does not exist." };
            }
            Logger.Log(LogSeverity.Debug, $"Found person with id of {person.Id}.", nameof(LoadAddresses));

            var addresses = await _serviceUnitOfWork.AddressService.GetAddressesByPersonAsync(new GetAddressByPersonContract() { PersonId = person.Id });
            if (addresses == null || !addresses.Any())
            {
                Logger.Log(LogSeverity.Warning, $"Could not find any addresses for person with id of {person.Id}.", nameof(LoadAddresses));
                return new LoadAddressData() { ErrorMessage = "Person does not have any addresses." };
            }
            Logger.Log(LogSeverity.Debug, $"Found addresses for person, {nameof(addresses.Count)} = {addresses.Count}.", nameof(LoadAddresses));
            return new LoadAddressData() { Addresses = addresses };
        }

        private async Task<TeleenaServiceReferences.AddressService.TeleenaInnerExp> UpdateAddresses(List<Guid> addresseIds, Guid? newBusinessUnitId)
        {
            // MP relies on Address <-> Business Unit relationship to work correctly, we need to update business unit id here to maintain it
            var addressUpdateTasks = addresseIds.Select(addressId => _serviceUnitOfWork.AddressService.UpdateAddressAsync(new UpdateAddressContract() { Id = addressId, AccountId = newBusinessUnitId }));
            try
            {
                var updatedAddresses = await Task.WhenAll(addressUpdateTasks);
                if (updatedAddresses == null || updatedAddresses.Any(address => address == null))
                    throw new InvalidOperationException(MessageConstants.UnexpectedServiceNullResponseMessage);
            }
            catch (FaultException<TeleenaServiceReferences.AddressService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, "Error when updating addresses for person when creating business unit", nameof(UpdateAddresses), ex);
                return ex.Detail;
            }
            return null;
        }

        private async Task MakePersonAvailableToNewBusinessUnit(Guid personId, Guid businessUnitId)
        {
            var serviceRequest = new ChangePersonOwnerRequest() { PersonId = personId, NewOwningAccountId = businessUnitId };
            await _serviceUnitOfWork.PersonService.ChangePersonOwnerAsync(serviceRequest);
        }

        public async Task<ProviderOperationResult<CreateBusinessUnitResponseModel>> CreateAsync(Guid userCompanyId, Guid? userBusinessUnitId, BusinessUnitCreateModel request, Guid requestId)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var serviceRequest = _translators.CreateBusinessUnitTranslator.Translate(request);

            var accountValidationRequest = new AccountValidationContract()
            {
                PersonId = request.PersonId,
                CompanyId = userCompanyId,
                ParentId = request.ParentId ?? userBusinessUnitId,
                BillCycleId = null,
                BillingCycleStartDay = request.BillingCycleStartDay,
                WholesalePriceplan = request.WholesalePriceplan,
                Propositions = serviceRequest.Propositions
            };

            var validation = await _serviceUnitOfWork.AccountService.CreateAccountValidationAsync(accountValidationRequest);
            if (!validation.IsValid)
            {
                Logger.Log(LogSeverity.Warning, $"Validation failed when creating business unit", nameof(CreateAsync));
                return ProviderOperationResult<CreateBusinessUnitResponseModel>.InvalidInput(validation.ErrorTarget, validation.ErrorMessage);
            }

            serviceRequest.RequestId = requestId;
            serviceRequest.CompanyId = userCompanyId;
            serviceRequest.ResellerId = userCompanyId;
            serviceRequest.ParentId = request.ParentId ?? userBusinessUnitId; //If Parent ID is not set use default CrmAccountId for this user
            serviceRequest.BillCycleOffset = request.BillingCycleStartDay;
            serviceRequest.BillCycleId = validation.BillCycleId;
            serviceRequest.RateKey = validation.RateKey.GetValueOrDefault();

            AccountContract serviceResponse = null;
            try
            {
                serviceResponse = await _serviceUnitOfWork.AccountService.AddAccountAsync(serviceRequest);
                if (serviceResponse != null && request.PersonId != Guid.Empty)
                {
                    await MakePersonAvailableToNewBusinessUnit(request.PersonId.GetValueOrDefault(), serviceResponse.Id);
                }
            }
            catch (FaultException<TeleenaServiceReferences.AccountService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, "error creating business unit", nameof(CreateAsync), ex);
                var errorResponse = ProviderOperationResult<CreateBusinessUnitResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(request), ex.Detail.ErrorDescription, ex.Detail.TicketId);
                return errorResponse;
            }
            if (serviceResponse == null)
            {
                throw new InvalidOperationException(MessageConstants.UnexpectedServiceNullResponseMessage);
            }

            var addressUpdateResponse = await UpdateAddresses(validation.AddressIds, serviceResponse.Id);
            if (addressUpdateResponse != null)
                return ProviderOperationResult<CreateBusinessUnitResponseModel>.TeleenaExceptionAsResult(addressUpdateResponse.ErrorCode, nameof(request.PersonId), addressUpdateResponse.ErrorDescription, addressUpdateResponse.TicketId);

            var hostName = _appConfiguration.GetConfigurationValue(ConfigurationConstants.RestfulApiDomainNameSection, ConfigurationConstants.RestAPIDomainName);
            var businessUnitApiPath = _appConfiguration.GetConfigurationValue(ConfigurationConstants.RestfullApiPathSection, ConfigurationConstants.BusinessUnitApi);

            var response = new CreateBusinessUnitResponseModel()
            {
                Id = serviceResponse.Id,
                Location = $"{hostName}/{businessUnitApiPath}/business-units/{serviceResponse.Id}"
            };

            return ProviderOperationResult<CreateBusinessUnitResponseModel>.AcceptedResult(response);
        }

        /// <summary>
        /// Search for business units
        /// </summary>
        /// <param name="request">contains search criteria</param>
        /// <returns></returns>
        public async Task<BusinessUnitListModel> GetBusinessUnitsWithFilteringAsync(GetBusinessUnitRequest request)
        {
            var businessUnitLoadingStrategy = _producerFactory.GetLoader(request);
            if (businessUnitLoadingStrategy == null)
            {
                throw new InvalidOperationException("No suitable loading strategy found to load a list of business units for current user based on given domain request");
            }

            var filterStrategy = _producerFactory.GetFilterForRequest(request);
            if (filterStrategy == null)
            {
                throw new InvalidOperationException("No suitable filter strategy found to filter by specified criteria");
            }

            var pricePlanContracts = await GetPricePlanContracts(request);

            List<AccountContract> serviceBusinessUnits = await businessUnitLoadingStrategy.LoadBusinessUnitsAsync(request, _serviceUnitOfWork);
            List<AccountContract> filteredBusinessUnits = filterStrategy.FilterBusinessUnitsByRequest(serviceBusinessUnits, request);

            List<BusinessUnitExtraInfoModel> nonAccountContractData = await GetNonAccountContractData(filteredBusinessUnits);

            var result = _translators.AccountContractTranslator.Translate(filteredBusinessUnits, nonAccountContractData, pricePlanContracts, request.IncludeChildren);
            return result;
        }

        private async Task<List<PricePlanContract>> GetPricePlanContracts(GetBusinessUnitRequest request)
        {
            var getPricePlansContract = new GetPricePlansByAccountOrCompanyContract()
            {
                AccountId = request.UserBusinessUnitId,
                CrmCompanyId = request.UserCompanyId
            };

            return await _serviceUnitOfWork.AccountService.GetPricePlansForAccountAsync(getPricePlansContract);
        }

        private async Task<List<BusinessUnitExtraInfoModel>> GetNonAccountContractData(List<AccountContract> filteredBusinessUnits)
        {
            if (filteredBusinessUnits == null || filteredBusinessUnits.Count == 0)
                return new List<BusinessUnitExtraInfoModel>();

            List<BusinessUnitExtraInfoModel> resultList = new List<BusinessUnitExtraInfoModel>();

            var accountIds = filteredBusinessUnits.Select(x => x.Id).ToList();

            var addOns = await GetAddOnsByBusinessUnit(accountIds);
            var propositions = await GetPropositionsByBusinessUnitId(accountIds);

            foreach (var account in filteredBusinessUnits)
            {
                var addOnsPerBusinessUnit = new BusinessUnitExtraInfoModel
                {
                    BusinessUnitId = account.Id,
                    AddOns = addOns.Where(x => x.BusinessUnitId == account.Id).ToList(),
                    Propositions = propositions.Where(x => x.AccountId == account.Id).ToList()
                };

                resultList.Add(addOnsPerBusinessUnit);
            }


            return resultList;
        }

        private async Task<List<SimpleAddOnContract>> GetAddOnsByBusinessUnit(List<Guid> businesUnitIds)
        {
            var result = await _serviceUnitOfWork.AddOnService.GetAllowedAddOnsForListOfBusinessUnitsAsync(new BusinessUnitListContract() { BusinessUnitIds = businesUnitIds });

            return result;
        }

        private async Task<List<AllowedPropositionContract>> GetPropositionsByBusinessUnitId(List<Guid> businesUnitIds)
        {
            List<AllowedPropositionContract> propositions = await _serviceUnitOfWork.PropositionService.GetAllowedPropositionsByAccountIdsAsync(new BusinessUnitsContract() { BusinessUnitIds = businesUnitIds });
            return propositions;
        }

        public async Task<ProviderOperationResult<object>> UpdateBusinessUnitAsync(Guid id, BusinessUnitPatchModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //GET Business unit to update
            var currentBusinessUnit = await _serviceUnitOfWork.AccountService.GetAccountByIdAsync(id);
            if (currentBusinessUnit == null)
                throw new Exception(MessageConstants.UnexpectedServiceNullResponseMessage);

            var isPersonChanged = currentBusinessUnit.PersonId.GetValueOrDefault() != model.PersonId.GetValueOrDefault();

            LoadAddressData oldAddresses = null;
            LoadAddressData newAddresses = null;
            if (model.IsPersonIdSet && isPersonChanged)
            {
                var personValidationResponse = await _serviceUnitOfWork.AccountService.AccountPersonValidationAsync(new AccountPersonValidationContract
                {
                    AccountId = id,
                    PersonId = model.PersonId.Value
                });

                if (!personValidationResponse.IsValid)
                    return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.BadRequest, personValidationResponse.ErrorTarget, personValidationResponse.ErrorMessage);

                if (currentBusinessUnit.PersonId.GetValueOrDefault() != Guid.Empty)
                {
                    oldAddresses = await LoadAddresses(currentBusinessUnit.PersonId.GetValueOrDefault());
                    if (!oldAddresses.IsSuccess)
                        return ProviderOperationResult<object>.InvalidInput(nameof(model.PersonId), oldAddresses.ErrorMessage);
                }

                newAddresses = await LoadAddresses(model.PersonId.GetValueOrDefault());
                if (!newAddresses.IsSuccess)
                    return ProviderOperationResult<object>.InvalidInput(nameof(model.PersonId), newAddresses.ErrorMessage);
            }

            int pricePlan = currentBusinessUnit.RateKey;
            if (model.IsWholesalePriceplanSet && model.WholesalePriceplan != null)
            {
                var pricePlans = await _serviceUnitOfWork.AccountService.GetPricePlansForAccountAsync(new GetPricePlansByAccountOrCompanyContract() { CrmCompanyId = currentBusinessUnit.CompanyId.GetValueOrDefault() });
                var foundPricePlan = pricePlans.FirstOrDefault(x => x.Description == model.WholesalePriceplan);
                if (foundPricePlan == null)
                {
                    return ProviderOperationResult<object>
                        .InvalidInput(nameof(model.WholesalePriceplan), "Wholesale Priceplan does not exist for this company.");
                }
                pricePlan = foundPricePlan.RateKey;
            }

            var updateContract = new UpdateAccountContract
            {
                Id = id,
                CompanyId = currentBusinessUnit.CompanyId,
                UserName = model.IsNameSet ? model.Name : currentBusinessUnit.UserName,
                CustomerNumber = model.IsCustomerIdSet ? model.CustomerId : currentBusinessUnit.CustomerNumber,
                ContractNumber = model.ContractNumber ?? currentBusinessUnit.ContractNumber,
                PersonId = model.IsPersonIdSet ? model.PersonId : currentBusinessUnit.PersonId,
                RateKey = pricePlan
            };

            AccountContract response = null;
            try
            {
                response = await _serviceUnitOfWork.AccountService.UpdateAccountAsync(updateContract);
                if (response != null && updateContract.PersonId.HasValue)
                {
                    await MakePersonAvailableToNewBusinessUnit(updateContract.PersonId.Value, id);
                }
            }
            catch (FaultException<TeleenaServiceReferences.AccountService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, "Teleena exception while updating business unit", nameof(UpdateBusinessUnitAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(UpdateAccountContract), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (response == null)
                throw new InvalidOperationException(MessageConstants.UnexpectedServiceNullResponseMessage);

            if (oldAddresses != null)
            {
                var oldAddressResult = await UpdateAddresses(oldAddresses.Addresses.Select(a => a.Id).ToList(), Guid.Empty); // use this value to set db value to null
                if (oldAddressResult != null)
                    return ProviderOperationResult<object>.TeleenaExceptionAsResult(oldAddressResult.ErrorCode, nameof(model.PersonId), oldAddressResult.ErrorDescription, oldAddressResult.TicketId);
            }

            if (newAddresses != null)
            {
                var newAddressResult = await UpdateAddresses(newAddresses.Addresses.Select(a => a.Id).ToList(), id);
                if (newAddressResult != null)
                    return ProviderOperationResult<object>.TeleenaExceptionAsResult(newAddressResult.ErrorCode, nameof(model.PersonId), newAddressResult.ErrorDescription, newAddressResult.TicketId);
            }

            if (model.IsPropositionsSet)
            {
                var propositionUpdateResult = await UpdatePropositionsAsync(id, model, currentBusinessUnit);
                if (!propositionUpdateResult.Success)
                {
                    Logger.Log(LogSeverity.Error, "Error while updating business unit propositions", propositionUpdateResult.Errors?.FirstOrDefault());
                    return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.BadRequest, nameof(model.Propositions), propositionUpdateResult.Errors?.FirstOrDefault());
                }
            }

            if (model.IsAddOnIdsSet)
            {
                var addAddOnResult = await UpdateAddOnsAsync(model.AddOnIds.ToList(), id);
                if (!addAddOnResult.IsSuccess)
                {
                    return addAddOnResult;
                }
            }

            return ProviderOperationResult<object>.OkResult();
        }

        private async Task<PropositionSaveResultContract> UpdatePropositionsAsync(Guid businessUnitId, BusinessUnitPatchModel model, AccountContract currentBusinessUnit)
        {
            try
            {
                var propositionUpdateResult = await _serviceUnitOfWork.PropositionService.AddOrUpdateAllowedPropositionsAsync(new AddOrUpdateAllowedPropositionsContract
                {
                    AccountId = businessUnitId,
                    PersonId = model.IsPersonIdSet ? model.PersonId : currentBusinessUnit.PersonId,
                    ParentId = currentBusinessUnit.ParentId,
                    CompanyId = currentBusinessUnit.CompanyId,
                    Propositions = model.Propositions
                        .Select(x => new AllowedPropositionContract
                        {
                            PropositionId = x.Id,
                            EndUserSubscription = x.EndUserSubscription
                        }).ToList()
                }).ConfigureAwait(false);

                return propositionUpdateResult;
            }
            catch(FaultException<TeleenaServiceReferences.PropositionService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, "Teleena exception while updating allowed propositions", nameof(UpdateBusinessUnitAsync), ex);
                return new PropositionSaveResultContract { Success = false, Errors = new List<string> { ex.Detail.ErrorDescription } };
            }
        }

        private async Task<ProviderOperationResult<object>> UpdateAddOnsAsync(List<Guid> addOnIds, Guid businessUnitId)
        {
            var allAddOnIds = new List<Guid>();

            var alreadyAllowedAddons = await _serviceUnitOfWork.AddOnService.GetAllowedAddOnsForBusinessUnitAsync(businessUnitId);
            var alreadyAllowedAddonIds = alreadyAllowedAddons.AddOnContracts.Select(x => x.Id).ToList();

            allAddOnIds.AddRange(alreadyAllowedAddonIds);
            allAddOnIds.AddRange(addOnIds);

            var setOfIds = new HashSet<Guid>();
            var duplicates = allAddOnIds.Where(x => !setOfIds.Add(x)).Distinct();
            if (duplicates.Any())
            {
                string duplicatesStr = string.Join(", ", duplicates);
                return ProviderOperationResult<object>.InvalidInput(nameof(businessUnitId), $"Add-on(s) [{duplicatesStr}] are duplicates or already linked to business unit with id {businessUnitId}.");
            }

            AddAddOnsToBusinessUnitContract contract = new AddAddOnsToBusinessUnitContract
            {
                AddOnIds = addOnIds,
                BusinessUnitId = businessUnitId
            };
            var response = await _serviceUnitOfWork.AddOnService.AddAllowedAddOnsToBusinessUnitAsync(contract);
            if (!response.IsSuccess)
            {
                return ProviderOperationResult<object>.GenerateFailureResult(System.Net.HttpStatusCode.BadRequest, "General Error", response.Message);
            }
            return ProviderOperationResult<object>.AcceptedResult();
        }
    }
}
