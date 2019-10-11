using Andgasm.BB.Harvest;
using Andgasm.BB.SquadRegistration.Core;
using Andgasm.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Andgasm.BB.SquadRegistration.Svc
{
    class Startup
    {
        public IHostBuilder Host { get; internal set; }
        public IConfiguration Configuration { get; internal set; }

        public Startup()
        {
            Host = new HostBuilder()
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Environment.CurrentDirectory);
                config.AddJsonFile("appsettings.json", optional: false);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                //config.AddUserSecrets<Startup>();
                Configuration = config.Build();
            });
            ConfigureServices();
        }

        public void ConfigureServices()
        {
            Host.ConfigureServices((_hostcontext, services) =>
            {
                services.AddSingleton(sp =>
                {
                    return new BusSettings()
                    {
                        ServiceBusHost = Configuration.GetSection("ServiceBus")["ServiceBusHost"],
                        ServiceBusConnectionString = Configuration.GetSection("ServiceBus")["ServiceBusConnectionString"],
                        NewClubSeasonAssociationSubscriptionName = Configuration.GetSection("ServiceBus")["NewClubSeasonAssociationSubscriptionName"],
                        NewClubSeasonAssociationTopicName = Configuration.GetSection("ServiceBus")["NewClubSeasonAssociationTopicName"]
                    };
                });
                services.AddSingleton(sp =>
                {
                    return new ApiSettings()
                    {
                        PlayersDbApiRootKey = Configuration.GetSection("API")["PlayersDbApiRootKey"],
                        PlayerSquadRegistrationsApiPath = Configuration.GetSection("API")["PlayerSquadRegistrationsApiPath"]
                    };
                });

                services.AddLogging(loggingBuilder => loggingBuilder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug));

                services.AddTransient(typeof(SquadRegistrationHarvester));
                services.AddSingleton((ctx) =>
                {
                    return new HarvestRequestManager(ctx.GetService<ILogger<HarvestRequestManager>>(),
                                                     Convert.ToInt32(Configuration["MaxRequestsPerSecond"]));
                });

                services.AddSingleton(sp =>
                {
                    return ServiceBusFactory.GetBus(Enum.Parse<BusHost>(Configuration.GetSection("ServiceBus")["ServiceBusHost"]),
                                                                        Configuration.GetSection("ServiceBus")["ServiceBusConnectionString"],
                                                                        Configuration.GetSection("ServiceBus")["NewClubSeasonAssociationTopicName"],
                                                                        Configuration.GetSection("ServiceBus")["NewClubSeasonAssociationSubscriptionName"]);
                });

                services.AddScoped<IHostedService, SquadRegistrationExtractorSvc>();
            });
        }
    }
}
