using System;
using System.Collections.Generic;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.DepartmentModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.ProductTypeModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators.AddOn;
using RestfulAPI.BusinessUnitApi.Domain.Translators.APNTranslators;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Balance;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BalanceProfile;
using RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Department;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Notifications;
using RestfulAPI.BusinessUnitApi.Domain.Translators.PreferredLanguage;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Product;
using RestfulAPI.BusinessUnitApi.Domain.Translators.ProductImei;
using RestfulAPI.BusinessUnitApi.Domain.Translators.ProductType;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Quota;
using RestfulAPI.Common;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.DepartmentCostCenterService;
using RestfulAPI.TeleenaServiceReferences.MobileService;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;
using RestfulAPI.TeleenaServiceReferences.ProductImeiService;
using RestfulAPI.TeleenaServiceReferences.ProductService;
using RestfulAPI.TeleenaServiceReferences.ProductTypeService;
using RestfulAPI.TeleenaServiceReferences.PropositionService;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using RestfulAPI.TeleenaServiceReferences.Translators;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators
{
	public class BusinessUnitApiTranslators : IBusinessUnitApiTranslators
	{
		private readonly ISysCodeConstants _sysCodeConstants;
		private readonly IServiceTypeConfigurationProvider _configProvider;
		private readonly ICustomAppConfiguration _customAppConfiguration;

		public BusinessUnitApiTranslators(ISysCodeConstants sysCodeConstants,
										  IServiceTypeConfigurationProvider configProvider,
										  ICustomAppConfiguration customAppConfiguration)
		{
			_sysCodeConstants = sysCodeConstants;
			_configProvider = configProvider;
			_customAppConfiguration = customAppConfiguration;
		}

		private ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract> _createBusinessUnitPropositionTranslator;
		public ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract> CreateBusinessUnitPropositionTranslator
		{
			get
			{
				if (_createBusinessUnitPropositionTranslator == null)
				{
					_createBusinessUnitPropositionTranslator = new CreateBusinessUnitPropositionTranslator();
				}
				return _createBusinessUnitPropositionTranslator;
			}
		}

		private ITranslate<BusinessUnitCreateModel, AddAccountContract> _createBusinessUnitTranslator;
		public ITranslate<BusinessUnitCreateModel, AddAccountContract> CreateBusinessUnitTranslator
		{
			get
			{
				if (_createBusinessUnitTranslator == null)
				{
					_createBusinessUnitTranslator = new CreateBusinessUnitTranslator(CreateBusinessUnitPropositionTranslator);
				}
				return _createBusinessUnitTranslator;
			}
		}

		private ITranslate<GetProductResponse, ProductModel> _productModelTranslator;
		public ITranslate<GetProductResponse, ProductModel> ProductModelTranslator
		{
			get
			{
				if (_productModelTranslator == null)
				{
					_productModelTranslator = new ProductModelTranslator();
				}
				return _productModelTranslator;
			}
		}

		private ITranslate<GetProductResponse[], List<ProductModel>> _productListTranslator;
		public ITranslate<GetProductResponse[], List<ProductModel>> ProductListTranslator
		{
			get
			{
				if (_productListTranslator == null)
				{
					_productListTranslator = new ProductModelListTranslator(ProductModelTranslator);
				}
				return _productListTranslator;
			}
		}

		private ITranslate<ProductImeiByBusinessUnitDataContract[], ProductImeiListModel> _productImeiListTranslator;
		public ITranslate<ProductImeiByBusinessUnitDataContract[], ProductImeiListModel> ProductImeiListTranslator
		{
			get
			{
				if (_productImeiListTranslator == null)
				{
					_productImeiListTranslator = new ProductImeiModelListTranslator();
				}
				return _productImeiListTranslator;
			}
		}

		private IAccountContractTranslator _accountContractTranslator;

		public IAccountContractTranslator AccountContractTranslator
		{
			get
			{
				if (_accountContractTranslator == null)
					_accountContractTranslator = new AccountContractTranslator();

				return _accountContractTranslator;
			}
		}

		private IServiceTypeTranslator _serviceTypeTranslator;

		public IServiceTypeTranslator ServiceTypeTranslator
		{
			get
			{
				if (_serviceTypeTranslator == null)
				{
					_serviceTypeTranslator = new ServiceTypeTranslator(_configProvider);
				}
				return _serviceTypeTranslator;
			}
		}

		private IDataTypeCodeTranslator _dataTypeCodeTranslator;

		public IDataTypeCodeTranslator DataTypeCodeTranslator
		{
			get
			{
				if (_dataTypeCodeTranslator == null)
				{
					_dataTypeCodeTranslator = new DataTypeCodeTranslator(_configProvider);
				}
				return _dataTypeCodeTranslator;
			}
		}

		private IBalanceTranslator _balanceTranslator;

		public IBalanceTranslator BalanceTranslator
		{
			get
			{
				if (_balanceTranslator == null)
				{
					_balanceTranslator = new BalanceTranslator(ServiceTypeTranslator, DataTypeCodeTranslator);
				}
				return _balanceTranslator;
			}
		}

		private IPropositionContractTranslator _propositionsContractTranslator;

		public IPropositionContractTranslator PropositionsContractTranslator
		{
			get
			{
				if (_propositionsContractTranslator == null)
					_propositionsContractTranslator = new PropositionsContractTranslator();

				return _propositionsContractTranslator;
			}
		}

		private ITranslate<AllowedPropositionContract, Models.BusinessUnitModels.Proposition> _allowedPropositionContractTranslator;

		public ITranslate<AllowedPropositionContract, Models.BusinessUnitModels.Proposition> AllowedPropositionContractTranslator
		{
			get
			{
				if (_allowedPropositionContractTranslator == null)
					_allowedPropositionContractTranslator = new AllowedPropositionContractTranslator();

				return _allowedPropositionContractTranslator;
			}
		}

		private ITranslate<MsisdnContract[], AvailableMSISDNResponseModel> _msisdnContractTranslator;

		public ITranslate<MsisdnContract[], AvailableMSISDNResponseModel> MsisdnContractTranslator
		{
			get
			{
				if (_msisdnContractTranslator == null)
					_msisdnContractTranslator = new MsisdnContractTranslator();

				return _msisdnContractTranslator;
			}
		}

		private ITranslate<string, Guid> _simStatusTranslator;

		public ITranslate<string, Guid> SimStatusTranslator
		{
			get
			{
				if (_simStatusTranslator == null)
					_simStatusTranslator = new SimStatusTranslator(_sysCodeConstants);

				return _simStatusTranslator;
			}
		}

		private ITranslate<Guid, SimStatusWrapper> _simStatusGuidToStringTranslator;

		public ITranslate<Guid, SimStatusWrapper> SimStatusGuidToStringTranslator
		{
			get
			{
				if (_simStatusGuidToStringTranslator == null)
					_simStatusGuidToStringTranslator = new SimStatusGuidToStringTranslator(_sysCodeConstants);

				return _simStatusGuidToStringTranslator;
			}
		}

		private ITranslate<BusinessUnitPatchModel, UpdateBusinessUnitPropositionsContract> _buPatchModelPropositionsTranslator;

		public ITranslate<BusinessUnitPatchModel, UpdateBusinessUnitPropositionsContract> BusinessUnitPatchModelPropositionsTranslator
		{
			get
			{
				if (_buPatchModelPropositionsTranslator == null)
				{
					_buPatchModelPropositionsTranslator = new BusinessUnitPatchModelPropositionsTranslator();
				}

				return _buPatchModelPropositionsTranslator;
			}
		}

		ITranslate<PurchaseAddOnResultContract, AddAddOnResponseModel> _purchaseAddOnResulttranslator;

		public ITranslate<PurchaseAddOnResultContract, AddAddOnResponseModel> PurchaseAddOnResultTranslator
		{
			get
			{
				if (_purchaseAddOnResulttranslator == null)
				{
					_purchaseAddOnResulttranslator = new PurchaseAddOnResultTranslator();
				}

				return _purchaseAddOnResulttranslator;
			}
		}

		private IAllowedAddOnsContractTranslator _allowedAddOnsContract;
		public IAllowedAddOnsContractTranslator AllowedAddOnsContractTranslator
		{
			get
			{
				if (_allowedAddOnsContract == null)
				{
					_allowedAddOnsContract = new AllowedAddOnsContractTranslator();
				}

				return _allowedAddOnsContract;
			}
		}

		private IDeleteAddOnModelTranslator _deleteAddOnModelTranslator;
		public IDeleteAddOnModelTranslator DeleteAddOnModelTranslator
		{
			get
			{
				if (_deleteAddOnModelTranslator == null)
				{
					_deleteAddOnModelTranslator = new DeleteAddOnModelTranslator();
				}

				return _deleteAddOnModelTranslator;
			}
		}

		private IAddOnContractTranslator _addOnContractTranslator;

		public IAddOnContractTranslator AddOnContractTranslator
		{
			get
			{
				if (_addOnContractTranslator == null)
				{
					_addOnContractTranslator = new AddOnContractTranslator();
				}
				return _addOnContractTranslator;
			}
		}

		private ISaveQuotaDistributionContractTranslator _saveQuotaDistributionContractTranslator;
		public ISaveQuotaDistributionContractTranslator SaveQuotaDistributionContactTranslator
		{
			get
			{
				if (_saveQuotaDistributionContractTranslator == null)
				{
					_saveQuotaDistributionContractTranslator = new SaveQuotaDistributionContractTranslator();
				}

				return _saveQuotaDistributionContractTranslator;
			}
		}

		private ITranslate<ProductQuotaDistributionContract, BalanceQuotasListModel> _productQuotaDistributionContractTranslator;

		public ITranslate<ProductQuotaDistributionContract, BalanceQuotasListModel> ProductQuotaDistributionContractTranslator
		{
			get
			{
				if (_productQuotaDistributionContractTranslator == null)
				{
					_productQuotaDistributionContractTranslator = new ProductQuotaDistributionContractTranslator();
				}
				return _productQuotaDistributionContractTranslator;
			}
		}

		private ITranslate<ProductTypeContract[], ProductTypeListResponseModel> _productTypeContractTranslator;
		public ITranslate<ProductTypeContract[], ProductTypeListResponseModel> ProductTypeContractTranslator
		{
			get
			{
				if (_productTypeContractTranslator == null)
				{
					_productTypeContractTranslator = new ProductTypeContractTranslator();
				}
				return _productTypeContractTranslator;
			}
		}

		private ITranslate<BusinessUnitPurchasedAddOnListContract, AddOnCumulativeListModel> _businessUnitPurchasedAddOnsCumulativeTranslator;
		public ITranslate<BusinessUnitPurchasedAddOnListContract, AddOnCumulativeListModel> BusinessUnitPurchasedAddOnsCumulativeTranslator
		{
			get
			{
				if (_businessUnitPurchasedAddOnsCumulativeTranslator == null)
				{
					_businessUnitPurchasedAddOnsCumulativeTranslator = new BusinessUnitPurchasedAddOnsCumulativeTranslator();
				}

				return _businessUnitPurchasedAddOnsCumulativeTranslator;
			}
		}

		private ITranslate<SysCodeContract, BalanceProfileModel> _balanceProfileTranslator;
		public ITranslate<SysCodeContract, BalanceProfileModel> BalanceProfileTranslator
		{
			get
			{
				if (_balanceProfileTranslator == null)
					_balanceProfileTranslator = new BalanceProfileTranslator();

				return _balanceProfileTranslator;
			}
		}

		private ITranslate<SysCodeContract[], BalanceProfileListModel> _balanceProfileListTranslator;
		public ITranslate<SysCodeContract[], BalanceProfileListModel> BalanceProfileListTranslator
		{
			get
			{
				if (_balanceProfileListTranslator == null)
					_balanceProfileListTranslator = new BalanceProfileListTranslator(BalanceProfileTranslator);

				return _balanceProfileListTranslator;
			}
		}

		private ITranslate<List<ProductAllowedBalancesContract>, ProductAllowedBalanceList> _productAllowedBalancesTranslator;
		public ITranslate<List<ProductAllowedBalancesContract>, ProductAllowedBalanceList> ProductAllowedBalancesTranslator
		{
			get
			{
				if (_productAllowedBalancesTranslator == null)
				{
					_productAllowedBalancesTranslator = new ProductAllowedBalancesContractTranslator();
				}

				return _productAllowedBalancesTranslator;
			}
		}

		private ITranslate<AccountBalanceWithBucketsContract[], BalanceQuotasListModel> _balancesContractBalanceQuotaListModelTranslator;
		public ITranslate<AccountBalanceWithBucketsContract[], BalanceQuotasListModel> BalancesContractBalanceQuotaListModelTranslator
		{
			get
			{
				if (_balancesContractBalanceQuotaListModelTranslator == null)
				{
					_balancesContractBalanceQuotaListModelTranslator = new BalancesContractBalanceQuotaListModelTranslator(ServiceTypeTranslator, DataTypeCodeTranslator);
				}

				return _balancesContractBalanceQuotaListModelTranslator;
			}
		}

		private ITranslate<PropositionsContract, List<PropositionInfoModel>> _propositionInfoModelTranslator;
		public ITranslate<PropositionsContract, List<PropositionInfoModel>> PropositionInfoModelTranslator
		{
			get
			{
				if (_propositionInfoModelTranslator == null)
				{
					_propositionInfoModelTranslator = new PropositionInfoModelTranslator();
				}

				return _propositionInfoModelTranslator;
			}
		}

		private ITranslate<int?, PageSizeInfo> _pageSizeInfoTranslator;
		public ITranslate<int?, PageSizeInfo> PageSizeInfoTranslator
		{
			get
			{
				if (_pageSizeInfoTranslator == null)
				{
					_pageSizeInfoTranslator = new PageSizeInfoTranslator();
				}

				return _pageSizeInfoTranslator;
			}
		}

		private ITranslate<ApnSetWithDetailsContract[], APNSetList> _apnSetWithDetailsTranslator;

		public ITranslate<ApnSetWithDetailsContract[], APNSetList> APNTranslator
		{
			get
			{
				if (_apnSetWithDetailsTranslator == null)
				{
					_apnSetWithDetailsTranslator = new BusinessUnitAPNsTranslator();
				}

				return _apnSetWithDetailsTranslator;
			}
		}

		private IUpdateApnsTranslator _updateApnsTranslator;
		public IUpdateApnsTranslator UpdateApnsTranslator
		{
			get
			{
				if (_updateApnsTranslator == null)
					_updateApnsTranslator = new UpdateApnsTranslator();

				return _updateApnsTranslator;
			}
		}

		private ITranslate<ApnDetailContract[], APNsResponseModel> _apnsResponseModelTranslator;
		public ITranslate<ApnDetailContract[], APNsResponseModel> ApnsResponseModelTranslator
		{
			get
			{
				if (_apnsResponseModelTranslator == null)
				{
					_apnsResponseModelTranslator = new ApnsResponseModelTranslator();
				}

				return _apnsResponseModelTranslator;
			}
		}

		private IUpdateDefaultApnTranslator _updateDefaultApnTranslator;
		public IUpdateDefaultApnTranslator UpdateDefaultApnTranslator
		{
			get
			{
				if (_updateDefaultApnTranslator == null)
				{
					_updateDefaultApnTranslator = new UpdateDefaultApnTranslator();
				}

				return _updateDefaultApnTranslator;
			}
		}

		private IDeleteApnTranslator _deleteApnTranslator;
		/// <summary>
		/// Delete apn translator
		/// </summary>
		public IDeleteApnTranslator DeleteApnTranslator
		{
			get
			{
				if (_deleteApnTranslator == null)
				{
					_deleteApnTranslator = new DeleteApnTranslator();
				}

				return _deleteApnTranslator;
			}
		}

		private ICreateDepartmentContractTranslator _createDepartmentContractTranslator;
		/// <summary>
		/// Create Department Translator
		/// </summary>
		public ICreateDepartmentContractTranslator CreateDepartmentContractTranslator
		{
			get
			{
				if (_createDepartmentContractTranslator == null)
				{
					_createDepartmentContractTranslator = new CreateDepartmentContractTranslator();
				}

				return _createDepartmentContractTranslator;
			}
		}

		ITranslate<DepartmentCostCenterContract, CreateDepartmentResponseModel> _departmentModelTranslator;
		public ITranslate<DepartmentCostCenterContract, CreateDepartmentResponseModel> DepartmentModelTranslator
		{
			get
			{
				if (_departmentModelTranslator == null)
				{
					_departmentModelTranslator = new DepartmentModelTranslator(_customAppConfiguration);
				}

				return _departmentModelTranslator;
			}
		}

		private IGetDepartmentsModelTranslator _getDepartmentsModelTranslator;
		/// <summary>
		/// Get Department Translator
		/// </summary>
		public IGetDepartmentsModelTranslator GetDepartmentsModelTranslator
		{
			get
			{
				if (_getDepartmentsModelTranslator == null)
				{
					_getDepartmentsModelTranslator = new GetDepartmentsModelTranslator();
				}

				return _getDepartmentsModelTranslator;
			}
		}

		ITranslate<GetNotificationConfigurationListResponse, GetNotificationListDataModel> _notificationListDataModelTranslator;
		public ITranslate<GetNotificationConfigurationListResponse, GetNotificationListDataModel> NotificationListDataModelTranslator
		{
			get
			{
				if (_notificationListDataModelTranslator == null)
				{
					_notificationListDataModelTranslator = new NotificationListDataModelTranslator(NotificationTypeTranslator);
				}

				return _notificationListDataModelTranslator;
			}
		}

		private ITranslate<UpdateNotificationModel, UpdateBusinessUnitNotificationConfigurationContract> _updateNotificationConfigurationTranslator;
		public ITranslate<UpdateNotificationModel, UpdateBusinessUnitNotificationConfigurationContract> UpdateNotificationConfigurationTranslator
		{
			get
			{
				if (_updateNotificationConfigurationTranslator == null)
				{
					_updateNotificationConfigurationTranslator = new NotificationTranslators.UpdateBusinessUnitNotificationConfigurationTranslator(NotificationTypeTranslator);
				}

				return _updateNotificationConfigurationTranslator;
			}
		}

		private INotificationTypeTranslator _notificationTypeTranslator;
		public INotificationTypeTranslator NotificationTypeTranslator
		{
			get
			{
				if (_notificationTypeTranslator == null)
				{
					_notificationTypeTranslator = new NotificationTypeTranslator();
				}
				return _notificationTypeTranslator;
			}
		}

		private ITranslate<CreateNotificationModel, CreateBusinessUnitNotificationConfigurationContract> _createNotificationTranslator;
		public ITranslate<CreateNotificationModel, CreateBusinessUnitNotificationConfigurationContract> CreateNotificationTranslator
		{
			get
			{
				if (_createNotificationTranslator == null)
				{
					_createNotificationTranslator = new CreateNotificationTranslator(NotificationTypeTranslator);
				}
				return _createNotificationTranslator;
			}
		}

		private ITranslate<CreateBusinessUnitNotificationConfigurationResult, CreateNotificationModelResponse> _createNotificationResponseTranslator;
		public ITranslate<CreateBusinessUnitNotificationConfigurationResult, CreateNotificationModelResponse> CreateNotificationResponseTranslator
		{
			get
			{
				if (_createNotificationResponseTranslator == null)
					_createNotificationResponseTranslator = new CreateNotificationResultTranslator(_customAppConfiguration);
				return _createNotificationResponseTranslator;
			}
		}

		private ITranslate<UpdateDepartmentModel, UpdateDepartmentCostCenterContract> _updateDepartmentContractTranslator;
		public ITranslate<UpdateDepartmentModel, UpdateDepartmentCostCenterContract> UpdateDepartmentContractTranslator
		{
			get
			{
				if (_updateDepartmentContractTranslator == null)
					_updateDepartmentContractTranslator = new UpdateDepartmentContractTranslator();
				return _updateDepartmentContractTranslator;
			}
		}

		private ISetQuotaTranslator _setQuotaTranslator;
		public ISetQuotaTranslator SetQuotaTranslator
		{
			get
			{
				if (_setQuotaTranslator == null)
					_setQuotaTranslator = new SetQuotaTranslator();
				return _setQuotaTranslator;
			}
		}

        private ITranslate<GetCompanyLanguageContract[], AvailableLanguagesResponseModel> _availableLanguagesModelTranslator;
        public ITranslate<GetCompanyLanguageContract[], AvailableLanguagesResponseModel> AvailableLanguagesModelTranslator
        {
            get
            {
                if (_availableLanguagesModelTranslator == null)
                {
                    _availableLanguagesModelTranslator = new AvailableLanguagesResponseModelTranslator();
                }

                return _availableLanguagesModelTranslator;
            }
        }

        private ITranslate<GetAccountLanguageContract[], PreferredLanguageResponseModel> _preferredLanguageModelTranslator;
        public ITranslate<GetAccountLanguageContract[], PreferredLanguageResponseModel> PreferredLanguageModelTranslator
        {
            get
            {
                if (_preferredLanguageModelTranslator == null)
                {
                    _preferredLanguageModelTranslator = new PreferredLanguageResponseModelTranslator();
                }

                return _preferredLanguageModelTranslator;
            }
        }

        private ITranslate<UpdatePreferredLanguagesRequestModel, UpdateAccountLanguagesContract> _updatePreferredLanguageContractTranslator;
        public ITranslate<UpdatePreferredLanguagesRequestModel, UpdateAccountLanguagesContract> UpdatePreferredLanguageContractTranslator
        {
            get
            {
                if (_updatePreferredLanguageContractTranslator == null)
                {
                    _updatePreferredLanguageContractTranslator = new UpdatePreferredLanguagesContractTranslator();
                }

                return _updatePreferredLanguageContractTranslator;
            }
        }
    }
}
