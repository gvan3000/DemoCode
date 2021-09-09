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
using RestfulAPI.BusinessUnitApi.Domain.Translators.BusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Department;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Proposition;
using RestfulAPI.BusinessUnitApi.Domain.Translators.Quota;
using RestfulAPI.Common;
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
using RestfulAPI.TeleenaServiceReferences.Translators;

namespace RestfulAPI.BusinessUnitApi.Domain.Translators
{
	/// <summary>
	/// Collection of wcf service to REST api and back translators
	/// </summary>
	public interface IBusinessUnitApiTranslators
	{
		/// <summary>
		/// Used for converting into account creation contract for wcf
		/// </summary>
		ITranslate<BusinessUnitCreateModel, AddAccountContract> CreateBusinessUnitTranslator { get; }
		/// <summary>
		/// Used for converting single account proposition when creating account on wcf
		/// </summary>
		ITranslate<Models.BusinessUnitModels.Proposition, AccountPropositionsContract> CreateBusinessUnitPropositionTranslator { get; }
		/// <summary>
		/// Used to convert from wcf into REST model for single product
		/// </summary>
		ITranslate<GetProductResponse, ProductModel> ProductModelTranslator { get; }
		/// <summary>
		/// Used to convert a list of products from wcf to REST model
		/// </summary>
		ITranslate<GetProductResponse[], List<ProductModel>> ProductListTranslator { get; }

		/// <summary>
		/// Used to convert a list of products and imeis from wcf to REST model
		/// </summary>
		ITranslate<ProductImeiByBusinessUnitDataContract[], ProductImeiListModel> ProductImeiListTranslator { get; }

		/// <summary>
		/// Used to convert and create tree structure from list of accounts from wcf
		/// </summary>
		IAccountContractTranslator AccountContractTranslator { get; }

		/// <summary>
		/// Balance translator to convert Services Balance to Model
		/// </summary>
		IBalanceTranslator BalanceTranslator { get; }

		/// <summary>
		/// Propositions translator to convert model to service contract
		/// </summary>
		IPropositionContractTranslator PropositionsContractTranslator { get; }

		/// <summary>
		/// Msisdn Translator to convert service contract to REST model
		/// </summary>
		ITranslate<MsisdnContract[], AvailableMSISDNResponseModel> MsisdnContractTranslator { get; }

		/// <summary>
		/// Sim Status Translator to convert status name to status code
		/// </summary>
		ITranslate<string, System.Guid> SimStatusTranslator { get; }

		/// <summary>
		/// Sim Status Translator to convert status Guid to status string
		/// </summary>
		ITranslate<System.Guid, SimStatusWrapper> SimStatusGuidToStringTranslator { get; }

		/// <summary>
		/// Convert BusinessUnitPatchModel propositions to UpdateBusinessUnitPropositionsContract
		/// </summary>
		ITranslate<BusinessUnitPatchModel, UpdateBusinessUnitPropositionsContract> BusinessUnitPatchModelPropositionsTranslator { get; }

		/// <summary>
		/// Convert  PurchaseAddOnResultContract to AddAddOnResponseModel
		/// </summary>
		ITranslate<PurchaseAddOnResultContract, AddAddOnResponseModel> PurchaseAddOnResultTranslator { get; }

		/// <summary>
		/// Creates AllowedAddOnsContract from businessUnitId, ComapnyId, AddOnId
		/// </summary>
		IAllowedAddOnsContractTranslator AllowedAddOnsContractTranslator { get; }

		/// <summary>
		/// Translate from Wcf contract to Rest model
		/// </summary>
		IAddOnContractTranslator AddOnContractTranslator { get; }

		/// <summary>
		/// Translates from WCF contract to Proposition model
		/// </summary>
		ITranslate<AllowedPropositionContract, Models.BusinessUnitModels.Proposition> AllowedPropositionContractTranslator { get; }

		/// <summary>
		/// Translates DeleteAddOn model to WCF contract
		/// </summary>
		IDeleteAddOnModelTranslator DeleteAddOnModelTranslator { get; }

		/// <summary>
		/// Convert SetBalanceModel, productId, busienssUnitId to wcf SaveQuotaDistributionContract
		/// </summary>
		ISaveQuotaDistributionContractTranslator SaveQuotaDistributionContactTranslator { get; }

		/// <summary>
		/// Convert service contract to API model
		/// </summary>
		ITranslate<ProductQuotaDistributionContract, BalanceQuotasListModel> ProductQuotaDistributionContractTranslator { get; }

		/// <summary>
		/// Translate ProductTypeContract array to ProductTypeListResponseModel
		/// </summary>
		ITranslate<ProductTypeContract[], ProductTypeListResponseModel> ProductTypeContractTranslator { get; }

		/// <summary>
		/// Translate BusinessUnitPurchasedAddOnListContract to AddOnCumulativeListModel
		/// </summary>
		ITranslate<BusinessUnitPurchasedAddOnListContract, AddOnCumulativeListModel> BusinessUnitPurchasedAddOnsCumulativeTranslator { get; }

		/// <summary>
		/// Translate single syscode into balance profile model
		/// </summary>
		ITranslate<SysCodeContract, BalanceProfileModel> BalanceProfileTranslator { get; }

		/// <summary>
		/// Translate list of syscodes into balance profile ist model
		/// </summary>
		ITranslate<SysCodeContract[], BalanceProfileListModel> BalanceProfileListTranslator { get; }

		/// <summary>
		/// Translate product allowed balances contract into model
		/// </summary>
		ITranslate<List<ProductAllowedBalancesContract>, ProductAllowedBalanceList> ProductAllowedBalancesTranslator { get; }

		/// <summary>
		///   Translate list of AccountBalanceWithBucketsContracts into BalanceQuotasListModel
		/// </summary>
		ITranslate<AccountBalanceWithBucketsContract[], BalanceQuotasListModel> BalancesContractBalanceQuotaListModelTranslator { get; }

		/// <summary>
		/// Translate PropositionsContract to list of propostion inof model
		/// </summary>
		ITranslate<PropositionsContract, List<PropositionInfoModel>> PropositionInfoModelTranslator { get; }

		/// <summary>
		/// Translates input pageSize to PageSizeInfo model
		/// </summary>
		ITranslate<int?, PageSizeInfo> PageSizeInfoTranslator { get; }

		ITranslate<ApnSetWithDetailsContract[], APNSetList> APNTranslator { get; }

        /// <summary>
        /// Translates GetCompanyLanguageContract to AvailableLanguagesResponseModel
        /// </summary>
        ITranslate<GetCompanyLanguageContract[], AvailableLanguagesResponseModel> AvailableLanguagesModelTranslator { get; }

        /// <summary>
        /// Translates GetAccountLanguageContract to PreferredLanguageResponseModel
        /// </summary>
        ITranslate<GetAccountLanguageContract[], PreferredLanguageResponseModel> PreferredLanguageModelTranslator { get; }

        /// <summary>
        /// Translates UpdatePreferredLanguagesRequestModel to UpdateAccountLanguagesContract
        /// </summary>
        ITranslate<UpdatePreferredLanguagesRequestModel, UpdateAccountLanguagesContract> UpdatePreferredLanguageContractTranslator { get; }

        /// <summary>
        /// Translates updated apn list for business unit to wcf service contract
        /// </summary>
        IUpdateApnsTranslator UpdateApnsTranslator { get; }

		/// <summary>
		/// Translates  ApnDetailContract array to APNsResponseModel
		/// </summary>
		ITranslate<ApnDetailContract[], APNsResponseModel> ApnsResponseModelTranslator { get; }

		/// <summary>
		/// Translates delete apn contract
		/// </summary>
		IDeleteApnTranslator DeleteApnTranslator { get; }

		/// <summary>
		/// Translates apn list of business unit to wcf service contract
		/// </summary>
		IUpdateDefaultApnTranslator UpdateDefaultApnTranslator { get; }

		/// <summary>
		/// Translates Department Model to AddDepartmentContract
		/// </summary>
		ICreateDepartmentContractTranslator CreateDepartmentContractTranslator { get; }

		/// <summary>
		/// Translates Department response contract to CreateDepartment Response Model
		/// </summary>
		ITranslate<DepartmentCostCenterContract, CreateDepartmentResponseModel> DepartmentModelTranslator { get; }

		/// <summary>
		/// Translates list of department cost center contract
		/// </summary>
		IGetDepartmentsModelTranslator GetDepartmentsModelTranslator { get; }

		/// <summary>
		/// Translates GetNotificationConfigurationListResponse contract to GetNotificationListDataModel
		/// </summary>
		ITranslate<GetNotificationConfigurationListResponse, GetNotificationListDataModel> NotificationListDataModelTranslator { get; }

        /// <summary>
        /// Translates update model to service request
        /// </summary>
        ITranslate<UpdateNotificationModel, UpdateBusinessUnitNotificationConfigurationContract> UpdateNotificationConfigurationTranslator { get; }

		/// <summary>
		/// Translates between backedn notification config service event types and restful api ones
		/// </summary>
		INotificationTypeTranslator NotificationTypeTranslator { get; }

		/// <summary>
		/// Translates create business unit notification request to appropriate wcf service contract
		/// </summary>
		ITranslate<CreateNotificationModel, CreateBusinessUnitNotificationConfigurationContract> CreateNotificationTranslator { get; }

		/// <summary>
		/// Translates create response from service into contract returned by api
		/// </summary>
		ITranslate<CreateBusinessUnitNotificationConfigurationResult, CreateNotificationModelResponse> CreateNotificationResponseTranslator { get; }

		/// <summary>
		/// Translates update department model into service request
		/// </summary>
		ITranslate<UpdateDepartmentModel, UpdateDepartmentCostCenterContract> UpdateDepartmentContractTranslator { get; }

		/// <summary>
		/// Produces set quota contract to change business unit quota
		/// </summary>
		ISetQuotaTranslator SetQuotaTranslator { get; }
	}
}
