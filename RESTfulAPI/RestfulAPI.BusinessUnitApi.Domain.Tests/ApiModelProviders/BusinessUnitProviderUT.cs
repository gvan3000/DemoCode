using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.AddressService;
using RestfulAPI.TeleenaServiceReferences.BillCycleService;
using RestfulAPI.TeleenaServiceReferences.PersonService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.Tests.ApiModelProviders
{
    [TestClass]
    public class BusinessUnitProviderUT
    {
        #region Private Fields

        private List<AccountContract> accountContractFilledResponse;
        private GetAccountWithChildAccountsContract accountWithChildRequest;
        private List<AccountContract> accountContractListResponse;
        private BusinessUnitModel businessUnitResponse;
        private AddAccountContract serviceRequest;
        private AccountContract serviceResponse;
        private UpdateAccountContract updateAccountRequest;
        private AccountContract accountContract;
        private BusinessUnitPatchModel patchModel;
        private PropositionsContract propositionsContract;
        private Guid propositionId;
        private Mock<IBusinessUnitApiTranslators> mockTranslators;
        private Mock<ITeleenaServiceUnitOfWork> mockService;
        private Mock<IBusinessUnitProducerFactory> mockProducer;
        private Mock<ICustomAppConfiguration> mockCustomAppConfig;
        private Mock<IJsonRestApiLogger> mockLogger;
        private UpdateBusinessUnitPropositionsContract updateBUPropositionsContract;
        private AddOnsContract addOnsContractResponse;

        #endregion

        [TestInitialize]
        public void SetupEachTest()
        {
            Guid accountId = Guid.NewGuid();
            accountContractFilledResponse = new List<AccountContract>
            {
               new AccountContract { Id = accountId }
            };
            addOnsContractResponse = new AddOnsContract
            {
                AddOnContracts = new List<AddOnContract> { new AddOnContract { Id = Guid.NewGuid(), Name = "test_adon" } }
            };
            updateBUPropositionsContract = new UpdateBusinessUnitPropositionsContract { Propositions = new List<UpdateBusinessUnitPropositionContract> { new UpdateBusinessUnitPropositionContract { EndUserSubscritpion = true } } };
            propositionId = Guid.NewGuid();
            patchModel = new BusinessUnitPatchModel();
            accountWithChildRequest = new GetAccountWithChildAccountsContract();
            accountContractListResponse = new List<AccountContract>();
            businessUnitResponse = new BusinessUnitModel();
            serviceRequest = new AddAccountContract() { PersonId = Guid.NewGuid() };
            serviceRequest.Propositions = new List<AccountPropositionsContract>();
            serviceRequest.Propositions.Add(new AccountPropositionsContract() { PropositionId = propositionId });
            serviceResponse = new AccountContract();
            updateAccountRequest = new UpdateAccountContract();
            accountContract = new AccountContract();
            propositionsContract = new PropositionsContract()
            {
                PropositionContracts = new List<PropositionContract>()
            };
            propositionsContract.PropositionContracts.Add(new PropositionContract() { Id = propositionId });

            mockTranslators = new Mock<IBusinessUnitApiTranslators>();

            mockTranslators.Setup(x => x.CreateBusinessUnitTranslator.Translate(It.IsAny<BusinessUnitCreateModel>()))
                .Returns(serviceRequest);

            mockTranslators.Setup(x => x.AccountContractTranslator.Translate(It.IsAny<List<AccountContract>>(), It.IsAny<List<BusinessUnitExtraInfoModel>>(), It.IsAny<List<PricePlanContract>>(), It.IsAny<bool>()))
                .Returns(new BusinessUnitListModel() { BusinessUnits = new List<BusinessUnitModel>() { businessUnitResponse } });
            mockTranslators.Setup(x => x.AccountContractTranslator.Translate(It.IsAny<AccountContract>()))
                .Returns(businessUnitResponse);

            mockTranslators.Setup(x => x.BusinessUnitPatchModelPropositionsTranslator.Translate(It.IsAny<BusinessUnitPatchModel>()))
                .Returns(updateBUPropositionsContract);

            mockService = new Mock<ITeleenaServiceUnitOfWork>();
            mockService.Setup(x => x.BillCycleService.GetByCompanyIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new CompanyBillCycleListContract()
                {
                    BillCycles = new BillCycleContract[]
                    {
                        new BillCycleContract()
                        {
                            MaxOffsetDays = 8
                        }
                    }
                });

            mockService.Setup(x => x.AccountService.GetPricePlansForAccountAsync(It.IsAny<GetPricePlansByAccountOrCompanyContract>()))
                .ReturnsAsync(new List<PricePlanContract>()
                {
                    new PricePlanContract()
                    {
                        RateKey = 1,
                        Description = "wpp"
                    }
                });

            mockService.Setup(x => x.AccountService.AddAccountAsync(It.IsAny<AddAccountContract>()))
                .ReturnsAsync(serviceResponse);

            mockService.Setup(x => x.AccountService.GetAccountsByCompanyAsync(It.IsAny<GetAccountsByCompanyContract>()))
                .ReturnsAsync(accountContractListResponse);

            mockService.Setup(x => x.AccountService.GetAccountWithChildAccountsByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(accountContractListResponse);

            mockService.Setup(x => x.AccountService.UpdateAccountAsync(It.IsAny<UpdateAccountContract>()))
                .ReturnsAsync(accountContract);

            mockService.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(accountContract);

            mockService.Setup(x => x.AccountService.AccountPersonValidationAsync(It.IsAny<AccountPersonValidationContract>()))
                .ReturnsAsync(new ValidationResponseContract { IsValid = true });

            mockService.Setup(x => x.PropositionService.GetActivePropositionsByCompanyAsync(It.IsAny<Guid>()))
                .ReturnsAsync(propositionsContract);

            mockService.Setup(x => x.PropositionService.GetActivePropositionsForCreateBusinessUnitAsync(It.IsAny<Guid?>(), It.IsAny<Guid>()))
                .ReturnsAsync(propositionsContract);

            mockService.Setup(x => x.PropositionService.GetPropositionByCodeAsync(It.IsAny<string>())).ReturnsAsync(new PropositionContract() { Id = propositionId });

            mockService.Setup(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new PersonContract() { Id = Guid.NewGuid() });

            mockService.Setup(x => x.PersonService.ChangePersonOwnerAsync(It.IsAny<ChangePersonOwnerRequest>()))
                .Returns(Task.FromResult(default(object)));

            mockService.Setup(x => x.AddressService.UpdateAddressAsync(It.IsAny<UpdateAddressContract>()))
                .ReturnsAsync(new AddressContract() { Id = Guid.NewGuid() });

            mockService.Setup(x => x.AddressService.GetAddressesByPersonAsync(It.IsAny<GetAddressByPersonContract>()))
                .ReturnsAsync(new List<AddressContract>() { new AddressContract()
                {
                    Email="asdas@we.com",
                    MobilePhone="65654984"
                }});

            mockService.Setup(x => x.AddOnService.AddAllowedAddOnsToBusinessUnitAsync(It.IsAny<AddAddOnsToBusinessUnitContract>()))
                .ReturnsAsync(new AddAddOnsToBusinessUnitResponse() { IsSuccess = true });

            mockService.Setup(x => x.CommercialOfferService.GetCommercailOfferPropositionForAsync(It.IsAny<string>()))
                .ReturnsAsync(new TeleenaServiceReferences.CommercialOfferService.CommercialOfferPropositionContract()
                {
                    Offers = new List<TeleenaServiceReferences.CommercialOfferService.CommercialOfferConfigurationContract>()
                    {
                        new TeleenaServiceReferences.CommercialOfferService.CommercialOfferConfigurationContract()
                        {
                            IsSharedWallet=true
                        }
                    }
                });

            mockService.Setup(x => x.PropositionService.UpdateBusinessUnitPropositionAsync(It.IsAny<UpdateBusinessUnitPropositionsContract>()))
                .Returns(Task.FromResult(new object()));

            mockService.Setup(x => x.PropositionService.AddOrUpdateAllowedPropositionsAsync(It.IsAny<AddOrUpdateAllowedPropositionsContract>()))
                .ReturnsAsync(new PropositionSaveResultContract { Success = true });

            mockService.Setup(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new PersonContract()
                {
                    Mobile = "548668864",
                    Email = "dvvvzv@fs.ff"
                });

            mockService.Setup(x => x.AccountService.CreateAccountValidationAsync(It.IsAny<AccountValidationContract>()))
                .ReturnsAsync(new AccountValidationResponseContract()
                {
                    IsValid = true
                });

            mockService.Setup(x => x.AddOnService.GetAllowedAddOnsForBusinessUnitAsync(It.IsAny<Guid>())).ReturnsAsync(addOnsContractResponse);
            mockService.Setup(x => x.AddOnService.GetAllowedAddOnsForListOfBusinessUnitsAsync(It.IsAny<BusinessUnitListContract>()))
                .ReturnsAsync(new List<SimpleAddOnContract>()
                {
                    new SimpleAddOnContract()
                    {
                        BusinessUnitId = accountId,
                        AddOnId = Guid.NewGuid(),
                        AddOnType = "some_type",
                        IsOneTime = true
                    }
                });
            mockService.Setup(x => x.PropositionService.GetAllowedPropositionsByAccountIdsAsync(It.IsAny<BusinessUnitsContract>()))
                .ReturnsAsync(new List<AllowedPropositionContract>() { new AllowedPropositionContract() });
            var mockLoader = new Mock<IBusinessUnitLoadingStrategy>();
            mockLoader.Setup(x => x.LoadBusinessUnitsAsync(It.IsAny<GetBusinessUnitRequest>(), It.IsAny<ITeleenaServiceUnitOfWork>()))
                .ReturnsAsync(accountContractFilledResponse);

            var mockFilter = new Mock<IBusinessUnitFilter>();
            mockFilter.Setup(x => x.FilterBusinessUnitsByRequest(It.IsAny<List<AccountContract>>(), It.IsAny<GetBusinessUnitRequest>()))
                .Returns(accountContractFilledResponse);

            mockProducer = new Mock<IBusinessUnitProducerFactory>();
            mockProducer.Setup(x => x.GetLoader(It.IsAny<GetBusinessUnitRequest>()))
                .Returns(mockLoader.Object);
            mockProducer.Setup(x => x.GetFilterForRequest(It.IsAny<GetBusinessUnitRequest>()))
                .Returns(mockFilter.Object);

            mockCustomAppConfig = new Mock<ICustomAppConfiguration>();
            mockCustomAppConfig.Setup(x => x.GetConfigurationValue(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());

            mockLogger = new Mock<IJsonRestApiLogger>(MockBehavior.Loose);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateAsync_ShouldThrowWhenInputParameterIsNull()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = providerUnderTest.CreateAsync(Guid.NewGuid(), null, null, Guid.NewGuid()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldCallAccountContractTranslator()
        {
            var businessUnitProviderMocked = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = businessUnitProviderMocked.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest()).ConfigureAwait(false).GetAwaiter().GetResult();

            mockTranslators.Verify(t => t.AccountContractTranslator.Translate(It.IsAny<List<AccountContract>>(), It.IsAny<List<BusinessUnitExtraInfoModel>>(), It.IsAny<List<PricePlanContract>>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldReturn_BusinessUnitListModel_IfInputIsValid()
        {
            var businessUnitProviderMocked = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = businessUnitProviderMocked.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.AreNotEqual(0, response.BusinessUnits.Count);
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldUseBusinessUnitProducerFactoryForBothLoaderAndFilter()
        {
            var businessUnitProviderMocked = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = businessUnitProviderMocked.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest()).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);

            mockProducer.Verify(x => x.GetLoader(It.IsAny<GetBusinessUnitRequest>()), Times.Once);
            mockProducer.Verify(x => x.GetFilterForRequest(It.IsAny<GetBusinessUnitRequest>()), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetBusinessUnit_ShouldThrowIfProducerReturnsNullLoader()
        {
            mockProducer.Setup(x => x.GetLoader(It.IsAny<GetBusinessUnitRequest>()))
                .Returns(default(IBusinessUnitLoadingStrategy));

            var businessUnitProviderMocked = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = businessUnitProviderMocked.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetBusinessUnit_ShouldThrowIfProducerReturnsNullFilter()
        {
            mockProducer.Setup(x => x.GetFilterForRequest(It.IsAny<GetBusinessUnitRequest>()))
                .Returns(default(IBusinessUnitFilter));

            var businessUnitProviderMocked = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = businessUnitProviderMocked.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest()).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldCall_AddOnsService_GetAllowedAddOnsForBusinessUnitAsync_IfBUsAreFound()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = providerUnderTest.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest { FilterHasSharedWallet = "true", UserBusinessUnitId = Guid.NewGuid() }).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);

            mockService.Verify(x => x.AddOnService.GetAllowedAddOnsForListOfBusinessUnitsAsync(It.IsAny<BusinessUnitListContract>()), Times.Once);
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldCall_PropositionService_GetActivePropositionsByBusinessUnitAsync_IfBUsAreFound()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = providerUnderTest.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest { FilterHasSharedWallet = "true", UserBusinessUnitId = Guid.NewGuid() }).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);

            mockService.Verify(x => x.PropositionService.GetAllowedPropositionsByAccountIdsAsync(It.IsAny<BusinessUnitsContract>()), Times.Once);
        }

        [TestMethod]
        public void GetBusinessUnit_ShouldNotCallAddOnAndPropositionServicesIfNoBusinessUnitsSurviveFiltering()
        {
            var mockFilterStrategy = new Mock<IBusinessUnitFilter>();
            mockFilterStrategy.Setup(x => x.FilterBusinessUnitsByRequest(It.IsAny<List<AccountContract>>(), It.IsAny<GetBusinessUnitRequest>()))
                .Returns(new List<AccountContract>());
            mockProducer.Setup(x => x.GetFilterForRequest(It.IsAny<GetBusinessUnitRequest>()))
                .Returns(mockFilterStrategy.Object);
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = providerUnderTest.GetBusinessUnitsWithFilteringAsync(new GetBusinessUnitRequest { FilterHasSharedWallet = "true", UserBusinessUnitId = Guid.NewGuid() }).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);

            mockService.Verify(x => x.PropositionService.GetAllowedPropositionsByAccountIdsAsync(It.IsAny<BusinessUnitsContract>()), Times.Never);
            mockService.Verify(x => x.AddOnService.GetAllowedAddOnsForListOfBusinessUnitsAsync(It.IsAny<BusinessUnitListContract>()), Times.Never);
        }

        [TestMethod]
        public void UpdateAsync_ShouldReturnBusinessUnitModel()
        {
            var request = new BusinessUnitPatchModel();
            var expectedId = Guid.NewGuid();
            accountContract.AccountId = expectedId.ToString();

            var provider = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = provider.UpdateBusinessUnitAsync(expectedId, request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsInstanceOfType(response, typeof(ProviderOperationResult<object>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateAsync_ShouldReturnArgumentNullException_IfPatchModelIsNull()
        {
            var expectedId = Guid.NewGuid();
            BusinessUnitPatchModel patchModel = null;

            var provider = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = provider.UpdateBusinessUnitAsync(expectedId, patchModel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateAsync_ShouldReturnExceptionWithMessage_IfAccountServiceReturnsNull()
        {
            var expectedId = Guid.NewGuid();
            var request = new BusinessUnitPatchModel();
            AccountContract updatedContract = null;

            mockService.Setup(x => x.AccountService.UpdateAccountAsync(It.IsAny<UpdateAccountContract>()))
                .ReturnsAsync(updatedContract);

            var provider = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var response = provider.UpdateBusinessUnitAsync(expectedId, request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateAsync_ShouldCallUpdatePropositionWhenNeeded()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                Propositions = new Proposition[]
                {
                    new Proposition() { Id = Guid.NewGuid(), EndUserSubscription = true }
                }
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockService.Verify(x => x.PropositionService.AddOrUpdateAllowedPropositionsAsync(It.IsAny<AddOrUpdateAllowedPropositionsContract>()), Times.Once);
        }

        [TestMethod]
        public void UpdateAsync_ShouldNotCallUpdatePropositionsWhenNoPropositionsSet()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                Name = "bla"
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockService.Verify(x => x.PropositionService.UpdateBusinessUnitPropositionAsync(It.IsAny<UpdateBusinessUnitPropositionsContract>()), Times.Never);
        }

        [TestMethod]
        public void UpdateAsync_ShouldCallChangePersonOwner()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            mockService.Verify(x => x.PersonService.ChangePersonOwnerAsync(It.IsAny<ChangePersonOwnerRequest>()), Times.Once);
        }

        [TestMethod]
        public void UpdateAsync_ShouldNotThrowWhenUpdatePropositionsThrows()
        {
            mockService.Setup(x => x.PropositionService.AddOrUpdateAllowedPropositionsAsync(It.IsAny<AddOrUpdateAllowedPropositionsContract>()))
                .ReturnsAsync(new PropositionSaveResultContract { Success = false });

            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                Propositions = new Proposition[]
                {
                    new Proposition() { Id = Guid.NewGuid(), EndUserSubscription = true }
                }
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public void UpdateAsync_ShouldCallPersonServiceGetWhenPersonIdSet()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            mockService.Verify(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void UpdateAsync_ShouldNotCallPersonServiceGetIfPersonIsNotSetInContract()
        {
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                Name = "bla" // PersonId not set here
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            mockService.Verify(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<TeleenaServiceReferences.PersonService.TeleenaInnerExp>))]
        public void UpdateAsync_ShouldThrowWhenPersonServiceForGetThrows()
        {
            mockService.Setup(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new FaultException<TeleenaServiceReferences.PersonService.TeleenaInnerExp>(new TeleenaServiceReferences.PersonService.TeleenaInnerExp()));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateAsync_ShouldFailWhenPersonSericeForGetReturnsNull()
        {
            mockService.Setup(x => x.PersonService.GetPersonByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(default(PersonContract));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);

            mockService.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountContract() { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() });

            response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        public void UpdateAsync_ShouldFailWhenAddresServiceGetReturnsNull()
        {
            mockService.Setup(x => x.AddressService.GetAddressesByPersonAsync(It.IsAny<GetAddressByPersonContract>()))
                .ReturnsAsync(default(List<AddressContract>));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsSuccess);
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException<TeleenaServiceReferences.AddressService.TeleenaInnerExp>))]
        public void UpdateAsync_ShouldThrowWhenAddressServiceGetThrows()
        {
            mockService.Setup(x => x.AddressService.GetAddressesByPersonAsync(It.IsAny<GetAddressByPersonContract>()))
                .ThrowsAsync(new FaultException<TeleenaServiceReferences.AddressService.TeleenaInnerExp>(new TeleenaServiceReferences.AddressService.TeleenaInnerExp()));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateAsync_ShouldCallAddressServiceUpdateTwiceWhenAccountServiceReturnesPerson()
        {
            // in this particular case sicne mock returns only one address anyway we are calling update 2 times, once for new person, and once for old one to unlink it from account
            mockService.Setup(x => x.AccountService.GetAccountByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountContract() { Id = Guid.NewGuid(), PersonId = Guid.NewGuid() });
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
            mockService.Verify(x => x.AddressService.UpdateAddressAsync(It.IsAny<UpdateAddressContract>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UpdateAsync_ShouldThrowWhenAddressServiceUpdateReturnsNull()
        {
            mockService.Setup(x => x.AddressService.UpdateAddressAsync(It.IsAny<UpdateAddressContract>()))
                .ReturnsAsync(default(AddressContract));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void UpdateAsync_ShouldFailWhenAddressServiceUpdateThrowsFaultException()
        {
            mockService.Setup(x => x.AddressService.UpdateAddressAsync(It.IsAny<UpdateAddressContract>()))
                .ThrowsAsync(new FaultException<TeleenaServiceReferences.AddressService.TeleenaInnerExp>(new TeleenaServiceReferences.AddressService.TeleenaInnerExp()));
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsFalse(response.IsSuccess);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [TestMethod]
        public void UpdateAsync_ShouldNotBeCalledIfAddOnIsNotSet()
        {
            var providerUnderTest =
                new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);
            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            mockService.Verify(x => x.AddOnService.AddAllowedAddOnsToBusinessUnitAsync(It.IsAny<AddAddOnsToBusinessUnitContract>()), Times.Never);
        }

        [TestMethod]
        public void UpdateAsync_ShouldFetchWholesalePricePlansAndValidatePricePlanIntoRateKey()
        {

            mockService.Setup(x => x.AccountService.GetPricePlansForAccountAsync(It.IsAny<GetPricePlansByAccountOrCompanyContract>()))
                .ReturnsAsync(new List<PricePlanContract>()
                {
                    new PricePlanContract()
                    {
                        Description = "bla",
                        RateKey = 33
                    },
                    new PricePlanContract()
                    {
                        Description = "test",
                        RateKey = 1
                    }
                });
            var providerUnderTest = new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);
            var request = new BusinessUnitPatchModel()
            {
                WholesalePriceplan = "bla"
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            mockService.Verify(x => x.AccountService.GetPricePlansForAccountAsync(It.IsAny<GetPricePlansByAccountOrCompanyContract>()), Times.Once);
            mockService.Verify(x => x.AccountService.UpdateAccountAsync(It.Is<UpdateAccountContract>(c => c.RateKey == 33)), Times.Once);
        }

        [TestMethod]
        public void UpdateAsync_ShouldReturnBadRequestForAddOnDuplicatesInInput()
        {
            var providerUnderTest =
                 new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var addOnId = Guid.NewGuid();
            var addOnIdsDuplicates = new List<Guid>() { addOnId, addOnId };
            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid(),
                AddOnIds = addOnIdsDuplicates.ToArray()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
        }


        [TestMethod]
        public void UpdateAsync_ShouldReturnBadRequestForAddOnAlreadyAllowed()
        {
            var providerUnderTest =
                 new BusinessUnitProvider(mockLogger.Object, mockService.Object, mockTranslators.Object, mockProducer.Object, mockCustomAppConfig.Object);

            var addOnId = Guid.NewGuid();
            var alreadyAllowedAddOnContractResponse = new AddOnsContract
            {
                AddOnContracts = new List<AddOnContract> { new AddOnContract { Id = addOnId, Name = "already_allowed_addon" } }
            };

            mockService.Setup(x => x.AddOnService.GetAllowedAddOnsForBusinessUnitAsync(addOnId)).ReturnsAsync(alreadyAllowedAddOnContractResponse);

            var addOnIdsDuplicates = new List<Guid>() { addOnId, addOnId };
            var request = new BusinessUnitPatchModel()
            {
                PersonId = Guid.NewGuid(),
                AddOnIds = addOnIdsDuplicates.ToArray()
            };

            var response = providerUnderTest.UpdateBusinessUnitAsync(Guid.NewGuid(), request).ConfigureAwait(false).GetAwaiter().GetResult();

            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.HttpResponseCode);
        }



    }
}
