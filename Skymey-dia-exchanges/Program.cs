using DnsClient.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Nancy.Json;
using RestSharp;
using Skymey_dia_exchanges.Actions.Exchanges;
using Skymey_dia_exchanges.Data;
using Skymey_dia_exchanges.Models;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Skymey_dia_exchanges
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddSingleton<IHostedService, MySpecialService>();
                });

            await builder.RunConsoleAsync();
        }
    }

    public class MySpecialService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            GetExchanges ge = new GetExchanges();
            while (!stoppingToken.IsCancellationRequested)
            {
                ge.GetExchangesFromDia();
                await Task.Delay(TimeSpan.FromSeconds(60));
            }
        }
    }
}

