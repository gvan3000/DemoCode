using Microsoft.Practices.Unity;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.Configuration.GetConfiguration;
using RestfulAPI.Logging;
using RestfulAPI.Logging.Loggers;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.ServiceTypeConfiguration;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Configuration
{
	public static class UnityConfig
	{
		/// <summary>Registers the type mappings with the Unity container.</summary>
		/// <param name="container">The unity container to configure.</param>
		/// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
		/// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
		public static void RegisterTypes(IUnityContainer container)
		{
			container.RegisterType<ITeleenaServiceUnitOfWork, TeleenaServiceUnitOfWork>(new HierarchicalLifetimeManager(), new InjectionConstructor());
			container.RegisterType<Common.Validation.IValidator, Common.Validation.Validator>();
			container.RegisterInstance<ISysCodeConstants>(SysCodeConstants.Instance);

			// translators
			container.RegisterType<IBusinessUnitApiTranslators, BusinessUnitApiTranslators>();

			// providers
			container.RegisterType<IBusinessUnitProvider, BusinessUnitProvider>();
			container.RegisterType<IProductProvider, ProductProvider>();
			container.RegisterType<IBalanceProvider, BalanceProvider>();
			container.RegisterType<IPropositionsProvider, PropositionsProvider>();
			container.RegisterType<IServiceTypeConfigurationProvider, ServiceTypeConfigurationProvider>();
			container.RegisterType<IMobileProvider, MobileProvider>();
			container.RegisterType<ISimProvider, SimProvider>();
			container.RegisterType<IAddOnProvider, AddOnProvider>();
			container.RegisterType<IQuotaDistributionProvider, QuotaDistributionProvider>();
			container.RegisterType<IProductTypeProvider, ProductTypeProvider>();
			container.RegisterType<IBalanceProfileProvider, BalanceProfileProvider>();
			container.RegisterType<IAPNProvider, APNProvider>();
			container.RegisterType<ISmscSettingProvider, SmscSettingsProvider>();
			container.RegisterType<IFeatureProvider, FeatureProvider>();
            container.RegisterType<IPreferredLanguageProvider, PreferredLanguageProvider>();
			container.RegisterType<IDepartmentProvider, DepartmentProvider>();
			container.RegisterType<INotificationConfigurationProvider, NotificationConfigurationProvider>();

			// validators
			container.RegisterType<IBusinessUnitValidators, BusinessUnitValidators>();

			// internal classes
			container.RegisterType<IBusinessUnitProducerFactory, BusinessUnitProducerFactory>();
			container.RegisterType<IAvailableMsisdnFactory, AvailableMsisdnFactory>();

			//application configuration reader
			container.RegisterType<ICustomAppConfiguration, CustomAppConfiguration>();

			//logger Settings
			List<string> properties = new List<string> { "password" };
			container.RegisterType<ILogFiltrationSettings, HideLogProperties>(new InjectionConstructor(properties));

			//logger
			container.RegisterType<IJsonRestApiLogger, JsonRestApiLogger>(new PerThreadLifetimeManager());
			container.RegisterType<IJsonRestApiLoggerProvider, LoggerProvider>();
		}
	}
}
