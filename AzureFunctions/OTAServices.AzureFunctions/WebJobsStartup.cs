using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OTAServices.AzureFunctions;
using OTAServices.Business.Functions.Helpers;
using OTAServices.Business.Functions.Implementations.OTACampaign;
using OTAServices.Business.Functions.Implementations.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Implementations.OTACampaignSubscribers;
using OTAServices.Business.Functions.Interfaces.OTACampaign;
using OTAServices.Business.Functions.Interfaces.OTACampaignDeleteImsi;
using OTAServices.Business.Functions.Interfaces.OTACampaignSubscribers;
using OTAServices.Business.Interfaces.UnitOfWork;
using OTAServices.DataCore.MaximityDb;
using OTAServices.DataCore.Repositories.OtaDb;
using OTAServices.DataCore.Repositories.ProvisioningDb;
using SimProfileServiceReference;
using System;
using System.IO;
using Teleena.Azure.KeyVault;
using Teleena.AzureFunctions.DependencyInjection;
using Teleena.AzureStorage.Client;
using Teleena.Sftp.Client;
using TeleenaFileLogging.AzureFunctions;
using TeleenaFileLogging.Interfaces;
using static SimProfileServiceReference.SimProfileServiceClient;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "InjectConfiguration")]
namespace OTAServices.AzureFunctions
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var pathRoot = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)).FullName;

            var config = new ConfigurationBuilder()
                .SetBasePath(pathRoot)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            string otaDbConnectionString = config.GetConnectionString("OTADbConnectionString");
            string provisioningDbConnectionString = config.GetConnectionString("ProvisioningDbConnectionString");
            string maximityDbConnectionString = config.GetConnectionString("MaximityDbConnectionString");

            string partialDeleteImsiCallbackUrl = $"http://{Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME")}/api/simotacampaigndeleteimsicallbacks";
#if DEBUG
            string azureStorageConnectionString = config["AzureWebJobsStorage"];
            string sftpAddress = config["SftpAddress"];
            int sftpServerPort = int.Parse(config["SftpServerPort"]);
            string sftpUserName = config["SftpUserName"];
            string sftpInboundPathOTACampaign = config["SftpInboundOTACampaignPath"];
            string sftpProcessedPathOTACampaign = config["SftpProcessedOTACampaignPath"];
            string sftpErrorPathOTACampaign = config["SftpErrorOTACampaignPath"];
            string simProfileServiceUri = config["SimProfileServiceUri"];
            string simServiceUri = config["SimServiceUri"];
            string provisioningServiceUri = config["ProvisioningServiceUri"];
            string imsiManagementServiceUri = config["ImsiManagementServiceUri"];
            string keyVaultClientId = config["AzureKeyVaultClientId"];
            string keyVaultClientSecret = config["AzureKeyVaultClientSecret"];
            string keyVaultSecretUri = config["AzureKeyVaultSecretUri"];
            string logEnvironment = config["LogEnvironment"];

            string ServiceBusConnection = config["ServiceBusConnection"];
            string ProvisioningBusAzureListenerQueueName = config["ProvisioningBusAzureListenerQueueName"];
            string OTABusAzureListenerTopicName = config["OTAServiceBusAzureListenerTopicName"];
            string DeleteImsiCallbackResutsQueueName = config["DeleteImsiCallbackResutsQueueName"];
#else
            string azureStorageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage", EnvironmentVariableTarget.Process);
            string sftpAddress = Environment.GetEnvironmentVariable("SftpAddress", EnvironmentVariableTarget.Process);
            int sftpServerPort = int.Parse(Environment.GetEnvironmentVariable("SftpServerPort", EnvironmentVariableTarget.Process));
            string sftpUserName = Environment.GetEnvironmentVariable("SftpUserName", EnvironmentVariableTarget.Process);
            string sftpInboundPathOTACampaign = Environment.GetEnvironmentVariable("SftpInboundOTACampaignPath", EnvironmentVariableTarget.Process);
            string sftpProcessedPathOTACampaign = Environment.GetEnvironmentVariable("SftpProcessedOTACampaignPath", EnvironmentVariableTarget.Process);
            string sftpErrorPathOTACampaign = Environment.GetEnvironmentVariable("SftpErrorOTACampaignPath", EnvironmentVariableTarget.Process);
            string simProfileServiceUri = Environment.GetEnvironmentVariable("SimProfileServiceUri", EnvironmentVariableTarget.Process);
            string simServiceUri = Environment.GetEnvironmentVariable("SimServiceUri", EnvironmentVariableTarget.Process);
            string provisioningServiceUri = Environment.GetEnvironmentVariable("ProvisioningServiceUri", EnvironmentVariableTarget.Process);
            string imsiManagementServiceUri = Environment.GetEnvironmentVariable("ImsiManagementServiceUri", EnvironmentVariableTarget.Process);
            string keyVaultClientId = Environment.GetEnvironmentVariable("AzureKeyVaultClientId", EnvironmentVariableTarget.Process);
            string keyVaultClientSecret = Environment.GetEnvironmentVariable("AzureKeyVaultClientSecret", EnvironmentVariableTarget.Process);
            string keyVaultSecretUri = Environment.GetEnvironmentVariable("AzureKeyVaultSecretUri", EnvironmentVariableTarget.Process);
            string logEnvironment = Environment.GetEnvironmentVariable("LogEnvironment", EnvironmentVariableTarget.Process);

            string ServiceBusConnection = Environment.GetEnvironmentVariable("ServiceBusConnection", EnvironmentVariableTarget.Process);
            string ProvisioningBusAzureListenerQueueName = Environment.GetEnvironmentVariable("ProvisioningBusAzureListenerQueueName", EnvironmentVariableTarget.Process);
            string OTABusAzureListenerTopicName = Environment.GetEnvironmentVariable("OTAServiceBusAzureListenerTopicName", EnvironmentVariableTarget.Process);
            string DeleteImsiCallbackResutsQueueName = Environment.GetEnvironmentVariable("DeleteImsiCallbackResutsQueueName", EnvironmentVariableTarget.Process);
#endif

            builder.ConfigureLogging(pathRoot, logEnvironment);

            var privateKey = AzureKeyVaultHelper.GetSecret(keyVaultSecretUri, new KeyVaultClientInfo(keyVaultClientId, keyVaultClientSecret)).GetAwaiter().GetResult();

            builder.Services.AddTransient(typeof(ITeleenaSftpClient), provider =>
                new TeleenaSftpClient(sftpAddress, sftpServerPort, sftpUserName, privateKey));

            builder.Services.AddTransient(typeof(IAzureStorageClient), provider =>
                new AzureStorageClient(azureStorageConnectionString));

            builder.Services.AddTransient(typeof(IProvisioningServicesBusQueueClient), provider =>
                new AzureServiceBusClient(
                    ServiceBusConnection,
                    ProvisioningBusAzureListenerQueueName
));

            builder.Services.AddTransient(typeof(IOTAServicesBusTopicClient), provider =>
                new AzureServiceBusClient(
                    ServiceBusConnection,
                    OTABusAzureListenerTopicName));

            builder.Services.AddTransient(typeof(IDeleteImsiCallbackResponseQueueClient), provider =>
                new AzureServiceBusClient(
                    ServiceBusConnection,
                    DeleteImsiCallbackResutsQueueName
                    ));

            builder.Services.AddSingleton(typeof(IDateTimeProvider), provider => new DateTimeProvider());

            //For services to work with azure functions there is a bug and workaround - https://github.com/dotnet/wcf/issues/2824 which is implemented
            builder.Services.AddTransient(typeof(SimProfileService), provider => new SimProfileServiceClient(EndpointConfiguration.NetTcpBinding_SimProfileService, simProfileServiceUri));

            builder.Services.AddTransient(typeof(OtaDbContext), provider => new OtaDbContext(otaDbConnectionString));
            builder.Services.AddTransient(typeof(ProvisioningDbContext), provider => new ProvisioningDbContext(provisioningDbConnectionString));
            builder.Services.AddTransient(typeof(MaximityDbContext), provider => new MaximityDbContext(maximityDbConnectionString));
            builder.Services.AddTransient(typeof(SimProfileService), provider => new SimProfileServiceClient(EndpointConfiguration.NetTcpBinding_SimProfileService, simProfileServiceUri));

            builder.Services.AddTransient(typeof(IProvisioningDbUnitOfWork), provider => new ProvisioningDbUnitOfWork(provider.GetService<ProvisioningDbContext>()));
            builder.Services.AddTransient(typeof(IMaximityDbUnitOfWork), provider => new MaximityDbUnitOfWork(provider.GetService<MaximityDbContext>()));
            builder.Services.AddTransient(typeof(IOtaDbUnitOfWork), provider => new OtaDbUnitOfWork(provider.GetService<OtaDbContext>()));

            builder.Services.AddTransient(typeof(IOTACampaignValidateData), provider => new OTACampaignValidateData(
                provider.GetService<SimProfileService>(),
                provider.GetService<IProvisioningDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignParseData), provider => new OTACampaignParseData(provider.GetService<IJsonLogger>()));
            builder.Services.AddTransient(typeof(IOTACampaignSaveCampaing), provider => new OTACampaignSaveCampaing(
                provider.GetService<IOtaDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignFinalizeProcessing), provider => new OTACampaignFinalizeProcessing(
                sftpInboundPathOTACampaign,
                sftpProcessedPathOTACampaign,
                sftpErrorPathOTACampaign,
                provider.GetService<ITeleenaSftpClient>(),
                provider.GetService<IDateTimeProvider>(),
                provider.GetService<IAzureStorageClient>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersParseData), provider => new OTACampaignSubscribersParseData(                
                provider.GetService<IOtaDbUnitOfWork>(),
                provider.GetService<IProvisioningDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersValidate), provider => new OTACampaignSubscribersValidate(
                provider.GetService<SimProfileService>(),
                provider.GetService<IProvisioningDbUnitOfWork>(),
                provider.GetService<IMaximityDbUnitOfWork>(),
                provider.GetService<IOtaDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersUpdateSimProfile), provider => new OTACampaignSubscribersUpdateSimProfile(
               provider.GetService<IProvisioningDbUnitOfWork>(),
               provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersLockProducts), provider => new OTACampaignSubscribersLockProducts(
                provider.GetService<IMaximityDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersLeaseImsi), provider => new OTACampaignSubscribersLeaseImsi(
                provider.GetService<IProvisioningDbUnitOfWork>(),
                provider.GetService<IProvisioningServicesBusQueueClient>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersSaveOasisRequest), provider => new OTACampaignSubscribersSaveOasisRequest(
                provider.GetService<IOtaDbUnitOfWork>(),
                provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersEnrichOasisRequest), provider => new OTACampaignSubscribersEnrichOasisRequest(
               provider.GetService<IMaximityDbUnitOfWork>(),
               provider.GetService<IProvisioningDbUnitOfWork>(),
               provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersTriggerSaga), provider => new OTACampaignSubscribersTriggerSaga(
                 provider.GetService<IOTAServicesBusTopicClient>(),
                 partialDeleteImsiCallbackUrl,
                 provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignSubscribersFinalizeProcessing), provider => new OTACampaignSubscribersFinalizeProcessing(
                sftpInboundPathOTACampaign,
                sftpProcessedPathOTACampaign,
                sftpErrorPathOTACampaign,
                provider.GetService<ITeleenaSftpClient>(),
                provider.GetService<IDateTimeProvider>(),
                provider.GetService<IAzureStorageClient>(),
                provider.GetService<IJsonLogger>()));


            builder.Services.AddTransient(typeof(IOTACampaignDeleteImsiCallback), provider => 
                new OTACampaignDeleteImsiCallback(provider.GetService<IDeleteImsiCallbackResponseQueueClient>(), provider.GetService<IJsonLogger>()));

            builder.Services.AddTransient(typeof(IOTACampaignProcessDeleteImsiCallback), provider => 
                new OTACampaignProcessDeleteImsiCallback(provider.GetService<IOtaDbUnitOfWork>(), provider.GetService<IJsonLogger>()));


            var serviceProvider = builder.Services.BuildServiceProvider(true);

            var injectConfiguration = new InjectConfiguration(serviceProvider);

            builder.AddExtension(injectConfiguration);
        }
    }
}