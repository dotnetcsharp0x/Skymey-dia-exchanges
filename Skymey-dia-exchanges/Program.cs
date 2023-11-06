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
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Skymey_dia_exchanges
{

    class Program
    {
        private static RestClient _client;
        private static RestRequest _request;
        private static MongoClient _mongoClient;
        private static ApplicationContext _db;
        static async Task Main(string[] args)
        {
            _client = new RestClient("https://api.diadata.org/v1/exchanges");
            _request = new RestRequest("https://api.diadata.org/v1/exchanges", Method.Get);
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
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
        GetExchanges ge = new GetExchanges();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ge.GetExchangesFromDia();
                    await Task.Delay(TimeSpan.FromSeconds(60));
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

