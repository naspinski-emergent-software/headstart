using Avalara.AvaTax.RestClient;
using Flurl.Http;
using Flurl.Http.Configuration;
using Headstart.API.Commands;
using Headstart.API.Commands.Crud;
using Headstart.API.Commands.Zoho;
using Headstart.Common;
using Headstart.Common.Helpers;
using Headstart.Common.Models;
using Headstart.Common.Queries;
using Headstart.Common.Repositories;
using Headstart.Common.Services;
using Headstart.Common.Services.CMS;
using Headstart.Common.Services.Zoho;
using LazyCache;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrderCloud.SDK;
using ordercloud.integrations.smartystreets;
using ordercloud.integrations.easypost;
using ordercloud.integrations.avalara;
using ordercloud.integrations.cardconnect;
using ordercloud.integrations.exchangerates;
using ordercloud.integrations.library;
using SendGrid;
using SmartyStreets;
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.OpenApi.Models;
using OrderCloud.Catalyst;
using OrderCloud.Common.Services;

namespace Headstart.API
{
    public class Startup
    {
        private readonly AppSettings _settings;

        public Startup(AppSettings settings)
        {
            _settings = settings;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var cosmosConfig = new CosmosConfig(
                _settings.CosmosSettings.DatabaseName,
                _settings.CosmosSettings.EndpointUri,
                _settings.CosmosSettings.PrimaryKey,
                _settings.CosmosSettings.RequestTimeoutInSeconds
            );
            var cosmosContainers = new List<ContainerInfo>()
            {
                new ContainerInfo()
                {
                    Name = "rmas",
                    PartitionKey = "PartitionKey"
                }
            };

            var avalaraConfig = new AvalaraConfig()
            {
                BaseApiUrl = _settings.AvalaraSettings.BaseApiUrl,
                AccountID = _settings.AvalaraSettings.AccountID,
                LicenseKey = _settings.AvalaraSettings.LicenseKey,
                CompanyCode = _settings.AvalaraSettings.CompanyCode,
                CompanyID = _settings.AvalaraSettings.CompanyID
            };

            var currencyConfig = new BlobServiceConfig()
            {
                ConnectionString = _settings.BlobSettings.ConnectionString,
                Container = _settings.BlobSettings.ContainerNameExchangeRates
            };
            var middlewareErrorsConfig = new BlobServiceConfig()
            {
                ConnectionString = _settings.BlobSettings.ConnectionString,
                Container = "unhandled-errors-log"
            };

            var flurlClientFactory = new PerBaseUrlFlurlClientFactory();
            var smartyStreetsUsClient = new ClientBuilder(_settings.SmartyStreetSettings.AuthID, _settings.SmartyStreetSettings.AuthToken).BuildUsStreetApiClient();

            services
                .AddSingleton<ISimpleCache, LazyCacheService>() // Replace LazyCacheService with RedisService if you have multiple server instances.
                .ConfigureServices()
                .AddOrderCloudUserAuth<AppSettings>()
                .AddOrderCloudWebhookAuth(opts => opts.HashKey = _settings.OrderCloudSettings.WebhookHashKey)
                .InjectCosmosStore<LogQuery, OrchestrationLog>(cosmosConfig)
                .InjectCosmosStore<ReportTemplateQuery, ReportTemplate>(cosmosConfig)
                .AddCosmosDb(_settings.CosmosSettings.EndpointUri, _settings.CosmosSettings.PrimaryKey, _settings.CosmosSettings.DatabaseName, cosmosContainers)
                .Inject<IPortalService>()
                .Inject<ISmartyStreetsCommand>()
                .Inject<ICheckoutIntegrationCommand>()
                .Inject<IShipmentCommand>()
                .Inject<IOrderCommand>()
                .Inject<IPaymentCommand>()
                .Inject<IOrderSubmitCommand>()
                .Inject<IEnvironmentSeedCommand>()
                .Inject<IHSProductCommand>()
                .Inject<ILineItemCommand>()
                .Inject<IMeProductCommand>()
                .Inject<IHSCatalogCommand>()
                .Inject<IHSRegisterCommand>()
                .Inject<ISendgridService>()
                .Inject<IHSSupplierCommand>()
                .Inject<ICreditCardCommand>()
                .Inject<ISupportAlertService>()
                .Inject<IOrderCalcService>()
                .Inject<ISupplierApiClientHelper>()
                .AddSingleton<ICMSClient>(new CMSClient(new CMSClientConfig() { BaseUrl = _settings.CMSSettings.BaseUrl }))
                .AddSingleton<ISendGridClient>(x => new SendGridClient(_settings.SendgridSettings.ApiKey))
                .AddSingleton<IFlurlClientFactory>(x => flurlClientFactory)
                .AddSingleton<DownloadReportCommand>()
                .Inject<IRMARepo>()
                .Inject<IZohoClient>()
                .AddSingleton<IZohoCommand>(z => new ZohoCommand(new ZohoClient(
                    new ZohoClientConfig()
                    {
                        ApiUrl = "https://books.zoho.com/api/v3",
                        AccessToken = _settings.ZohoSettings.AccessToken,
                        ClientId = _settings.ZohoSettings.ClientId,
                        ClientSecret = _settings.ZohoSettings.ClientSecret,
                        OrganizationID = _settings.ZohoSettings.OrgID
                    }, flurlClientFactory),
                    new OrderCloudClient(new OrderCloudClientConfig
                    {
                        ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                        AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                        ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
                        ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
                        Roles = new[] { ApiRole.FullAccess }
                    })))
                .AddSingleton<IOrderCloudIntegrationsExchangeRatesClient, OrderCloudIntegrationsExchangeRatesClient>()
                .AddSingleton<IExchangeRatesCommand>(provider => new ExchangeRatesCommand( new OrderCloudIntegrationsBlobService(currencyConfig), flurlClientFactory, provider.GetService<ISimpleCache>()))
                .AddSingleton<IAvalaraCommand>(x => new AvalaraCommand(
                    avalaraConfig,
                    new AvaTaxClient("four51_headstart", "v1", "four51_headstart", new Uri(avalaraConfig.BaseApiUrl)
                   ).WithSecurity(_settings.AvalaraSettings.AccountID, _settings.AvalaraSettings.LicenseKey)))
                .AddSingleton<IEasyPostShippingService>(x => new EasyPostShippingService(new EasyPostConfig() { APIKey = _settings.EasyPostSettings.APIKey }))
                .AddSingleton<ISmartyStreetsService>(x => new SmartyStreetsService(_settings.SmartyStreetSettings, smartyStreetsUsClient))
                .AddSingleton<IOrderCloudIntegrationsCardConnectService>(x => new OrderCloudIntegrationsCardConnectService(_settings.CardConnectSettings, flurlClientFactory))
                .AddSingleton<IOrderCloudClient>(provider => new OrderCloudClient(new OrderCloudClientConfig
                {
                    ApiUrl = _settings.OrderCloudSettings.ApiUrl,
                    AuthUrl = _settings.OrderCloudSettings.ApiUrl,
                    ClientId = _settings.OrderCloudSettings.MiddlewareClientID,
                    ClientSecret = _settings.OrderCloudSettings.MiddlewareClientSecret,
                    Roles = new[]
                    {
                        ApiRole.FullAccess
                    }
                }))
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Headstart API", Version = "v1" });
                    c.CustomSchemaIds(x => x.FullName);
                });
            var serviceProvider = services.BuildServiceProvider();
            services
                .AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
                {
                    EnableAdaptiveSampling = false, // retain all data
                    InstrumentationKey = _settings.ApplicationInsightsSettings.InstrumentationKey
                });


            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            FlurlHttp.Configure(settings => settings.Timeout = TimeSpan.FromSeconds(_settings.FlurlSettings.TimeoutInSeconds));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            CatalystApplicationBuilder.CreateApplicationBuilder(app, env)
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"/swagger", $"API v1");
                    c.RoutePrefix = string.Empty;
                });
        }
    }
}
